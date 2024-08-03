using ImGuiNET;
using Murder.Assets.Localization;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using Murder.Editor.Utilities.Serialization;
using System.Numerics;
using static Murder.Assets.Localization.LocalizationAsset;

namespace Murder.Editor.CustomEditors
{
    [CustomEditorOf(typeof(LocalizationAsset))]
    public class LocalizationAssetEditor : CustomEditor
    {
        public override object Target => _localization!;

        private LocalizationAsset? _localization;

        private Guid? _referenceResource = null;

        public override void OpenEditor(ImGuiRenderer imGuiRenderer, RenderContext renderContext, object target, bool overwrite)
        {
            _localization = target as LocalizationAsset;
            _referenceResource = null;

            if (Game.Profile.LocalizationResources.TryGetValue(LanguageId.English, out Guid englishResource))
            {
                _referenceResource = englishResource;
            }
        }

        protected static readonly Lazy<EditorMember?> MemberForResources = new(() =>
            typeof(LocalizationAsset).TryGetFieldForEditor(nameof(LocalizationAsset.Resources)));

        public override void DrawEditor()
        {
            GameLogger.Verify(_localization is not null);

            bool isDefaultResource = _referenceResource == _localization.Guid;
            LocalizationAsset? asset = _referenceResource is not null ?
                Game.Data.TryGetAsset<LocalizationAsset>(_referenceResource.Value) : null;

            bool fixButtonSelected = ImGuiHelpers.IconButton('\uf0f1', $"fix_{_localization.Guid}");

            if (fixButtonSelected)
            {
                if (!isDefaultResource && _referenceResource is not null)
                {
                    if (asset is not null)
                    {
                        foreach (LocalizedStringData data in _localization.Resources)
                        {
                            // Localized data is not here, so let's add it.
                            if (!asset.HasResource(data.Guid))
                            {
                                _localization.RemoveResource(data.Guid, force: true);
                                _localization.FileChanged = true;
                            }
                        }
                    }

                    AddMissingResourcesFromAsset(asset);
                }
                else if (isDefaultResource)
                {
                    // Remove all resources that are no longer used.
                    foreach (ResourceDataForAsset data in _localization.DialogueResources)
                    {
                        if (Game.Data.TryGetAsset(data.DialogueResourceGuid) is null)
                        {
                            _localization.RemoveResourceForDialogue(data.DialogueResourceGuid);
                            _localization.FileChanged = true;
                        }
                    }
                }
            }
            
            ImGuiHelpers.HelpTooltip(!isDefaultResource ? 
                "Fix references from the default resource" : "Fix strings not referenced");

            ImGui.SameLine();
            
            bool importButton = isDefaultResource ?
                ImGuiHelpers.SelectedIconButton('\uf56f') :
                ImGuiHelpers.IconButton('\uf56f', $"import_{_localization.Guid}");

            if (importButton && !isDefaultResource)
            {
                LocalizationExporter.ImportFromCsv(_localization);
            }

            ImGuiHelpers.HelpTooltip("Import from .csv");
            ImGui.SameLine();

            bool exportButton = isDefaultResource ?
                ImGuiHelpers.SelectedIconButton('\uf56e') :
                ImGuiHelpers.IconButton('\uf56e', $"export_{_localization.Guid}");

            if (exportButton && !isDefaultResource)
            {
                LocalizationExporter.ExportToCsv(_localization);
            }

            ImGuiHelpers.HelpTooltip("Export to .csv");

            if (_referenceResource == _localization.Guid)
            {
                ImGui.SameLine();
                ImGuiHelpers.SelectedButton("\uf005 Default resource");
            }

            // Draw the actual resources.
            foreach (LocalizedStringData localizedStringData in _localization.Resources)
            {
                Guid g = localizedStringData.Guid;

                bool deleteButton = localizedStringData.IsGenerated ?
                    ImGuiHelpers.SelectedIconButton('\uf2ed') :
                    ImGuiHelpers.DeleteButton($"delete_{g}");

                // == Delete button ==
                if (deleteButton && localizedStringData.IsGenerated)
                {
                    _localization.RemoveResource(g, force: true);
                    _localization.FileChanged = true;
                }

                if (localizedStringData.IsGenerated)
                {
                    ImGuiHelpers.HelpTooltip("Generated string");
                }
                else if (_referenceResource == _localization.Guid)
                {
                    string plural = localizedStringData.Counter > 1 ? "s" : "";
                    ImGuiHelpers.HelpTooltip($"Remove resource string ({localizedStringData.Counter ?? 1} reference{plural})");
                }
                else
                {
                    ImGuiHelpers.HelpTooltip("Remove resource string");
                }

                ImGui.SameLine();

                // == Notes ==
                if (ImGuiHelpers.BlueIcon('\uf10d', $"note_{g}"))
                {
                    ImGui.OpenPopup($"notes_{g}");
                }

                ImGuiHelpers.HelpTooltip(string.IsNullOrEmpty(localizedStringData.Notes) ? "(No notes)" : localizedStringData.Notes);
                ImGui.SameLine();

                DrawNotesPopup(g, localizedStringData);

                // == Modified button ==
                bool modified = false;

                if (asset is not null && asset.TryGetResource(g) is LocalizedStringData otherData &&
                    otherData.String != localizedStringData.String)
                {
                    modified = true;
                }

                Vector4? color = modified ? Game.Profile.Theme.RedFaded : null;
                ImGuiHelpers.SelectedButton("\uf303", color);
                ImGui.SameLine();

                ImGui.PushItemWidth(-1);

                // == Text! ==
                string text = localizedStringData.String ?? string.Empty;
                if (ImGui.InputText($"##{g}", ref text, 1024))
                {
                    _localization.SetResource(localizedStringData with { String = text });
                    _localization.FileChanged = true;
                }

                if (modified && asset is not null && asset.TryGetResource(g) is LocalizedStringData originalData)
                {
                    // Show original asset, if applicable.
                    ImGuiHelpers.HelpTooltip(originalData.String);
                }

                ImGui.PopItemWidth();
            }

        }

