using ImGuiNET;
using Murder.Core.Physics;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using Murder.Utilities.Attributes;
using System.Reflection;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(int))]
    internal class IntField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;
            int number = (int)fieldValue!;

            if (AttributeExtensions.IsDefined(member, typeof(CollisionLayerAttribute)))
            {
                var list = AssetsFilter.CollisionLayers;

                int selection = 0;
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i].id == number)
                    {
                        selection = i;
                        break;
                    }
                }

                if (ImGui.Combo($"#{member.Name}-{member.Member.Name}-col-layer", ref selection, AssetsFilter.CollisionLayersNames, AssetsFilter.CollisionLayersNames.Length))
                {
                    number = list[selection].id;
                    modified = true;
                }
                return (modified, number);

            }

            modified = ImGui.InputInt("", ref number, 1);
            
            return (modified, number);
        }
    }
}
