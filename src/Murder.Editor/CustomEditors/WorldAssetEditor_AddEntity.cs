using Bang.Components;
using ImGuiNET;
using Murder.Assets;
using Murder.Diagnostics;
using Murder.Editor.Components;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Utilities;
using Murder.Prefabs;
using Murder.Utilities;

namespace Murder.Editor.CustomEditors
{
    internal partial class WorldAssetEditor
    {
        private string _searchEntityText = string.Empty;
        
        private void DrawAllInstancesToAdd(uint id, EditorHook hook)
        {
            ImGui.SetNextWindowBgAlpha(0.75f);
            ImGui.SetNextWindowDockID(id, ImGuiCond.Appearing);

            bool inspectingWindowOpen = true;
            _ = ImGui.Begin($"Entity Picker##Instance_Add_selector", ref inspectingWindowOpen);

            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1);
            ImGui.PushItemWidth(-1);

            bool enterPressed = ImGui.InputText("##ComboWithFilter_entity_inputText", ref _searchEntityText, 256,
                ImGuiInputTextFlags.EnterReturnsTrue);

            ImGui.PopStyleVar();
            ImGui.PopItemWidth();

            const int tableSize = 8;

            float width = ImGui.GetContentRegionMax().X - 140.WithDpi();
            int previewSize = Calculator.RoundToInt(width / tableSize);

            {
                using TableMultipleColumns table = new($"entities_selctor", flags: ImGuiTableFlags.SizingFixedSame, 
                    -1, -1, -1, -1, -1, -1, -1, -1);

                if (table.Opened)
                {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();

                    IEnumerable<PrefabAsset> prefabs = AssetsFilter.GetAllCandidatePrefabs();

                    int counter = 0;
                    foreach (PrefabAsset prefab in prefabs)
                    {
                        if (!prefab.Name.Contains(_searchEntityText, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        if (counter++ % tableSize == 0)
                        {
                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                        }

                        bool isSelected = hook.EntityToBePlaced == prefab.Guid;
                        if (AssetsHelpers.DrawPreviewButton(prefab, previewSize, isSelected))
                        {
                            hook.EntityToBePlaced = prefab.Guid;

                            InstantiateEntityFromSelector(prefab);
                        }

                        ImGui.TableNextColumn();
                    }
                }
            }

            ImGui.End();
        }

        private void InstantiateEntityFromSelector(PrefabAsset asset)
        {
            GameLogger.Verify(_asset is not null && Stages.ContainsKey(_asset.Guid));
            
            EntityInstance instance = EntityBuilder.CreateInstance(asset.Guid);
            foreach (IComponent c in asset.Components)
            {
                instance.AddOrReplaceComponent(c);
            }
            
            instance.AddOrReplaceComponent(new IsPlacingComponent());

            // Only add the instance in the stage until it's actually built.
            Stages[_asset.Guid].AddEntity(instance);
        }
    }
}