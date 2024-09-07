using Bang.Components;
using Murder.Assets;
using Murder.Assets.Localization;
using Murder.Core;
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

        private HashSet<DialogueId> _matchedComponents = new();
        private HashSet<DialogueId> _matchedPortraits = new();

        private ImmutableDictionary<DialogueId, LineInfo> _data =
            ImmutableDictionary<DialogueId, LineInfo>.Empty;

        private Guid _speakerOwner = Guid.Empty;

        /// <summary>
        /// These are the localized strings produced during this dialogue.
        /// </summary>
        private ImmutableArray<LocalizedDialogueData>.Builder _localizedStrings = ImmutableArray.CreateBuilder<LocalizedDialogueData>();

        /// <summary>
        /// These are all the previous localized strings that existed in the .gum file.
        /// </summary>
        private Dictionary<string, LocalizedString> _previousStringsInScript = new();

        public void Reset()
        {
            _lastScriptFetched = null;
            _speakers = null;
        }

        public void ReloadDialogWith(GumData.CharacterScript script, CharacterAsset asset)
        {
            _lastScriptFetched = script.Name;

            _data = asset.Data;
            _speakerOwner = asset.Owner;

            _matchedComponents = new();
            _matchedPortraits = new();

            _localizedStrings = ImmutableArray.CreateBuilder<LocalizedDialogueData>();

            LocalizationAsset localizationAsset = Game.Data.GetDefaultLocalization();
            _previousStringsInScript = FetchResourcesForAsset(localizationAsset, asset.Guid);

            var builder = ImmutableDictionary.CreateBuilder<string, Situation>();
            foreach (GumData.Situation gumSituation in script.FetchAllSituations())
            {
                Situation situation = ConvertSituation(gumSituation);
                builder[situation.Name] = situation;
            }

            asset.SetSituations(builder.ToImmutable());

            // Remove all the components that have not been used in the latest sync.
            asset.PrunUnusedComponents(_data.Keys.Where(t => !_matchedComponents.Contains(t)));
            asset.PrunUnusedData();

            localizationAsset.SetResourcesForDialogue(asset.Guid, _localizedStrings.ToImmutable());
            Architect.EditorData.SaveAsset(localizationAsset);
        }

        private Dictionary<string, LocalizedString> FetchResourcesForAsset(LocalizationAsset localizationAsset, Guid characterGuid)
        {
            ImmutableArray<LocalizedDialogueData> resources = localizationAsset.FetchResourcesForDialogue(characterGuid);

            Dictionary<string, LocalizedString> result = new();
            foreach (LocalizedDialogueData resource in resources)
            {
                LocalizedStringData? data = localizationAsset.TryGetResource(resource.Guid);
                if (data is null)
                {
                    continue;
                }

                result[data.Value.String] = new(data.Value.Guid);
            }

            return result;
        }

        private LocalizedString? TryGetLocalizedString(Guid speaker, string? text)
        {
            if (text is null)
            {
                return null;
            }

            if (!_previousStringsInScript.TryGetValue(text, out LocalizedString data))
            {
                LocalizationAsset localizationAsset = Game.Data.GetDefaultLocalization();
                data = localizationAsset.AddResource(text, isGenerated: true);
            }

            _localizedStrings.Add(new(speaker, data.Id));
            return data;
        }

        private Situation ConvertSituation(GumData.Situation gumSituation)
        {
            // TODO: Would it be okay if the index of each dialog is not the id?
            // I am not sure if this ever happen.
            var dialogsBuilder = ImmutableArray.CreateBuilder<Dialog>();
            foreach (GumData.Block gumBlock in gumSituation.Blocks)
            {
                dialogsBuilder.Add(ConvertBlockToDialog(gumSituation.Name, gumBlock));
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

        private Dialog ConvertBlockToDialog(string situation, GumData.Block block)
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
            for (int i = 0; i < block.Lines.Count; ++i)
            {
                GumData.Line gumLine = block.Lines[i];

                if (ConvertLine(situation, block.Id, gumLine, lineIndex: i) is not Line line)
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

            return new(block.Id, block.PlayUntil, block.Chance, requirementsBuilder.ToImmutable(), lineBuilder.ToImmutable(), actions, block.GoTo, block.IsExit, block.IsChoice);
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

            Criterion criterion = new(fact.Value, kind, gumCriterion.BoolValue, gumCriterion.IntValue, gumCriterion.FloatValue, gumCriterion.StrValue);
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

        public Line? ConvertLine(string situation, int dialog, GumData.Line gumLine, int lineIndex)
        {
            Guid? speaker = null;

            DialogueId id = new(situation, dialog, lineIndex);

            string? @event = null;
            if (_data.TryGetValue(id, out LineInfo eventInfo))
            {
                @event = eventInfo.Event;

                if (eventInfo.Speaker != Guid.Empty || eventInfo.Portrait != null)
                {
                    // If this matches, it means that the user previously set the portrait with a custom value.
                    // We override whatever was set in the dialog.
                    _matchedPortraits.Add(id);
                    return new(eventInfo.Speaker, eventInfo.Portrait, TryGetLocalizedString(eventInfo.Speaker, gumLine.Text), gumLine.Delay, @event);
                }
            }

            string? gumSpeaker = gumLine.Speaker;
            if (gumSpeaker is string)
            {
                speaker = FindSpeaker(gumSpeaker);
                if (speaker is null)
                {
                    GameLogger.Error($"Unable to find a speaker of name {gumSpeaker} on script {_lastScriptFetched}.");
                    return null;
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

            return new(speaker, portrait, TryGetLocalizedString(speaker ?? _speakerOwner, gumLine.Text), gumLine.Delay, @event);
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

        private DialogAction? ConvertDialogAction(string situation, int dialog, int actionIndex, GumData.DialogAction gumAction)
        {
            IComponent? c = null;

            Fact? fact = ConvertFact(gumAction.Fact);
            if (fact is null)
            {
                return null;
            }
            else if (fact.Value.ComponentType is Type t)
            {
                DialogueId id = new(situation, dialog, actionIndex);
                if (_data.TryGetValue(id, out LineInfo info) && info.Component != null)
                {
                    c = info.Component;
                }
                else
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
                actionIndex, fact.Value, ToBlackboardActionKind(gumAction.Kind),
                gumAction.StrValue, gumAction.IntValue, gumAction.BoolValue, gumAction.FloatValue, c);
        }

        private BlackboardActionKind ToBlackboardActionKind(Gum.Blackboards.BlackboardActionKind kind) => (BlackboardActionKind)kind;

        #endregion
    }
}