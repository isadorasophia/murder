using ImGuiNET;
using System.Diagnostics;
using Murder.Assets.Graphics;
using Murder.Core.Dialogs;
using Murder.Assets;
using Murder.Editor.Utilities;
using Murder.Core;
using System.Collections.Immutable;

namespace Murder.Editor.CustomEditors
{
    public partial class CharacterEditor : CustomEditor
    {
        /// <summary>
        /// Show all the nodes within a dialog.
        /// </summary>
        /// <returns>Whether the nodes have been modified.</returns>
        private bool ShowDialogs(ref Situation situation)
        {
            Debug.Assert(_script != null);

            bool modified = false;
            return modified;
        }
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
            if (!line.IsText || line.Speaker is null)
            {
                return false;
            }

            Guid speakerGuid = line.Speaker.Value;

            if (Game.Data.TryGetAsset<SpeakerAsset>(speakerGuid) is not SpeakerAsset speaker ||
                line.Portrait is null ||
                !speaker.Portraits.TryGetValue(line.Portrait, out Portrait portrait) ||
                Game.Data.TryGetAsset<AsepriteAsset>(portrait.Aseprite) is not AsepriteAsset aseprite)
            {
                return false;
            }

            EditorAssetHelpers.DrawPreview(aseprite, maxSize: 100, portrait.AnimationId);
            return true;
        }
    }
}
