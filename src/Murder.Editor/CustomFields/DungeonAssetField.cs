using InstallWizard.Data.Prefabs;
using InstallWizard.DebugUtilities;
using InstallWizard.Dungeons;
using Editor.Gui;
using Editor.Reflection;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Murder.Assets;

namespace Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<DungeonAssetPlacer>), priority: 10)]
    internal class DungeonAssetField : ImmutableArrayField<DungeonAssetPlacer>
    {
        protected override bool Add(in EditorMember member, [NotNullWhen(true)] out DungeonAssetPlacer? element)
        {
            element = default;

            Guid guid = Guid.Empty;
            if (SearchBox.SearchAsset(ref guid, typeof(PrefabAsset)))
            {
                if (member.CustomElementType is Type elementType)
                {
                    GameLogger.Verify(typeof(DungeonAssetPlacer).IsAssignableFrom(elementType), 
                        "Invalid custom element for placer array!");
                    element = (DungeonAssetPlacer)Activator.CreateInstance(elementType, guid)!;
                }
                else
                {
                    // Create default one
                    element = new DungeonAssetPlacer(guid);
                }

                return true;
            }

            return false;
        }
    }
}
