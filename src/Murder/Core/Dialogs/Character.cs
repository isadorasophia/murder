using Bang;
using Bang.Components;
using Bang.Entities;
using Murder.Components;
using Murder.Save;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Murder.Messages;
using Murder.Diagnostics;

namespace Murder.Core.Dialogs
{
    public class Character
    {
        /// <summary>
        /// The guid of the character asset being tracked by this.
        /// </summary>
        private readonly Guid _guid;
        
        /// <summary>
        /// All situations for the character.
        /// </summary>
        public ImmutableDictionary<int, Situation> Situations;

        /// <summary>
        /// This is the current situation that the charcterr is currently executing.
        /// </summary>
        private int _currentSituation = 0;

        /// <summary>
        /// This is the dialog within the situation that is currently active.
        /// This is zero if it hasn't started yet.
        /// This is null if the situation has no more viable states.
        /// Not persisted.
        /// </summary>
        private int? _currentDialog = null;

        /// <summary>
        /// Active line in the current dialog.
        /// Not persisted.
        /// </summary>
        private int _activeLine = 0;

        private Situation ActiveSituation => Situations[_currentSituation];

        private Dialog ActiveDialog => ActiveSituation.Dialogs[_currentDialog!.Value];

        private Dialog DialogAt(int id) => ActiveSituation.Dialogs[id];

        public Character(Guid guid, ImmutableArray<Situation> situations)
        {
            Situations = situations.ToDictionary(s => s.Id, s => s).ToImmutableDictionary();

            _guid = guid;
        }

        public void StartAtSituation(int situation)
        {
            // Reset the state.
            _currentDialog = 0;
            _activeLine = 0;

            if (!Situations.ContainsKey(situation))
            {
                GameLogger.Error($"Invalid initial situation of {situation} set to dialog '{_guid}'.");

                _currentDialog = null;
                return;
            }

            // We also have our built-in character support for amount of times we interacted with a speaker.
            //tracker.SetInt(BaseCharacterBlackboard.Name, nameof(BaseCharacterBlackboard.TotalInteractions),
            //    BlackboardActionKind.Add, 1, _guid);

            _currentSituation = situation;
        }

        /// <summary>
        /// Returns whether the active dialog state for this dialogue is valid or not.
        /// </summary>
        [MemberNotNullWhen(true, nameof(_currentDialog))]
        private bool HasNext(World world, Entity? target)
        {
            // Are we in the initial state? If so, calculate the next outcome.
            if (_currentDialog == 0)
            {
                _ = TryMatchNextDialog(world, target);
            }

            return _currentDialog != null;
        }

        public Line? NextLine(World world, Entity? target = null)
        {
            if (!HasNext(world, target))
            {
                return null;
            }

            Dialog dialog = ActiveSituation.Dialogs[_currentDialog.Value];
            while (true)
            {
                if (_activeLine < dialog.Lines.Length)
                {
                    return FormatLine(dialog.Lines[_activeLine++]);
                }
                else
                {
                    // These dialog lines are *done*, so move on to the next thing.

                    // First, do all the actions for this dialog.
                    DoActionsForActiveDialog(world, target);

                    if (!TryMatchNextDialog(world, target))
                    {
                        return null;
                    }

                    dialog = ActiveSituation.Dialogs[_currentDialog.Value];
                }
            }
        }