        /// <summary>
        /// Scan all resources from <paramref name="asset"/> and fill the <see cref="_localization"/> resources
        /// data.
        /// </summary>
        private void AddMissingResourcesFromAsset(LocalizationAsset? asset)
        {
            GameLogger.Verify(_localization is not null);

            if (asset is null)
            {
                return;
            }

            foreach (LocalizedStringData referenceData in asset.Resources)
            {
                LocalizedStringData? data = _localization.TryGetResource(referenceData.Guid);

                // Localized data is not here, so let's add it.
                if (data is null)
                {
                    data = referenceData;
                    _localization.SetResource(referenceData with { Counter = null });
                }

                if (data is not null && !string.IsNullOrEmpty(referenceData.Notes) && 
                    referenceData.Notes != data.Value.Notes)
                {
                    _localization.SetResource(data.Value with { Notes = referenceData.Notes });
                }
            }

            _localization.SetAllDialogueResources(asset.DialogueResources);
        }

        /// <summary>
        /// Draw pop up with notes for this resource string.
        /// </summary>
        private void DrawNotesPopup(Guid g, LocalizedStringData localizedStringData)
        {
            GameLogger.Verify(_localization is not null);

            if (ImGui.BeginPopup($"notes_{g}"))
            {
                string text = localizedStringData.Notes ?? string.Empty;
                if (ImGui.InputText("##notes_name", ref text, 1024, ImGuiInputTextFlags.AutoSelectAll))
                {
                    _localization.SetResource(localizedStringData with { Notes = text });
                }

                if (ImGui.Button("Ok!") || Game.Input.Pressed(MurderInputButtons.Submit))
                {
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }
        }

    }
}
