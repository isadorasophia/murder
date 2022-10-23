using System.Collections.Immutable;
using InstallWizard.Util;
using Editor.Reflection;
using InstallWizard;
using InstallWizard.Data.Hardware;
using System.Diagnostics.CodeAnalysis;

namespace Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<SpellSlot>))]
    internal class ImmutableSpellSlotField : ImmutableArrayField<SpellSlot>
    {
        protected override bool Add(in EditorMember member, [NotNullWhen(true)] out SpellSlot slot)
        {
            slot = default;

            if (ImGuiExtended.IconButton('\uf055', $"{member.Name}_add", Game.Data.GameProfile.Theme.Accent))
            {
                slot = new();
                return true;
            }

            return false;
        }
    }
}
