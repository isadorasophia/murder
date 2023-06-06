using ImGuiNET;
using Bang.Components;
using Bang.Interactions;
using Bang.StateMachines;
using System.Diagnostics.CodeAnalysis;
using Murder.Assets;
using Murder.Prefabs;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Core.Dialogs;
using Murder.Editor.Utilities;
using System.Text;
using Murder.Services;
using Murder.Core.Sounds;

namespace Murder.Editor.ImGuiExtended
{
    public static class SearchBox
    {
        private static string _tempSearchText = string.Empty;
        private static int _tempCurrentItem = 0;

        public static bool SearchAsset(ref Guid guid, Type assetType, IEnumerable<Guid>? ignoreAssets = null, string? defaultText = null)
        {
            string selected = defaultText ?? "Select an asset";
            bool hasInitialValue = false;

            if (Game.Data.TryGetAsset(guid) is GameAsset selectedAsset)
            {
                selected = selectedAsset.Name;
                hasInitialValue = true;
            }

            var candidates = Game.Data.FilterAllAssetsWithImplementation(assetType).Values.Where(a => ignoreAssets == null || !ignoreAssets.Contains(a.Guid))
                .ToDictionary(a => a.Name, a => a);
            
            if (Search(id: "a_", hasInitialValue, selected, values: candidates, out GameAsset? chosen))
            {
                if (chosen is null)
                {
                    guid = default;
                    return true;
                }

                guid = chosen.Guid;
                return true;
            }

            return false;
        }

        public static Type? SearchShapes()
        {
            string selected = "Select a shape";

            // Find all non-repeating components
            var candidates = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => !p.IsInterface && typeof(IShape).IsAssignableFrom(p))
                .ToDictionary(t => t.Name, t => t);

            if (Search(id: "c_", hasInitialValue: false, selected, values: candidates, out Type? chosen))
            {
                return chosen;
            }

            return default;
        }

        public static Type? SearchComponent(IEnumerable<IComponent>? excludeComponents = default, IComponent? initialValue = default) =>
            SearchComponentType(excludeComponents, initialValue?.GetType());
        
        public static Type? SearchComponentType(IEnumerable<IComponent>? excludeComponents = default, Type? t = default)
        {
            string selected = "Select a component";

            bool hasInitialValue = false;
            if (t is not null)
            {
                selected = t.IsGenericType ? t.GenericTypeArguments[0].Name : t.Name;
                hasInitialValue = true;
            }

            // Find all non-repeating components
            var candidates = AssetsFilter.GetAllComponents()
                .Where(t => excludeComponents?.FirstOrDefault(c => c.GetType() == t) is null && !t.IsGenericType)
                .ToDictionary(t => t.Name, t => t);

            AssetsFilter.FetchStateMachines(candidates, excludeComponents);
            AssetsFilter.FetchInteractions(candidates, excludeComponents);

            if (Search(id: "c_", hasInitialValue: hasInitialValue, selected, values: candidates, out Type? chosen))
            {
                return chosen;
            }

            return default;
        }
        
        public static Type? SearchInteractions()
        {
            string selected = "Select an interaction";

            Dictionary<string, Type> candidates = new();
            AssetsFilter.FetchInteractions(candidates, excludeComponents: null);

            if (Search(id: "i_", hasInitialValue: false, selected, values: candidates, out Type? chosen))
            {
                return chosen;
            }

            return default;
        }

        public static Type? SearchStateMachines()
        {
            string selected = "Select a state machine";

            Dictionary<string, Type> candidates = new();
            AssetsFilter.FetchStateMachines(candidates, excludeComponents: null);

            if (Search(id: "s_", hasInitialValue: false, selected, values: candidates, out Type? chosen))
            {
                return chosen;
            }

            return default;
        }

