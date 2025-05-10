using ImGuiNET;
using Murder.Assets;
using Murder.Assets.Localization;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Editor.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Services;
using Murder.Editor.Utilities;
using Murder.Editor.Utilities.Serialization;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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

        private string? _selectedMode = null;
        private volatile bool _currentlyPruningData = false;

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

            if (ImGui.BeginTabBar("EditorBar"))
            {
                if (ImGui.BeginTabItem("Resources"))
                {
                    DrawResources();
                    ImGui.EndTabItem();
                }

                if (!isDefaultResource)
                {
                    if (ImGui.BeginTabItem("Exporter"))
                    {
                        DrawAssetPicker();
                        ImGui.EndTabItem();
                    }
                }
            }
        }

        public void DrawResources()
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

                    if (!_currentlyPruningData)
                    {
                        _currentlyPruningData = true;

                        Task.Run(() =>
                        {
                            EditorLocalizationServices.PrunNonReferencedStrings(
                                _localization, name: FilterLocalizationAsset.DefaultFilterName, log: true);

                            _currentlyPruningData = false;
                        });
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
                if (deleteButton && !localizedStringData.IsGenerated)
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

        private string _modeBuff = string.Empty;

        public void DrawAssetPicker()
        {
            GameLogger.Verify(_localization is not null);

            FilterLocalizationAsset f = EditorLocalizationServices.GetFilterLocalizationAsset();

            bool modified = SelectOrCreateFilterMode(f);
            modified |= DrawFilteredAssets(_selectedMode);

            if (modified)
            {
                _localization?.TrackAssetOnSave(f.Guid);
            }
        }

        [MemberNotNull(nameof(_selectedMode))]
        private bool SelectOrCreateFilterMode(FilterLocalizationAsset filter)
        {
            GameLogger.Verify(_localization is not null);

            bool modifiedAsset = false;

            _selectedMode ??= FilterLocalizationAsset.DefaultFilterName;

            int index = 0;
            string[] items = filter.Filters.Keys.Order().ToArray();
            for (int i = 0; i < items.Length; ++i)
            {
                if (items[i].Equals(_selectedMode))
                {
                    index = i;
                    break;
                }
            }

            ImGui.PushItemWidth(150);
            bool modified = ImGui.Combo("##select_filter_mode", ref index, items, items.Length);
            if (modified)
            {
                _selectedMode = items[index];
            }
            ImGui.PopItemWidth();

            ImGui.SameLine();

            if (ImGuiHelpers.BlueIcon('\uf303', $"add_mode"))
            {
                ImGui.OpenPopup($"add_mode");
            }

            if (ImGui.BeginPopup("add_mode"))
            {
                ImGui.SetNextItemWidth(200);
                ImGui.InputText($"##add_mode_input", ref _modeBuff, 200);

                bool submit = false;
                if (string.IsNullOrEmpty(_modeBuff))
                {
                    ImGuiHelpers.SelectedButton("Add");
                }
                else
                {
                    submit |= ImGui.Button("Add");
                }

                if (submit)
                {
                    modifiedAsset = true;

                    _ = filter.GetOrCreate(_modeBuff);
                    _modeBuff = string.Empty;

                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }

            ImGui.SameLine();

            if (!_selectedMode.Equals(FilterLocalizationAsset.DefaultFilterName))
            {
                if (ImGuiHelpers.IconButton('\uf2ed', "delete_mode"))
                {
                    modifiedAsset = true;

                    filter.Remove(_selectedMode);
                    _selectedMode = FilterLocalizationAsset.DefaultFilterName;
                }
            }
            else
            {
                ImGuiHelpers.SelectedIconButton('\uf2ed');
            }

            ImGuiHelpers.HelpTooltip("Remove mode");
            ImGui.SameLine();

            if (ImGuiHelpers.IconButton('\uf021', $"clear_{_localization.Guid}"))
            {
                modifiedAsset = true;
                filter.Clear();
            }

            ImGuiHelpers.HelpTooltip("Clear and get all assets");
            ImGui.SameLine();

            if (ImGuiHelpers.IconButton('\uf56e', $"export_{_localization.Guid}"))
            {
                LocalizationExporter.ExportToCsv(_localization, name: _selectedMode);
            }

            ImGuiHelpers.HelpTooltip("Export to .csv");

            return modifiedAsset;
        }

        private bool DrawFilteredAssets(string name)
        {
            ImmutableArray<(string, AssetInfoPropertiesForEditor)> resources =
                EditorLocalizationServices.GetLocalizationCandidates(name);

            bool modified = false;

            using TableMultipleColumns table = new($"localization_filter",
                flags: ImGuiTableFlags.SizingFixedSame,
                (20, ImGuiTableColumnFlags.WidthFixed), (-1, ImGuiTableColumnFlags.WidthStretch));

            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            foreach ((string label, AssetInfoPropertiesForEditor info) in resources)
            {
                bool selected = info.IsSelected;
                if (ImGui.Checkbox(label: $"##{label}", ref selected))
                {
                    info.IsSelected = selected;
                    modified = true;
                }

                ImGui.TableNextColumn();

                if (TreeGroupNode(label, Game.Profile.Theme.White, icon: '\uf500', flags: ImGuiTreeNodeFlags.None))
                {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TableNextColumn();

                    using TableMultipleColumns innerTable = new($"subassets_filter",
                        flags: ImGuiTableFlags.SizingFixedSame,
                        (20, ImGuiTableColumnFlags.WidthFixed), (-1, ImGuiTableColumnFlags.WidthStretch));

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();

                    for (int i = 0; i < info.Assets.Length; ++i)
                    {
                        AssetPropertiesForEditor assetInfo = info.Assets[i];

                        GameAsset? asset = Game.Data.TryGetAsset(assetInfo.Guid);
                        if (asset is null)
                        {
                            continue;
                        }

                        bool selectedAsset = assetInfo.Show;
                        if (ImGui.Checkbox(label: $"##{assetInfo.Guid}", ref selectedAsset))
                        {
                            assetInfo.Show = selectedAsset;
                            modified = true;
                        }

                        ImGui.TableNextColumn();
                        ImGui.Text($"{asset.Name}");

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                    }

                    ImGui.TreePop();
                }

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
            }

            return modified;
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
            EditorLocalizationServices.DrawNotesPopup(_localization, g, localizedStringData);
        }

        private bool TreeGroupNode(string name, System.Numerics.Vector4 textColor, char icon = '\uf49e', ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.None)
        {
            return ImGuiHelpers.TreeNodeWithIconAndColor(
                icon: icon,
                label: name,
                flags: ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.FramePadding | flags,
                text: textColor,
                background: Game.Profile.Theme.BgFaded,
                active: Game.Profile.Theme.Bg);
        }
    }
}
