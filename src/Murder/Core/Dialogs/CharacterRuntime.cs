using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Interactions;
using Murder.Assets;
using Murder.Assets.Sounds;
using Murder.Components;
using Murder.Diagnostics;
using Murder.Messages;
using Murder.Save;
using Murder.Services;
using Murder.Utilities;
using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Core.Dialogs
{
    public class CharacterRuntime
    {
        private readonly Character _character;

        /// <summary>
        /// The guid of the character asset being tracked by this.
        /// </summary>
        private Guid Guid => _character.Guid;

        /// <summary>
        /// This is the current situation that the character is currently executing.
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

        private Situation ActiveSituation => _character.Situations[_currentSituation];

        private Dialog ActiveDialog => ActiveSituation.Dialogs[_currentDialog!.Value];

        private Dialog DialogAt(int id) => ActiveSituation.Dialogs[id];

        public CharacterRuntime(Character character, int situation)
        {
            _character = character;

            StartAtSituation(situation);
        }

        public void StartAtSituation(int situation)
        {
            // Reset the state.
            _currentDialog = 0;
            _activeLine = 0;

            if (!_character.Situations.ContainsKey(situation))
            {
                GameLogger.Error($"Invalid initial situation of {situation} set to dialog '{_character.Guid}'.");

                _currentDialog = null;
                return;
            }

            _currentSituation = situation;
        }

        /// <summary>
        /// Returns whether the active dialog state for this dialogue is valid or not.
        /// </summary>
        [MemberNotNullWhen(true, nameof(_currentDialog))]
        public bool HasNext(World world, Entity? target, bool track = false)
        {
            // Are we in the initial state? If so, calculate the next outcome.
            if (_currentDialog == 0)
            {
                _ = TryMatchNextDialog(world, track, target);
            }

            return _currentDialog != 0 && _currentDialog != null;
        }

        public bool HasContentOnNextDialogueLine(World world, Entity? target, bool checkForNewContentOnly)
        {
            // Are we in the initial state? If so, calculate the next outcome.
            if (_currentDialog == 0)
            {
                _ = TryMatchNextDialog(world, track: false, target);
            }

            if (!HasNext(world, target))
            {
                return false;
            }

            Dialog d = ActiveSituation.Dialogs[_currentDialog.Value];
            if (d.Lines.Length == 0 && d.GoTo == -1)
            {
                return false;
            }

            while (true)
            {
                if (d.Lines.Length != 0)
                {
                    break;
                }

                if (!TryMatchNextDialog(world, track: false, target))
                {
                    return false;
                }

                d = ActiveSituation.Dialogs[_currentDialog.Value];
            }

            if (checkForNewContentOnly)
            {
                BlackboardTracker tracker = Game.Data.ActiveSaveData.BlackboardTracker;

                return tracker.PlayCount(Guid, _currentSituation, _currentDialog.Value) == 0;
            }

            return d.Lines.Length > 0;
        }

        public DialogLine? NextLine(World world, Entity? target = null)
        {
            if (!HasNext(world, target, track: true))
            {
                return null;
            }

            Dialog dialog = ActiveDialog;
            while (true)
            {
                if (dialog.IsChoice)
                {
                    // We will continue to be in a choice until DoChoice gets kicked off.
                    DoLineEvents(target, dialog.Lines[_activeLine]);
                    return new(FormatChoice(dialog));
                }

                bool play = true;
                if (dialog.Chance < 1)
                {
                    play = RandomExtensions.TryWithChanceOf(Game.Random, dialog.Chance);
                }

                if (play)
                {
                    if (_activeLine < dialog.Lines.Length)
                    {
                        Line line = dialog.Lines[_activeLine];
                        _activeLine++;

                        TrackInteracted();
                        DoLineEvents(target, line);

                        return new(FormatLine(line));
                    }

                    // These dialog lines are *done*, so move on to the next thing.

                    // First, do all the actions for this dialog.
                    DoActionsForActiveDialog(world, target);
                }
                
                if (!TryMatchNextDialog(world, track: true, target))
                {
                    return null;
                }

                dialog = ActiveSituation.Dialogs[_currentDialog.Value];
            }
        }

        /// <summary>
        /// Track the character blackboard that an interaction has occurred.
        /// </summary>
        private void TrackInteracted()
        {
            BlackboardTracker tracker = Game.Data.ActiveSaveData.BlackboardTracker;

            // We also have our built-in character support for amount of times we interacted with a speaker.
            tracker.SetInt(BaseCharacterBlackboard.Name, nameof(BaseCharacterBlackboard.TotalInteractions),
                BlackboardActionKind.Add, 1, Guid);
        }

        /// <summary>
        /// This looks for the next dialog most eligible to be triggered.
        /// </summary>
        [MemberNotNullWhen(true, nameof(_currentDialog))]
        private bool TryMatchNextDialog(World world, bool track, Entity? target = null)
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
                int count = tracker.PlayCount(Guid, _currentSituation, d);

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

            if (track)
            {
                // Keep track of the counter!
                tracker.Track(Guid, _currentSituation, _currentDialog.Value);
            }

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
                int count = tracker.PlayCount(Guid, _currentSituation, dialogIndex);
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

                if (tracker.Matches(criterion.Criterion, Guid, world, targetEntityId, out int weight))
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

        public void DoChoice(int choice, World world, Entity? target = null)
        {
            Debug.Assert(_currentDialog is not null);

            ImmutableArray<int> choices = ActiveSituation.Edges[_currentDialog.Value].Dialogs;

            Line choiceLine = ActiveSituation.Dialogs[choices[choice]].Lines[0];
            DoLineEvents(target, choiceLine);

            // Go to the choice made by the player.
            _currentDialog = choices[choice];

            // And choose whatever's next from there.
            if (!TryMatchNextDialog(world, track: true, target))
            {
                _currentDialog = null;
            }
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

                        if (action.ComponentValue is IInteractiveComponent interactive)
                        {
                            // If this is an interaction, immediately trigger it instead of adding it to the entity.
                            // (since an entity can only have one interaction)
                            interactive.Interact(world, actionEntity, interacted: actionEntity);
                            continue;
                        }
                    }

                    DoAction(actionEntity, tracker, action);
                }

                // If an entity has been created, trigger it immediately.
                actionEntity?.SendMessage<InteractMessage>();

                return true;
            }

            return false;
        }

        private bool DoLineEvents(Entity? target, Line line)
        {
            if (string.IsNullOrEmpty(line.Event))
            {
                // No event to play, whatever.
                return false;
            }

            Guid speakerGuid = line.Speaker ?? _character.Speaker;

            SpeakerAsset? speaker = Game.Data.TryGetAsset<SpeakerAsset>(speakerGuid);
            if (speaker is null)
            {
                GameLogger.Error("Unable to find speaker for playing event!");
                return false;
            }

            if (speaker.Events?.TryAsset is not SpeakerEventsAsset speakerEvents)
            {
                return false;
            }

            if (!speakerEvents.Events.TryGetValue(line.Event, out SpriteEventInfo info) || info.Sound is null)
            {
                return false;
            }

            SoundServices.Play(info.Sound.Value, target: target);

            return true;
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

            Fact fact = action.Fact;
            switch (fact.Kind)
            {
                case FactKind.Bool:
                    tracker.SetBool(fact.Blackboard, fact.Name, action.Kind, action.BoolValue!.Value);
                    break;

                case FactKind.Int:
                    tracker.SetInt(fact.Blackboard, fact.Name, action.Kind, action.IntValue!.Value);
                    break;

                case FactKind.Float:
                    tracker.SetFloat(fact.Blackboard, fact.Name, action.Kind, action.FloatValue!.Value);
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

        private ChoiceLine FormatChoice(Dialog dialog)
        {
            Debug.Assert(dialog.IsChoice);

            string FormatText(string? input)
            {
                if (input is null) return string.Empty;

                _ = BlackboardHelpers.FormatText(input, out string result);
                return result;
            }

            Line titleLine = dialog.Lines[0];
            DialogEdge choices = ActiveSituation.Edges[dialog.Id];

            (Guid speaker, string? portrait) = (titleLine.Speaker ?? _character.Speaker, titleLine.Portrait);
            string title = FormatText(LocalizationServices.TryGetLocalizedString(titleLine.Text));

            var choicesArray = ImmutableArray.CreateBuilder<string>();
            foreach (int c in choices.Dialogs)
            {
                Dialog choice = ActiveSituation.Dialogs[c];
                if (choice.Lines.Length == 0)
                {
                    GameLogger.Error("Empty choice on dialog?");
                    continue;
                }

                choicesArray.Add(FormatText(LocalizationServices.TryGetLocalizedString(choice.Lines[0].Text)));
            }

            return new(speaker, portrait, title, choicesArray.ToImmutable());
        }

        private Line FormatLine(Line line)
        {
            LocalizedString? text = line.Text;
            if (text is null)
            {
                return line;
            }

            if (line.Speaker is null || line.Speaker == Guid.Empty)
            {
                line = line.WithSpeakerAndPortrait(_character.Speaker, _character.Portrait);
            }

            if (!BlackboardHelpers.FormatText(LocalizationServices.GetLocalizedString(text.Value), out string result))
            {
                return line;
            }

            return line.WithText(new(result));
        }
    }
}