        public static Guid? SearchInstantiableEntities(IEntity? entityToExclude = default)
        {
            string selected = "New entity";

            Guid? excludeGuid = entityToExclude is PrefabAsset ? 
                entityToExclude.Guid : entityToExclude is PrefabEntityInstance prefabInstance ? 
                prefabInstance.PrefabRef.Guid : null;

            var candidates = AssetsFilter.GetAllCandidatePrefabs()
                .Where(e => excludeGuid != e.Guid)
                .ToDictionary(e => e.Name, e => e.Guid);

            GameLogger.Verify(!candidates.ContainsKey("Empty"), "Overriding empty entity.");
            candidates["Empty"] = Guid.Empty;

            if (Search(id: "e_", hasInitialValue: false, selected, values: candidates, out Guid chosen))
            {
                return chosen;
            }

            return null;
        }

        public static Type? SearchInterfaces(Type @interface)
        {
            string selected = $"Create {@interface.Name}";

            var candidates = AssetsFilter.GetFromInterface(@interface)
                .ToDictionary(s => s.Name, s => s);
            if (Search(id: "s_", hasInitialValue: false, selected, values: candidates, out Type? chosen))
            {
                return chosen;
            }

            return default;
        }

        public static Type? SearchSystems(IEnumerable<Type>? systemsToExclude = default)
        {
            string selected = "Add system";

            var candidates = AssetsFilter.GetAllSystems()
                .Where(s => systemsToExclude is null || !systemsToExclude.Contains(s))
                .ToDictionary(s => s.Name, s => s);
            if (Search(id: "s_", hasInitialValue: false, selected, values: candidates, out Type? chosen))
            {
                return chosen;
            }

            return default;
        }

        public static string? SearchSounds(string initial, string id)
        {
            string selected = "Select a sound";
            bool hasValue = false;
            if (!string.IsNullOrEmpty(initial))
            {
                selected = initial;
                hasValue = true;
            }

            Dictionary<string, string> candidates = Game.Data.Sounds.ToDictionary(v => v, v => v);

            if (ImGuiHelpers.IconButton('', $"play_sound_{id}"))
            {
                _ = SoundServices.Play(new SoundEventId { Path = selected });
            }
            ImGui.SameLine();

            if (Search(id: $"search_sound##{id}", hasInitialValue: hasValue, selected, values: candidates, out string? chosen))
            {
                return chosen;
            }
            
            return default;
        }

        public static Fact? SearchFacts(string id, Fact? current)
        {
            string selected;
            bool hasInitialValue = false;

            if (current is Fact && id is not null && !string.IsNullOrWhiteSpace(current.Value.EditorName))
            {
                selected = current.Value.EditorName;
                hasInitialValue = true;
            }
            else
            {
                selected = "Select fact";
            }

            var candidates = AssetsFilter.GetAllFactsFromBlackboards();

            if (Search(id: $"{id}_s_", hasInitialValue, selected, values: candidates, out Fact chosen))
            {
                return chosen.Equals(default(Fact)) ? null : chosen;
            }

            return default;
        }

        public static bool SearchEnum<T>(IEnumerable<T> valuesToSearch, [NotNullWhen(true)] out T? chosen) where T : Enum
        {
            string selected = "Add kind";

            var candidates = valuesToSearch.ToDictionary(v => Enum.GetName(typeof(T), v)!, v => v);
            return Search(id: "s_", hasInitialValue: false, selected, values: candidates, out chosen);
        }

        public static bool SearchInstanceInWorld(ref Guid guid, WorldAsset world)
        {
            string selected = "Select an instance";
            bool hasInitialValue = false;

            if (world.TryGetInstance(guid) is EntityInstance instance)
            {
                selected = instance.Name;
                hasInitialValue = true;
            }

            string GetName(Guid g, WorldAsset _)
            {
                StringBuilder result = new();
                if (world.GetGroupOf(g) is string folder)
                {
                    result.Append($"{folder}/");
                }
                
                result.Append(world.TryGetInstance(g)!.Name);
                return result.ToString();
            }
            
            // Manually add each key so we don't have problems with duplicates.
            Dictionary<string, Guid> candidates = new();
            HashSet<string> duplicateKeys = new();
            foreach (Guid g in world.Instances)
            {
                string name = GetName(g, world);
                if (duplicateKeys.Contains(name))
                {
                    continue;
                }

                if (candidates.ContainsKey(name))
                {
                    duplicateKeys.Add(name);
                    candidates.Remove(name);

                    continue;
                }

                candidates[name] = g;
            }
            
            if (Search(id: "a_", hasInitialValue, selected, values: candidates, out Guid chosen))
            {
                if (chosen == Guid.Empty)
                {
                    guid = default;
                    return false;
                }

                guid = chosen;
                return true;
            }

            return false;
        }

