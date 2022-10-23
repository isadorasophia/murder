using InstallWizard;
using InstallWizard.Core;
using InstallWizard.Util;
using InstallWizard.Util.Attributes;
using Editor.Gui;
using Editor.Reflection;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
namespace Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<Point>))]
    internal class ImmutableArrayPointField : ImmutableArrayField<Point>
    {
        protected override bool Add(in EditorMember member, [NotNullWhen(true)] out Point element)
        {
            element = Point.Zero;
            if (ImGuiExtended.IconButton('', $"{member.Name}_add", Game.Data.GameProfile.Theme.Accent))
            {
                return true;
            }
            return false;
        }
    }
}
