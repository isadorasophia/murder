using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Core;
using Murder.Core.Dialogs;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Stages;
using Murder.Editor.Utilities;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.CustomEditors
{
    public partial class CharacterEditor : CustomEditor
    {
        protected record ScriptInformation(Stage Stage)
        {
            /// <summary>
            /// Situation currently selected.
            /// </summary>
            public int ActiveSituation = 0;

            /// <summary>
            /// This is the entity id in the world.
            /// </summary>
            public int HelperId = 0;

            /// <summary>
            /// Cache the dialog selected for each situation.
            /// </summary>
            public Dictionary<int, int> CachedDialogs = new();

            public int ActiveDialog => CachedDialogs.TryGetValue(ActiveSituation, out int dialogId) ?
                dialogId : -1;
        }

        protected static readonly Lazy<EditorMember?> MemberForPortrait = new(() =>
            typeof(CharacterAsset).TryGetFieldForEditor(nameof(CharacterAsset.Portrait)));

        protected static readonly Lazy<EditorMember?> MemberForNotes = new(() =>
            typeof(CharacterAsset).TryGetFieldForEditor(nameof(CharacterAsset.LocalizationNotes)));

        protected static readonly Lazy<ImmutableArray<(string, EditorMember)>> MembersForCharacter = new(() =>
        {
            Dictionary<string, EditorMember> members =
                typeof(CharacterAsset).GetFieldsForEditor().ToDictionary(f => f.Name, f => f);

            return [(nameof(CharacterAsset.Owner), members[nameof(CharacterAsset.Owner)])];
        });

        private bool PrettySelectableWithIcon(string name, bool selectable, bool disabled) =>
            ImGuiHelpers.PrettySelectableWithIcon(
                label: name,
                selectable,
                disabled);

        public static ImmutableArray<(string, int)> FetchAllSituations(CharacterAsset asset)
        {
            var builder = ImmutableArray.CreateBuilder<(string, int)>();

            for (int i = 0; i < asset.Situations.Length; ++i)
            {
                builder.Add((asset.Situations[i].Name, asset.Situations[i].Id));
            }

            return builder.ToImmutable();
        }

        private bool TryDrawPortrait(Line line)
        {
            Debug.Assert(_script is not null);

            if (!line.IsText)
            {
                return false;
            }

            Guid speakerGuid = (line.Speaker is null || line.Speaker == Guid.Empty) ?
                _script.Owner : line.Speaker.Value;

            if (Game.Data.TryGetAsset<SpeakerAsset>(speakerGuid) is not SpeakerAsset speaker || speaker.Portraits.Count == 0)
            {
                return false;
            }

            string portraitName = line.Portrait is null ? _script.Portrait ?? speaker.DefaultPortrait ?? speaker.Portraits.Keys.First() : line.Portrait;

            if (!speaker.Portraits.TryGetValue(portraitName, out Portrait portrait) ||
                Game.Data.TryGetAsset<SpriteAsset>(portrait.Sprite) is not SpriteAsset aseprite)
            {
                return false;
            }

            EditorAssetHelpers.DrawPreview(aseprite, maxSize: 100, portrait.AnimationId);
            return true;
        }

        private bool FetchActiveSituation([NotNullWhen(true)] out Situation? situation)
        {
            situation = null;

            if (_script is null)
            {
                return false;
            }

            situation = _script?.TryFetchSituation(ActiveEditors[_script.Guid].ActiveSituation);
            return situation is not null;
        }

        internal string GetTargetStringValueForAction(DialogAction action)
        {
            switch (action.Fact.Kind)
            {
                case FactKind.Int:
                case FactKind.Weight:
                    return action.IntValue!.Value.ToString();

                case FactKind.Float:
                    return action.FloatValue!.Value.ToString();

                case FactKind.String:
                    return action.StrValue!;

                case FactKind.Bool:
                default:
                    return action.BoolValue!.Value.ToString().ToLower();
            }
        }
    }
}