        /// <summary>
        /// This looks for the next dialog most eligible to be triggered.
        /// </summary>
        [MemberNotNullWhen(true, nameof(_currentDialog))]
        private bool TryMatchNextDialog(World world, Entity? target = null)
        {
            if (_currentDialog is null)
            {
                return false;
            }

            BlackboardTracker tracker = Game.Data.ActiveSaveData.BlackboardTracker;
            Situation situation = ActiveSituation;
            Dialog activeDialog = ActiveDialog;

            if (activeDialog.GoTo is int @goto)
            {
                if (@goto == -1)
                {
                    // This is an exit leaf, end it right away.
                    return false;
                }

                StartAtSituation(@goto);
                return true;
            }

            if (!situation.Edges.TryGetValue(_currentDialog.Value, out DialogEdge edge))
            {
                return false;
            }

            ImmutableArray<int> neighbours = edge.Dialogs;

            List<int> candidates = new();
            foreach (int d in neighbours)
            {
                int count = tracker.PlayCount(_guid, _currentSituation, d);

                Dialog dialog = DialogAt(d);

                // Skip dialogs that should only be played once.
                if (dialog.PlayUntil != -1 && count >= dialog.PlayUntil)
                {
                    continue;
                }

                candidates.Add(d);
            }

            if (candidates.Count == 0)
            {
                // There are either no neighbours or they have all been played up to their limit.
                return false;
            }

            int? result = null;
            switch (edge.Kind)
            {
                case MatchKind.Next:
                case MatchKind.IfElse:
                    result = ChooseNextDialog(world, target, candidates);
                    break;

                case MatchKind.HighestScore:
                    result = ChooseBestScoreDialog(world, target, candidates);
                    break;

                case MatchKind.Random:
                    result = ChooseRandomDialog(candidates);
                    break;

                case MatchKind.Choice:
                    // TODO: Implement.
                    break;
            }

            if (result is null)
            {
                return false;
            }

            // Reset the line for the next block.
            _activeLine = 0;
            _currentDialog = result.Value;

            // Keep track of the counter!
            tracker.Track(_guid, _currentSituation, _currentDialog.Value);

            return true;
        }

        /// <summary>
        /// Returns the dialog with the highest score in <paramref name="candidates"/>.
        /// </summary>
        private int? ChooseBestScoreDialog(World world, Entity? target, IList<int> candidates)
        {
            BlackboardTracker tracker = Game.Data.ActiveSaveData.BlackboardTracker;

            // First, rank the dialog options according to criteria matching.
            List<(int DialogIndex, float Score)> dialogScore = new();

            foreach (int dialogIndex in candidates)
            {
                // We want to prioritize dialogs with the same score that haven't been played before.
                // So we subtract the total number of plays.
                int count = tracker.PlayCount(_guid, _currentSituation, dialogIndex);
                float score = 1 - count / 100f;

                Dialog dialog = DialogAt(dialogIndex);

                if (CheckRequirements(world, target, dialog.Requirements, out int requirementsScore))
                {
                    score += requirementsScore;
                    dialogScore.Add((dialogIndex, score));
                }
            }

            if (dialogScore.Count == 0)
            {
                // No valid dialogs were found.
                return null;
            }

            int bestMatchIndex = dialogScore.OrderByDescending(d => d.Score).First().DialogIndex;
            return bestMatchIndex;
        }

