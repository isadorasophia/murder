using ImGuiNET;
using Murder.Attributes;
using Murder.Core;
using Murder.Diagnostics;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using System;
using System.Collections.Immutable;
using System.Xml.Linq;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableDictionary<Guid, ImmutableArray<Guid>>))]
    internal class DictionaryGuidImmutableArrayGuidField : DictionaryField<Guid, ImmutableArray<Guid>>
    {
        private Guid _new = Guid.Empty;

        protected override bool AddNewKey(EditorMember member, ref IDictionary<Guid, ImmutableArray<Guid>> dictionary)
        {
            if (ImGuiHelpers.IconButton('\uf055', $"##add_key_{member.Name}"))
            {
                _new = Guid.Empty;
                ImGui.OpenPopup("Add Item##dictionary");
            }

            if (!AttributeExtensions.TryGetAttribute(member, out GameAssetDictionaryIdAttribute? gameAssetId))
            {
                GameLogger.Error($"{member.Name} needs a GameAssetDictionaryIdAttribute!");
                return false;
            }

            if (ImGui.BeginPopup("Add Item##dictionary"))
            {
                ImGui.BeginChild("##add_dictionary_guid_guid", new(250, ImGui.GetFontSize() * 1.5f));
                SearchBox.SearchAsset(ref _new, gameAssetId.Key, SearchBoxFlags.None, dictionary.Keys);
                ImGui.EndChild();

                if (ImGui.Button("Create"))
                {
                    ImGui.CloseCurrentPopup();

                    if (dictionary is ImmutableDictionary<Guid, ImmutableArray<Guid>> immutable)
                    {
                        dictionary = immutable.Add(_new, ImmutableArray<Guid>.Empty);
                    }
                    else
                    {
                        dictionary.Add(_new, ImmutableArray<Guid>.Empty);
                    }

                    ImGui.EndPopup();
                    return true;
                }

                ImGui.EndPopup();
            }

            return false;
        }

        protected override bool CanModifyKeys() => true;
    }
}