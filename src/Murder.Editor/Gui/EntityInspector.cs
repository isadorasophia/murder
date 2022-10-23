using InstallWizard;
using InstallWizard.Util;
using Editor.CustomComponents;
using Editor.Util;
using ImGuiNET;
using Bang.Components;
using Bang.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Gui
{
    internal static class EntityInspector
    {
        public static bool DrawInspector(Entity entity)
        {
            bool isOpen = true;

            ImGui.Begin($"Entity_Inspector_{entity.EntityId}", ref isOpen);

            foreach (IComponent c in entity.Components)
            {
                if (ImGui.TreeNode($"{c.GetType().Name}##Component_inspector_{c.GetType().Name}"))
                {
                    // This is modifying the memory of all readonly structs, so only create a copy if this 
                    // is not a modifiable component.
                    IComponent copy = c is IModifiableComponent ? c : SerializationHelper.DeepCopy(c);
                    if (CustomComponent.ShowEditorOf(copy))
                    {
                        // This will trigger reactive systems.
                        entity.ReplaceComponent(copy, copy.GetType());
                    }

                    ImGui.TreePop();
                }
            }

            ImGui.End();

            return isOpen;
        }
    }
}
