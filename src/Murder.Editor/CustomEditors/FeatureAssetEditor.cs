using ImGuiNET;
using Murder.Assets;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Editor.CustomEditors
{
    [CustomEditorOf(typeof(FeatureAsset))]
    internal class FeatureAssetEditor : CustomEditor
    {
        private FeatureAsset _featureAsset = null!;
        private static string _searchSystemText = "";

        public override object Target => _featureAsset;

        public override void OpenEditor(ImGuiRenderer imGuiRenderer, RenderContext _, object target, bool overwrite)
        {
            _featureAsset = (FeatureAsset)target;
        }

        public override void DrawEditor()
        {
            bool isDiagnostic = _featureAsset.IsDiagnostics;
            if (ImGuiHelpers.ColoredIconButton('\uf188', "#show_diagnostic_feature", isDiagnostic))
            {
                _featureAsset.IsDiagnostics = !isDiagnostic;
                _featureAsset.FileChanged = true;
            }

            ImGui.PushItemWidth(-1);
            ImGui.SameLine();
            ImGui.InputTextWithHint("##search_systems", "Search...", ref _searchSystemText, 256);
            ImGui.PopItemWidth();

            ImGuiHelpers.HelpTooltip("Enable this feature while running on editor.");

            if (DrawSystemsEditor(_featureAsset.SystemsOnly, _featureAsset.FetchAllSystems(true), out var newSystemsList))
            {
                _featureAsset.SetSystems(newSystemsList);
            }

            ImGui.Dummy(new System.Numerics.Vector2(0, 10));

            if (DrawFeaturesEditor(_featureAsset.FeaturesOnly, out var newFeaturesList))
            {
                _featureAsset.SetFeatures(newFeaturesList);
            }
        }

        public static bool DrawFeaturesEditor(ImmutableArray<(Guid guid, bool isActive)> features, out ImmutableArray<(Guid guid, bool isActive)> updatedSystems)
        {
            ImGui.BeginTable("Features", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit);
            ImGui.TableSetupColumn("Features");

            ImGui.TableHeadersRow();
            ImGui.TableNextColumn();

            int row = 0;
            var newList = new List<(Guid guid, bool isActive)>(features);
            var changed = false;

            foreach (var (guid, isActive) in features)
            {
                if (Game.Data.TryGetAsset<FeatureAsset>(guid) is FeatureAsset asset)
                {
                    string name = asset.Name;

                    ImGui.Text(name);

                    if (ImGuiHelpers.DeleteButton($"del_{name}"))
                    {
                        newList.RemoveAt(row);
                        changed = true;
                    }

                    if (row == 0)
                    {
                        ImGui.SameLine();
                        ImGuiHelpers.DisabledButton(
                            () => ImGui.ArrowButton($"up_{name}{row}", ImGuiDir.Up));
                    }
                    else
                    {
                        ImGui.SameLine();
                        if (ImGui.ArrowButton($"up_{name}{row}", ImGuiDir.Up))
                        {
                            SwitchAt(ref newList, row, row - 1);
                            changed = true;
                        }
                    }

                    if (row == features.Length - 1)
                    {
                        ImGui.SameLine();
                        ImGuiHelpers.DisabledButton(
                            () => ImGui.ArrowButton($"down_{name}{row}", ImGuiDir.Down));
                    }
                    else
                    {
                        ImGui.SameLine();
                        if (ImGui.ArrowButton($"down_{name}{row}", ImGuiDir.Down))
                        {
                            SwitchAt(ref newList, row, row + 1);
                            changed = true;
                        }
                    }

                    ImGui.SameLine();
                    bool isActiveInput = isActive;
                    if (ImGui.Checkbox(label: $"##{name}", ref isActiveInput))
                    {
                        SetAt(ref newList, row, isActiveInput);
                        changed = true;
                    }

                    ImGui.TableNextColumn();
                    ImGui.PushStyleColor(ImGuiCol.Text, Architect.Profile.Theme.Faded);
                    foreach (var system in asset.FetchAllSystems(true))
                    {

                        if (!string.IsNullOrEmpty(_searchSystemText) && !StringHelper.FuzzyMatch(_searchSystemText, system.systemType.Name))
                        {
                            continue;
                        }

                        ImGui.Text($"> {system.systemType.Name}");
                    }
                    ImGui.PopStyleColor();
                }
                else
                {
                    newList.RemoveAt(row);
                    changed = true;
                }
                row++;

                ImGui.TableNextColumn();
            }

            ImGui.EndTable();

            Guid addGuid = Guid.Empty;
            if (SearchBox.SearchAsset(ref addGuid, typeof(FeatureAsset), ignoreAssets: features.Select(f => f.guid), filter: (g => g is FeatureAsset f && !f.IsDiagnostics)))
            {
                var canCreate = true;
                foreach (var other in newList)
                {
                    if (other.guid == addGuid)
                    {
                        canCreate = false;
                        GameLogger.Warning($"Duplicated feature found. ({Game.Data.GetAsset(addGuid).Name})");
                    }
                }

                if (canCreate)
                {
                    newList.Add((addGuid, true));
                    changed = true;
                }
            }

            updatedSystems = newList.ToImmutableArray();

            return changed;
        }

        public static bool DrawSystemsEditor(IList<(Type systemType, bool isActive)> systems, out ImmutableArray<(Type systemType, bool isActive)> updatedSystems)
        {
            return DrawSystemsEditor(systems, currentSystemsList: systems, out updatedSystems);
        }

        public static bool DrawSystemsEditor(IList<(Type systemType, bool isActive)> systems, IList<(Type system, bool isActive)> currentSystemsList, out ImmutableArray<(Type systemType, bool isActive)> updatedSystems)
        {
            ImGui.BeginTable("Systems", 2, ImGuiTableFlags.BordersOuter | ImGuiTableFlags.SizingFixedFit);
            ImGui.TableSetupColumn("Systems");
            ImGui.TableSetupColumn("System List", ImGuiTableColumnFlags.WidthStretch | ImGuiTableColumnFlags.NoHeaderLabel);
            ImGui.TableHeadersRow();
            ImGui.TableNextColumn();

            int row = 0;
            var newList = new List<(Type systemType, bool isActive)>(systems);
            var changed = false;


            foreach (var (systemType, isActive) in systems)
            {
                if (systemType is null)
                {
                    // The system was likely removed.
                    newList.RemoveAt(row);
                    changed = true;

                    continue;
                }

                string name = systemType.FullName!;

                if (ImGuiHelpers.DeleteButton($"del_{name}"))
                {
                    newList.RemoveAt(row);
                    changed = true;
                }

                if (row == 0)
                {
                    ImGui.SameLine();
                    ImGuiHelpers.DisabledButton(
                        () => ImGui.ArrowButton($"up_{name}{row}", ImGuiDir.Up));
                }
                else
                {
                    ImGui.SameLine();
                    if (ImGui.ArrowButton($"up_{name}{row}", ImGuiDir.Up))
                    {
                        SwitchAt(ref newList, row, row - 1);
                        changed = true;
                    }
                }

                if (row == systems.Count - 1)
                {
                    ImGui.SameLine();
                    ImGuiHelpers.DisabledButton(
                        () => ImGui.ArrowButton($"down_{name}{row}", ImGuiDir.Down));
                }
                else
                {
                    ImGui.SameLine();
                    if (ImGui.ArrowButton($"down_{name}{row}", ImGuiDir.Down))
                    {
                        SwitchAt(ref newList, row, row + 1);
                        changed = true;
                    }
                }

                ImGui.SameLine();
                bool isActiveInput = isActive;
                if (ImGui.Checkbox(label: $"##{name}", ref isActiveInput))
                {
                    SetAt(ref newList, row, isActiveInput);
                    changed = true;
                }
                ImGui.TableNextColumn();

                ImGui.Selectable(name.Split('.').Last());
                ImGui.TableNextColumn();

                row++;
            }
            ImGui.EndTable();

            Type? newSystemToAdd = SearchBox.SearchSystems(currentSystemsList.Select(s => s.system));
            if (newSystemToAdd is not null)
            {
                newList.Add((newSystemToAdd, true));
                changed = true;
            }

            updatedSystems = newList.ToImmutableArray();

            return changed;
        }

        public static void SetAt<T>(ref List<(T systemType, bool isActive)> list, int index, bool isActive)
        {
            var kv = list[index];

            list[index] = (kv.systemType, isActive);
        }

        public static void SwitchAt<T>(ref List<(T systemType, bool isActive)> list, int oldIndex, int newIndex)
        {
            var kv = list[oldIndex];

            list.RemoveAt(oldIndex);
            list.Insert(newIndex, kv);
        }
    }
}