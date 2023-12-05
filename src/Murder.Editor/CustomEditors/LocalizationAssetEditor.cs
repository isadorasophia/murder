﻿using ImGuiNET;
using Murder.Assets;
using Murder.Assets.Localization;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using System;
using System.Collections.Immutable;
using System.Numerics;

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

            LocalizationAsset? asset = _referenceResource is not null ?
                Game.Data.TryGetAsset<LocalizationAsset>(_referenceResource.Value) : null;

            bool fixButtonSelected = _referenceResource is null || _referenceResource == _localization.Guid ?
                ImGuiHelpers.SelectedIconButton('\uf0f1') :
                ImGuiHelpers.IconButton('\uf0f1', $"fix_{_localization.Guid}");

            if (fixButtonSelected && _referenceResource is not null)
            {
                if (asset is not null)
                {
                    foreach (LocalizedStringData data in _localization.Resources)
                    {
                        // Localized data is not here, so let's add it.
                        if (!asset.HasResource(data.Guid))
                        {
                            _localization.RemoveResource(data.Guid, force: true);
                        }
                    }
                }

                AddMissingResourcesFromAsset(asset);
            }

            ImGuiHelpers.HelpTooltip(_referenceResource != _localization.Guid ? 
                "Fix references from the default resource" : "Default resource");

            // Draw the actual resources.
            foreach (LocalizedStringData localizedStringData in _localization.Resources)
            {
                Guid g = localizedStringData.Guid;

                // == Delete button ==
                if (ImGuiHelpers.DeleteButton($"delete_{g}"))
                {
                    _localization.RemoveResource(g, force: true);
                    _localization.FileChanged = true;
                }

                string plural = localizedStringData.Counter > 1 ? "s" : "";
                ImGuiHelpers.HelpTooltip($"Remove resource string ({localizedStringData.Counter ?? 1} reference{plural})");
                ImGui.SameLine();

                // == Notes ==
                if (ImGuiHelpers.BlueIcon('\uf10d', $"note_{g}"))
                {
                    ImGui.OpenPopup($"notes_{g}");
                }

                ImGuiHelpers.HelpTooltip(localizedStringData.Notes ?? "(No notes)");
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
                    _localization.SetResource(referenceData);
                }

                if (data is not null && referenceData.Notes != data.Value.Notes)
                {
                    _localization.SetResource(data.Value with { Notes = referenceData.Notes });
                }
            }
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
                if (ImGui.InputText("##notes_name", ref text, 128, ImGuiInputTextFlags.AutoSelectAll))
                {
                    _localization.SetResource(localizedStringData with { Notes = text });
                }

                ImGui.EndPopup();
            }
        }

    }
}