        /// <summary>
        /// This is set when a custom width for the search box is set.
        /// </summary>
        private static int _searchBoxWidth = -1;

        /// <summary>
        /// This is set to restore default when drawing the search box.
        /// </summary>
        public static void PushItemWidth(int width)
        {
            _searchBoxWidth = width;
        }

        public static void PopItemWidth()
        {
            _searchBoxWidth = -1;
        }

        /// <summary>
        /// Internally called through <see cref="SearchBox"/> implementations.
        /// Complete entrypoint for creating a search box.
        /// </summary>
        public static bool Search<T>(
            string id,
            bool hasInitialValue,
            string selected, 
            IDictionary<string, T> values,
            [NotNullWhen(true)] out T? result)
        {
            result = default;

            bool modified = false;
            bool clicked = false;

            if (hasInitialValue)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.White);

                if (ImGuiHelpers.IconButton('\uf2f1', $"search_{id}"))
                {
                    result = default;
                    modified = true;
                }
                if (ImGui.IsItemHovered() && values[selected] is GameAsset asset)
                    ImGui.SetTooltip(asset.Guid.ToString());

                ImGui.SameLine();
            }
            else
            {
                clicked = ImGuiHelpers.IconButton('\uf055',$"search_{id}");
                ImGui.SameLine();
                ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.Faded); 
            }

            ImGui.PushStyleColor(ImGuiCol.Header, Game.Profile.Theme.BgFaded);

            const int padding = 6;
            Vector2 size = new(_searchBoxWidth != -1 ? _searchBoxWidth : ImGui.GetContentRegionAvail().X - padding, ImGui.CalcTextSize(selected).Y);
            if (ImGui.Selectable(selected, true, ImGuiSelectableFlags.None, size) || clicked)
            {
                ImGui.OpenPopup(id + "_search");
                _tempSearchText = string.Empty;
                _tempCurrentItem = 0;
            }
            ImGui.PopStyleColor(2);


            if (ImGui.IsItemHovered() && hasInitialValue)
            {
                if (values.TryGetValue(selected, out var raw) && raw is IPreview preview)
                {
                    ImGui.BeginTooltip();
                    EditorAssetHelpers.DrawPreview(preview);
                    ImGui.EndTooltip();
                }
            }

            var pos = ImGui.GetItemRectMin();

            if (ImGui.BeginPopup(id + "_search", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove))
            {
                pos = new(pos.X, pos.Y + Math.Min(0, ImGui.GetWindowViewport().Size.Y - pos.Y - 400));
                ImGui.SetWindowPos(pos);

                if (ImGui.IsWindowAppearing())
                {
                    ImGui.SetKeyboardFocusHere();
                }

                bool enterPressed = ImGui.InputText("##ComboWithFilter_inputText", ref _tempSearchText, 256, ImGuiInputTextFlags.EnterReturnsTrue);
                
                ImGui.BeginChild("##Searchbox_containter", new Vector2(-1, 400), true, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove);

                int count = 0;
                foreach (var (name, asset) in values)
                {
                    if (name.Contains(_tempSearchText, StringComparison.InvariantCultureIgnoreCase))
                    {
                        bool item_selected = count++ == _tempCurrentItem;
                        ImGui.PushID("comboItem" + name);
                        if (ImGui.Selectable(name, item_selected) || (enterPressed && item_selected))
                        {
                            modified = true;
                            result = asset;

                            ImGui.CloseCurrentPopup();
                        }
                        if (item_selected)
                        {
                            ImGui.SetItemDefaultFocus();
                        }

                        if (ImGui.IsItemHovered())
                        {
                            if (asset is IPreview preview)
                            {
                                ImGui.BeginTooltip();
                                EditorAssetHelpers.DrawPreview(preview);
                                ImGui.EndTooltip();
                            }
                        }

                        ImGui.PopID();
                    }
                }

                ImGui.EndChild();

                ImGui.EndPopup();
            }

            return modified;
        }
    }
}
