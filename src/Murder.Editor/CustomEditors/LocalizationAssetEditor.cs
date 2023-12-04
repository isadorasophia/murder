using ImGuiNET;
using Murder.Assets;
using Murder.Assets.Localization;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using System.Collections.Immutable;

namespace Murder.Editor.CustomEditors
{
    [CustomEditorOf(typeof(LocalizationAsset))]
    public class LocalizationAssetEditor : CustomEditor
    {
        public override object Target => _localization!;

        private LocalizationAsset? _localization;

        private Guid _selectedCombo = Guid.Empty;

        public override void OpenEditor(ImGuiRenderer imGuiRenderer, RenderContext renderContext, object target, bool overwrite)
        {
            _localization = target as LocalizationAsset;
            _selectedCombo = Guid.Empty;
        }

        protected static readonly Lazy<EditorMember?> MemberForResources = new(() =>
            typeof(LocalizationAsset).TryGetFieldForEditor(nameof(LocalizationAsset.Resources)));

        public override void DrawEditor()
        {
            GameLogger.Verify(_localization is not null);

            if (ImGuiHelpers.IconButton('\uf56e', $"import_{_localization.Guid}"))
            {

            }

            ImGuiHelpers.HelpTooltip("Import resources strings from another file");

            ImGui.SameLine();

            ImGui.PushItemWidth(200);
            string[] resourcesAvailable = [];
            if (ImGui.BeginCombo($"##{_localization.Guid}", Game.Data.TryGetAsset(_selectedCombo)?.Name ?? "Import from..."))
            {
                foreach ((Guid guid, GameAsset asset) in Game.Data.FilterAllAssets(typeof(LocalizationAsset)))
                {
                    if (guid == _localization.Guid)
                    {
                        continue;
                    }

                    if (ImGui.MenuItem(asset.Name))
                    {
                        _selectedCombo = asset.Guid;
                    }
                }

                ImGui.EndCombo();
            }
            ImGui.PopItemWidth();

            // Draw the actual resources.
            ImmutableDictionary<Guid, LocalizedStringData> resources = _localization.Resources;
            foreach ((Guid g, LocalizedStringData localizedStringData) in resources)
            {
                if (ImGuiHelpers.DeleteButton($"delete_{g}"))
                {
                    _localization.RemoveResource(g);
                    _localization.FileChanged = true;
                }

                ImGuiHelpers.HelpTooltip($"Remove resource string ({localizedStringData.Counter} references)");
                ImGui.SameLine();

                ImGuiHelpers.SelectedButton("\uf303");
                ImGui.SameLine();

                ImGui.PushItemWidth(-1);
                string text = localizedStringData.String;
                if (ImGui.InputText($"##{g}", ref text, 1024))
                {
                    _localization.SetResource(g, localizedStringData with { String = text });
                    _localization.FileChanged = true;
                }
                ImGui.PopItemWidth();
            }

        }
    }
}
