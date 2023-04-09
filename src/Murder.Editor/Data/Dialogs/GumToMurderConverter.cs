using Bang.Components;
using Murder.Assets;
using Murder.Core.Dialogs;
using Murder.Diagnostics;
using Murder.Editor.Utilities;
using System.Collections.Immutable;

using GumData = Gum.InnerThoughts;

namespace Murder.Editor.Data
{
    internal class GumToMurderConverter
    {
        /// <summary>
        /// Used for diagnostics.
        /// </summary>
        private string? _lastScriptFetched = null;
        
        /// <summary>
        /// Cached speakers when fetching for valid values.
        /// </summary>
        private ImmutableDictionary<string, Guid>? _speakers = null;

        private HashSet<DialogActionId> _matchedComponents = new();

        private ImmutableDictionary<DialogActionId, IComponent> _components = 
            ImmutableDictionary<DialogActionId, IComponent>.Empty;

        private Guid _speakerOwner = Guid.Empty;

        public void Reset()
        {
            _lastScriptFetched = null;
            _speakers = null;
        }

        public void ReloadDialogWith(GumData.CharacterScript script, CharacterAsset asset)
        {
            _lastScriptFetched = script.Name;

            _components = asset.Components;
            _speakerOwner = asset.Owner;

            _matchedComponents = new();

            SortedList<int, Situation> situations = new();
            foreach (GumData.Situation gumSituation in script.FetchAllSituations())
            {
                Situation situation = ConvertSituation(gumSituation);
                situations.Add(situation.Id, situation);
            }

            asset.SetSituations(situations);

            // Remove all the components that have not been used in the latest sync.
            asset.RemoveCustomComponents(_components.Keys.Where(t => !_matchedComponents.Contains(t)));
        }

        private Situation ConvertSituation(GumData.Situation gumSituation)
        {
            // TODO: Would it be okay if the index of each dialog is not the id?
            // I am not sure if this ever happen.
            var dialogsBuilder = ImmutableArray.CreateBuilder<Dialog>();
            foreach (GumData.Block gumBlock in gumSituation.Blocks)
            {
                dialogsBuilder.Add(ConvertBlockToDialog(gumSituation.Id, gumBlock));
            }

            var edges = ImmutableDictionary.CreateBuilder<int, DialogEdge>();
            foreach ((int from, GumData.Edge gumEdge) in gumSituation.Edges)
            {
                DialogEdge edge = ConvertEdge(gumEdge);
                edges.Add(from, edge);
            }

            return new(gumSituation.Id, gumSituation.Name, dialogsBuilder.ToImmutable(), edges.ToImmutable());
        }

        #region ⭐ Edge ⭐

        private DialogEdge ConvertEdge(GumData.Edge edge)
        {
            MatchKind kind = ToMatchKind(edge.Kind);
            return new(kind, edge.Blocks.ToImmutableArray());
        }
        
        private MatchKind ToMatchKind(GumData.EdgeKind kind) => (MatchKind)kind;

        #endregion

        #region ⭐ Dialog ⭐

        private Dialog ConvertBlockToDialog(int situation, GumData.Block block)
        {
            ImmutableArray<CriterionNode>.Builder requirementsBuilder = ImmutableArray.CreateBuilder<CriterionNode>();
            foreach (GumData.CriterionNode gumCriterion in block.Requirements)
            {
                if (ConvertCriterionNode(gumCriterion) is not CriterionNode node)
                {
                    continue;
                }

                requirementsBuilder.Add(node);
            }

            ImmutableArray<Line>.Builder lineBuilder = ImmutableArray.CreateBuilder<Line>();
            foreach (GumData.Line gumLine in block.Lines)
            {
                if (ConvertLine(gumLine) is not Line line)
                {
                    continue;
                }

                lineBuilder.Add(line);
            }

            ImmutableArray<DialogAction>? actions;
            if (block.Actions is null)
            {
                actions = null;
            }
            else
            {
                ImmutableArray<DialogAction>.Builder actionBuilder = ImmutableArray.CreateBuilder<DialogAction>();
                for (int i = 0; i < block.Actions.Count; ++i)
                {
                    GumData.DialogAction gumAction = block.Actions[i];
                    if (ConvertDialogAction(situation, block.Id, i, gumAction) is not DialogAction action)
                    {
                        continue;
                    }

                    actionBuilder.Add(action);
                }
                
                actions = actionBuilder.Count > 0 ? actionBuilder.ToImmutable() : null;
            }
            
            return new(block.Id, block.PlayUntil, requirementsBuilder.ToImmutable(), lineBuilder.ToImmutable(), actions, block.GoTo);
        }