        /// <summary>
        /// Returns the first dialog that matches in <paramref name="candidates"/>.
        /// </summary>
        private int? ChooseNextDialog(World world, Entity? target, IList<int> candidates)
        {
            foreach (int dialogIndex in candidates)
            {
                Dialog dialog = DialogAt(dialogIndex);

                if (CheckRequirements(world, target, dialog.Requirements, out _))
                {
                    return dialogIndex;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a random dialog in <paramref name="candidates"/>.
        /// </summary>
        private int ChooseRandomDialog(IList<int> candidates)
        {
            return RandomExtensions.AnyOf(Game.Random, candidates);
        }

        public bool CheckRequirements(World world, ImmutableArray<CriterionNode> requirements, out int score) =>
            CheckRequirements(world, target: null, requirements, out score);

        private bool CheckRequirements(World world, Entity? target, ImmutableArray<CriterionNode> requirements, out int score)
        {
            bool valid = true;
            score = 1;

            CriterionNodeKind NextNodeKind(int i)
            {
                if (i < 0 || i >= requirements.Length)
                {
                    return CriterionNodeKind.And;
                }

                return requirements[i].Kind;
            }

            BlackboardTracker tracker = Game.Data.ActiveSaveData.BlackboardTracker;
            int? targetEntityId = target?.TryGetIdTarget()?.Target;

            int pendingScore = 0;
            for (int i = 0; i < requirements.Length && valid; ++i)
            {
                CriterionNodeKind nextKind = NextNodeKind(i + 1);

                CriterionNode criterion = requirements[i];

                if (tracker.Matches(criterion.Criterion, _guid, world, targetEntityId, out int weight))
                {
                    pendingScore = Math.Max(pendingScore, weight);

                    if (nextKind == CriterionNodeKind.And)
                    {
                        score += weight;
                        pendingScore = 0;
                    }
                    else
                    {
                        // continue looking for the highest pending score.
                    }
                }
                else
                {
                    if (nextKind == CriterionNodeKind.And)
                    {
                        // This means this is a NO match.
                        if (criterion.Kind == CriterionNodeKind.And)
                        {
                            valid = false;
                            break;
                        }
                        else
                        {
                            // Even coming from an "or", there wasn't any match.
                            if (pendingScore == 0)
                            {
                                valid = false;
                                break;
                            }
                            else
                            {
                                score += pendingScore;
                                continue;
                            }
                        }
                    }
                    else // nextKind is or
                    {
                        continue;
                    }
                }
            }

            return valid;
        }

        private Line FormatLine(Line line)
        {
            string? text = line.Text;
            if (text is null)
            {
                return line;
            }

            if (!BlackboardHelpers.FormatText(text, out string result))
            {
                return line;
            }

            return line.WithText(result);
        }

        private bool DoActionsForActiveDialog(World world, Entity? target)
        {
            // First, do all the actions for this dialog.
            if (_currentDialog != null && ActiveDialog.Actions is ImmutableArray<DialogAction> actions)
            {
                Entity? actionEntity = null;

                BlackboardTracker tracker = Game.Data.ActiveSaveData.BlackboardTracker;
                foreach (DialogAction action in actions)
                {
                    if (action.ComponentValue is not null)
                    {
                        actionEntity ??= CreateEntityForAction(world, target);
                    }

                    DoAction(actionEntity, tracker, action);
                }

                // If an entity has been created, trigger it immediately.
                actionEntity?.SendMessage<InteractMessage>();

                return true;
            }

            return false;
        }

        private void DoAction(Entity? actionEntity, BlackboardTracker tracker, DialogAction action)
        {
            if (action.ComponentValue is IComponent component)
            {
                // We need to guarantee that any modifiable components added here are safe.
                component = component is IModifiableComponent ? SerializationHelper.DeepCopy(component) : component;

                actionEntity?.AddComponent(component, component.GetType());
                return;
            }
            
            if (action.Fact is not Fact fact)
            {
                return;
            }
            
            switch (fact.Kind)
            {
                case FactKind.Bool:
                    tracker.SetBool(fact.Blackboard, fact.Name, action.BoolValue!.Value);
                    break;

                case FactKind.Int:
                    tracker.SetInt(fact.Blackboard, fact.Name, action.Kind, action.IntValue!.Value);
                    break;

                case FactKind.String:
                    tracker.SetString(fact.Blackboard, fact.Name, action.StrValue!);
                    break;
            }
        }

        /// <summary>
        /// When a dialog creates an entity, we propagate the relevant target values.
        /// </summary>
        /// <param name="world">Current world that the entity will be added.</param>
        /// <param name="target">Entity that originally triggered the action.</param>
        private Entity CreateEntityForAction(World world, Entity? target)
        {
            Entity result = world.AddEntity();

            if (target is null)
            {
                return result;
            }

            // Now, we should also propagate any targets that the entity hold.
            if (target.TryGetIdTarget() is IdTargetComponent idTarget)
            {
                result.SetIdTarget(idTarget);
            }
            else
            {
                // Otherwise, propagate the target entity through this component.
                result.SetIdTarget(target.EntityId);
            }

            if (target.TryGetIdTargetCollection() is IdTargetCollectionComponent idTargetCollection)
            {
                result.SetIdTargetCollection(idTargetCollection);
            }

            if (target.TryGetSpeaker() is SpeakerComponent speaker)
            {
                result.SetSpeaker(speaker);
            }

            return result;
        }
    }
}
