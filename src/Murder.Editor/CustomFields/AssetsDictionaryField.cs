using ImGuiNET;
using Murder.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using System.Collections.Immutable;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(IDictionary<Guid, int>), priority: 10)]
    internal class AssetsDictionaryField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;
            IDictionary<Guid, int> assets = (IDictionary<Guid, int>)fieldValue!;

            var resourceGuidToNameMap = new Dictionary<Guid, string>();
            if (AttributeExtensions.FindGameAssetType(member, out GameAssetIdInfo? info))
            {
                resourceGuidToNameMap = Game.Data.FilterAllAssets(info.Value.AssetType)
                    .ToDictionary(kv => kv.Key, kv => kv.Value.Name);
            }

            // Find out whether we have a new resource to be a candidate to the "Add" button!
            List<Guid> candidateResources = new();
            foreach (var r in resourceGuidToNameMap.Keys)
            {
                if (!assets.ContainsKey(r))
                {
                    candidateResources.Add(r);
                }
            }

            ImGui.PushID($"Add ${member.Name}");

            if (candidateResources.Count != 0 && ImGui.Button("Add"))
            {
                if (assets is ImmutableDictionary<Guid, int> immutable)
                {
                    assets = immutable.Add(candidateResources.First(), 1);
                }
                else
                {
                    assets.Add(candidateResources.First(), 1);
                }

                modified = true;
            }
            else if (candidateResources.Count == 0)
            {
                ImGuiHelpers.DisabledButton("Add");
            }

            ImGui.PopID();

            if (modified) return (true, assets);

            int index = 0;
            foreach (var kv in assets)
            {
                if (!resourceGuidToNameMap.TryGetValue(kv.Key, out string? selectedResourceName))
                {
                    continue;
                }

                if (ImGuiHelpers.DeleteButton($"delete_{kv.Key}"))
                {
                    if (assets is ImmutableDictionary<Guid, int> immutable)
                    {
                        assets = immutable.Remove(kv.Key);
                    }
                    else
                    {
                        assets.Remove(kv.Key);
                    }

                    return (true, assets);
                }

                ImGui.SameLine();
                ImGui.PushID($"Change-key-{index}");
                ImGui.SetNextItemWidth(200);

                if (ImGui.BeginCombo("", selectedResourceName))
                {
                    foreach (var currentResource in candidateResources)
                    {
                        string newResourceName = resourceGuidToNameMap[currentResource];

                        if (ImGui.MenuItem(newResourceName))
                        {
                            if (assets is ImmutableDictionary<Guid, int> immutable)
                            {
                                assets = immutable.Remove(kv.Key).Add(currentResource, kv.Value);
                            }
                            else
                            {
                                assets.Remove(kv.Key);
                                assets[currentResource] = kv.Value;
                            }

                            modified = true;
                        }
                    }

                    ImGui.EndCombo();
                }

                ImGui.PopID();

                if (modified) return (true, assets);

                ImGui.SameLine();
                ImGui.PushID($"Change-value-{index}");

                ImGui.SetNextItemWidth(120);

                int quantity = kv.Value;
                if (ImGui.InputInt("", ref quantity, 1))
                {
                    if (assets is ImmutableDictionary<Guid, int> immutable)
                    {
                        assets = immutable.SetItem(kv.Key, quantity);
                    }
                    else
                    {
                        assets[kv.Key] = quantity;
                    }

                    modified = true;
                }

                ImGui.PopID();

                index++;
            }

            return (modified, assets);
        }
    }
}