using Bang;
using Bang.Components;
using Bang.Entities;
using Murder.Components;
using Murder.Save;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

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
        private int _currentSituation;
        
        /// <summary>
        /// This is the dialog within the situation that is currently active.
        /// This is null if it hasn't started yet.
        /// Not persisted.
        /// </summary>
        private Dialog? _currentDialog = null;

        /// <summary>
        /// Active line in the current dialog.
        /// Not persisted.
        /// </summary>
        private int _activeLine = 0;

        public Character(Guid guid, ImmutableArray<Situation> situations, int initial)
        {
            Situations = situations.ToDictionary(s => s.Id, s => s).ToImmutableDictionary();

            if (!Situations.ContainsKey(initial))
            {
                throw new InvalidOperationException("Invalid initial situation id!");
            }

            _currentSituation = initial;
            _guid = guid;
        }

        public Line? NextLine(World world, Entity? target = null)
        {
            if (_currentDialog is null && !TryMatchBestDialog(world, target))
            {
                return default;
            }

            // We have to make sure that we return the result of the dialog state before performing actions.
            if (_activeLine >= _currentDialog.Value.Lines.Length)
            {
                // First, do all the actions for this dialog.
                DoActionsForActiveDialog(world, target);
                    
                if (_currentDialog.Value.GoTo is int @goto)
                {
                    _currentSituation = @goto;
                    _ = TryMatchBestDialog(world, target);
                }
                else
                {
                    _currentDialog = null;
                }
            }

            if (_currentDialog is Dialog dialog && _activeLine < dialog.Lines.Length)
            {
                return dialog.Lines[_activeLine++];
            }
            else
            {
                // We might have an empty dialog with actions. For such scenarios, immediately fire the actions.
                DoActionsForActiveDialog(world, target);
            }
            
            return default;
        }

        private bool DoActionsForActiveDialog(World world, Entity? target)
        {
            // First, do all the actions for this dialog.
            if (_currentDialog != null && _currentDialog.Value.Actions is ImmutableArray<DialogAction> actions)
            {
                BlackboardTracker tracker = Game.Data.ActiveSaveData.BlackboardTracker;
                foreach (DialogAction action in actions)
                {
                    DoAction(world, target, tracker, action);
                }

                return true;
            }

            return false;
        }

        [MemberNotNullWhen(true, nameof(_currentDialog))]
        private bool TryMatchBestDialog(World world, Entity? target = null)
        {
            _activeLine = 0;
            _currentDialog = null;

            BlackboardTracker tracker = Game.Data.ActiveSaveData.BlackboardTracker;
            
            // First, rank the dialog options according to criteria matching.
            List<(int DialogIndex, float Score)> dialogScore = new();

            ImmutableArray<Dialog> dialogs = Situations[_currentSituation].Dialogs;
            for (int d = 0; d < dialogs.Length; d++)
            {
                Dialog dialog = dialogs[d];

                int count = tracker.PlayCount(_guid, _currentSituation, d);

                // Skip dialogs that should only be played once.
                if (dialog.PlayOnce && count > 0)
                {
                    continue;
                }

                bool valid = true;

                // We want to prioritize dialogs with the same score that haven't been played before.
                // So we subtract the total number of plays.
                float score = 1 - count / 100f;
                
                foreach (Criterion criterion in dialog.Requirements)
                {
                    if (tracker.Matches(criterion, _guid, world, target?.TryGetIdTarget()?.Target, out int weight))
                    {
                        score += weight;
                    }
                    else
                    {
                        // This means this is a NO match. Give up immediately!
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    dialogScore.Add((d, score));
                }
            }

            if (dialogScore.Count == 0)
            {
                // No valid dialogs were found.
                return false;
            }

            int bestMatchIndex = dialogScore.OrderByDescending(d => d.Score).First().DialogIndex;
            _currentDialog = dialogs[bestMatchIndex];

            // Keep track of the counter!
            tracker.Track(_guid, _currentSituation, bestMatchIndex);

            // We also have our built-in character support for amount of times we interacted with a speaker.
            tracker.SetInt(BaseCharacterBlackboard.Name, nameof(BaseCharacterBlackboard.TotalInteractions),
                BlackboardActionKind.Add, 1, _guid);

            return true;
        }

        private void DoAction(World world, Entity? target, BlackboardTracker tracker, DialogAction action)
        {
            if (action.ComponentsValue is ImmutableArray<IComponent> components)
            {
                IComponent[] componentsToAdd = new IComponent[components.Length];
                for (int i = 0; i < components.Length; ++i)
                {
                    IComponent c = components[i];
                    
                    // We need to guarantee that any modifiable components added here are safe.
                    c = c is IModifiableComponent ? SerializationHelper.DeepCopy(c) : c;
                    componentsToAdd[i] = c;
                }
                
                Entity result = world.AddEntity(componentsToAdd);

                // Now, we should also propagate any targets that the entity hold.
                if (target?.TryGetIdTarget() is IdTargetComponent idTarget)
                {
                    result.SetIdTarget(idTarget);
                }
                
                if (target?.TryGetIdTargetCollection() is IdTargetCollectionComponent idTargetCollection)
                {
                    result.SetIdTargetCollection(idTargetCollection);
                }

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
    }
}
