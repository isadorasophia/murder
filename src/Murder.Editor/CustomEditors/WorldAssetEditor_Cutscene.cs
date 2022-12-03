using Bang.Components;
using ImGuiNET;
using Murder.Components;
using Murder.Components.Cutscenes;
using Murder.Diagnostics;
using Murder.Editor.Components;
using Murder.Editor.CustomComponents;
using Murder.Editor.Stages;
using Murder.Editor.Utilities;
using Murder.ImGuiExtended;
using Murder.Prefabs;
using Murder.Utilities;

namespace Murder.Editor.CustomEditors
{
    internal partial class WorldAssetEditor
    {
        protected virtual bool DrawCutsceneEditor(Stage stage)
        {
            GameLogger.Verify(_world is not null);
            
            bool modified = false;

            if (ImGui.Button("New cutscene!"))
            {
                EntityInstance empty = EntityBuilder.CreateInstance(Guid.Empty, "Cutscene");

                empty.AddOrReplaceComponent(new CutsceneAnchorsComponent());
                empty.AddOrReplaceComponent(new PositionComponent());

                AddInstance(empty);
                modified = true;
            } 

            IList<IEntity> cutscenes = stage.FindEntitiesWith(typeof(CutsceneAnchorsComponent));
            foreach (IEntity cutscene in cutscenes)
            {
                if (TreeEntityGroupNode($"{cutscene.Name}##{cutscene.Guid}", Game.Profile.Theme.White, icon: '\ue131'))
                {
                    if (ImGuiHelpers.DeleteButton($"Delete#{cutscene.Guid}"))
                    {
                        DeleteInstance(parent: null, cutscene.Guid);
                        modified = true;
                    }

                    ImGui.SameLine();

                    // Do not modify the name for entity assets, only instances.
                    if (ImGuiHelpers.IconButton('\uf304', $"rename_{cutscene.Guid}"))
                    {
                        ImGui.OpenPopup($"Rename#{cutscene.Guid}");
                    }

                    if (ImGui.BeginPopup($"Rename#{cutscene.Guid}"))
                    {
                        if (DrawRenameInstanceModal(parent: null, cutscene))
                        {
                            ImGui.CloseCurrentPopup();
                        }

                        ImGui.EndPopup();
                    }

                    // Draw components!
                    var components = GetComponents(parent: null, cutscene);
                    foreach (IComponent c in components)
                    {
                        Type t = c.GetType();
                        
                        if (ImGui.TreeNodeEx(ReflectionHelper.GetGenericName(t)))
                        {
                            // TODO: This is modifying the memory of all readonly structs.
                            IComponent copy = SerializationHelper.DeepCopy(c);
                            
                            if (CustomComponent.ShowEditorOf(copy))
                            {
                                // Asset was already modified, just pass along the updated asset.
                                ReplaceComponent(null, cutscene, copy);
                            }

                            ImGui.TreePop();
                        }
                    }

                    ImGui.TreePop();
                }
            }
            
            return modified;
        }
    }
}