        #endregion

        #region ⭐ Criteria ⭐

        private CriterionNode? ConvertCriterionNode(GumData.CriterionNode gumNode)
        {
            GumData.Criterion gumCriterion = gumNode.Criterion;
            
            Fact? fact = ConvertFact(gumCriterion.Fact);
            if (fact is null)
            {
                return null;
            }
            
            CriterionKind kind = ToCriterionKind(gumCriterion.Kind);
            CriterionNodeKind nodeKind = ToCriterionNodeKind(gumNode.Kind);

            Criterion criterion = new(fact.Value, kind, gumCriterion.BoolValue, gumCriterion.IntValue, gumCriterion.StrValue);
            CriterionNode node = new(criterion, nodeKind);

            return node;
        }

        private Fact? ConvertFact(GumData.Fact gumFact)
        {
            Type? componentType = null;
            if (gumFact.ComponentType is string componentName)
            {
                componentType = AssetsFilter.FindComponentWithName(componentName);
                if (componentType is null)
                {
                    GameLogger.Error($"Unable to find a component of name {componentName} on script {_lastScriptFetched}.");
                    return null;
                }
            }

            return new Fact(
                gumFact.Blackboard,
                gumFact.Name,
                ToFactKind(gumFact.Kind),
                componentType);
        }
        
        private FactKind ToFactKind(GumData.FactKind kind) => (FactKind)kind;

        private CriterionKind ToCriterionKind(GumData.CriterionKind kind) => (CriterionKind)kind;

        private CriterionNodeKind ToCriterionNodeKind(GumData.CriterionNodeKind kind) => (CriterionNodeKind)kind;

        #endregion

        #region ⭐ Line ⭐
        
        public Line? ConvertLine(GumData.Line gumLine)
        {
            Guid? speaker = null;
            
            string? gumSpeaker = gumLine.Speaker;
            if (gumSpeaker is string)
            {
                if (gumSpeaker == GumData.Line.OWNER)
                {
                    speaker = _speakerOwner;
                }
                else
                {
                    speaker = FindSpeaker(gumSpeaker);
                    if (speaker is null)
                    {
                        GameLogger.Error($"Unable to find a speaker of name {gumSpeaker} on script {_lastScriptFetched}.");
                        return null;
                    }
                }
            }
            
            string? portrait = gumLine.Portrait;
            if (speaker is not null && portrait is not null)
            {
                SpeakerAsset speakerAsset = Architect.EditorData.GetAsset<SpeakerAsset>(speaker.Value);
                if (!speakerAsset.Portraits.ContainsKey(portrait))
                {
                    portrait = Prettify.CapitalizeFirstLetter(portrait);
                    if (!speakerAsset.Portraits.ContainsKey(portrait))
                    {
                        GameLogger.Warning($"Unable to find a portrait of {portrait} on script {_lastScriptFetched}. This will fallback to the default one.");
                        portrait = null;
                    }
                }

            }

            return new(speaker, portrait, gumLine.Text, gumLine.Delay);
        }

        private Guid? FindSpeaker(string name)
        {
            _speakers ??= Architect.EditorData.FindAllNamesForAssetWithGuid(typeof(SpeakerAsset));

            if (_speakers.TryGetValue(name, out Guid guid))
            {
                return guid;
            }

            return null;
        }

        #endregion

        #region ⭐ Action ⭐

        private DialogAction? ConvertDialogAction(int situation, int dialog, int actionIndex, GumData.DialogAction gumAction)
        {
            IComponent? c = null;
            
            Fact? fact = ConvertFact(gumAction.Fact);
            if (fact is null)
            {
                return null;
            }
            else if (fact.Value.ComponentType is Type t)
            {
                DialogActionId id = new(situation, dialog, actionIndex);
                if (!_components.TryGetValue(id, out c))
                {
                    c = Activator.CreateInstance(t) as IComponent;
                }

                if (c is null)
                {
                    GameLogger.Error($"Unable to create a component of name {t.Name} on script {_lastScriptFetched}.");
                    return null;
                }

                _matchedComponents.Add(id);
            }
            
            return new DialogAction(
                actionIndex, fact, ToBlackboardActionKind(gumAction.Kind), 
                gumAction.StrValue, gumAction.IntValue, gumAction.BoolValue, c);
        }

        private BlackboardActionKind ToBlackboardActionKind(Gum.Blackboards.BlackboardActionKind kind) => (BlackboardActionKind)kind;
        
        #endregion
    }
}
