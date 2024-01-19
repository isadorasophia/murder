using Bang.Components;
using ImGuiNET;
using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core.Dialogs;
using Murder.Core.Geometry;
using Murder.Core.Sounds;
using Murder.Core.Ui;
using Murder.Editor.Utilities;
using Murder.Prefabs;
using Murder.Utilities;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text;

namespace Murder.Editor.ImGuiExtended
{
    [Flags]
    public enum SearchBoxFlags
    {
        None = 0,
        Unfolded = 1 << 1
    }

    public static class SearchBox
    {
        private static string _tempSearchText = string.Empty;
        public static int _searchBoxSelection = 0;

        public static bool SearchAsset(ref Guid guid, Type t, SearchBoxFlags flags = SearchBoxFlags.None, IEnumerable<Guid>? ignoreAssets = null, string? defaultText = null, Func<GameAsset, bool>? filter = null) =>
            SearchAsset(ref guid, new GameAssetIdInfo(t, allowInheritance: true), flags, ignoreAssets, defaultText, filter);

        public static bool SearchAsset(
            ref Guid guid,
            GameAssetIdInfo info,
            SearchBoxFlags flags = SearchBoxFlags.None,
            IEnumerable<Guid>? ignoreAssets = null,
            string? defaultText = null,
            Func<GameAsset, bool>? filter = null)
        {
            string selected = defaultText ?? "Select an asset";
            bool hasInitialValue = false;

            if (Game.Data.TryGetAsset(guid) is GameAsset selectedAsset)
            {
                if (selectedAsset.GetType().IsAssignableTo(info.AssetType))
                {
                    selected = selectedAsset.Name;
                    hasInitialValue = true;
                }
                else
                {
                    selected = $"INVALID({selectedAsset.Name})";
                }
            }

            Lazy<Dictionary<string, GameAsset>> candidates = new(() =>
            {
                IEnumerable<GameAsset> assets = Game.Data.FilterAllAssetsWithImplementation(info.AssetType).Values
                    .Where(a => ignoreAssets == null || !ignoreAssets.Contains(a.Guid));

                if (filter is not null)
                {
                    assets = assets.Where(filter);
                }

                return CollectionHelper.ToStringDictionary(assets, a => a.Name, a => a);
            });

            if (Search(id: "a_", hasInitialValue, selected, values: candidates, flags, out GameAsset? chosen))
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
            IEnumerable<Type> types = ReflectionHelper.SafeGetAllTypesInAllAssemblies()
                .Where(p => !p.IsInterface && typeof(IShape).IsAssignableFrom(p));

            Lazy<Dictionary<string, Type>> candidates = new(CollectionHelper.ToStringDictionary(types, t => t.Name, t => t));

            if (Search(id: "c_", hasInitialValue: false, selected, values: candidates, SearchBoxFlags.None, out Type? chosen))
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

            Lazy<Dictionary<string, Type>> candidates = new(() =>
            {
                // Find all non-repeating components
                IEnumerable<Type> types = AssetsFilter.GetAllComponents()
                    .Where(t => excludeComponents?.FirstOrDefault(c => c.GetType() == t) is null && !t.IsGenericType);

                Dictionary<string, Type> result = CollectionHelper.ToStringDictionary(types, t => t.Name, t => t);

                AssetsFilter.FetchStateMachines(result, excludeComponents);
                AssetsFilter.FetchInteractions(result, excludeComponents);

                return result;
            });

            if (Search(id: "c_", hasInitialValue: hasInitialValue, selected, values: candidates, SearchBoxFlags.None, out Type? chosen))
            {
                return chosen;
            }

            return default;
        }

        public static Type? SearchInteractions(Type? initialValue = null)
        {
            string selected = initialValue is null ? "Select an interaction" : initialValue.Name;

            Lazy<Dictionary<string, Type>> candidates = new(() =>
            {
                Dictionary<string, Type> result = new();
                AssetsFilter.FetchInteractions(result, excludeComponents: null);

                return result;
            });

            if (Search(id: "i_", hasInitialValue: initialValue is not null, selected, values: candidates, SearchBoxFlags.None, out Type? chosen))
            {
                return chosen;
            }

            return default;
        }

        public static bool SearchStateMachines(string? initialValue, out Type? chosen)
        {
            string selected;
            if (initialValue is not null)
            {
                selected = initialValue;
            }
            else
            {
                selected = "Select a state machine";
            }

            Lazy<Dictionary<string, Type>> candidates = new(() =>
            {
                Dictionary<string, Type> result = new();
                AssetsFilter.FetchStateMachines(result, excludeComponents: null);

                return result;
            });

            if (Search(id: "s_", hasInitialValue: initialValue != null, selected, values: candidates, SearchBoxFlags.None, out chosen))
            {
                return true;
            }

            return false;
        }

        public static Guid? SearchInstantiableEntities(IEntity? entityToExclude = default)
        {
            string selected = "New entity";

            Guid? excludeGuid = entityToExclude is PrefabAsset ?
                entityToExclude.Guid : entityToExclude is PrefabEntityInstance prefabInstance ?
                prefabInstance.PrefabRef.Guid : null;

            Lazy<Dictionary<string, Guid>> candidates = new(() =>
            {
                Dictionary<string, Guid> result = new();

                result["\uf007 Empty"] = Guid.Empty;

                IEnumerable<PrefabAsset> prefabs = AssetsFilter.GetAllCandidatePrefabs()
                    .Where(e => excludeGuid != e.Guid);

                CollectionHelper.ToStringDictionary(ref result, prefabs, p => p.Name, p => p.Guid);

                return result;
            });

            if (Search(id: "e_", hasInitialValue: false, selected, values: candidates, SearchBoxFlags.None, out Guid chosen))
            {
                return chosen;
            }

            return null;
        }

        public static Type? SearchInterfaces(Type @interface, Type? initialValue = null)
        {
            string selected = initialValue is null ? $"Create {@interface.Name}" : initialValue.Name;

            Lazy<Dictionary<string, Type>> candidates = new(() => CollectionHelper.ToStringDictionary(
                AssetsFilter.GetFromInterface(@interface), s => s.Name, s => s));

            if (Search(id: "s_", hasInitialValue: initialValue is not null, selected, values: candidates, SearchBoxFlags.None, out Type? chosen))
            {
                return chosen;
            }

            return default;
        }

        public static Type? SearchSystems(IEnumerable<Type>? systemsToExclude = default)
        {
            string selected = "Add system";

            Lazy<Dictionary<string, Type>> candidates = new(() => CollectionHelper.ToStringDictionary(
                AssetsFilter.GetAllSystems()
                    .Where(s => systemsToExclude is null || !systemsToExclude.Contains(s)),
                s => s.Name,
                s => s));

            if (Search(id: "s_", hasInitialValue: false, selected, values: candidates, SearchBoxFlags.None, out Type? chosen))
            {
                return chosen;
            }

            return default;
        }

        public static SoundFact? SearchSoundFacts(string id, SoundFact? current)
        {
            string selected;
            bool hasInitialValue = false;

            if (current is SoundFact && id is not null && !string.IsNullOrWhiteSpace(current.Value.Name))
            {
                selected = current.Value.Name;
                hasInitialValue = true;
            }
            else
            {
                selected = " Choose a field to track";
            }

            Lazy<Dictionary<string, SoundFact>> candidates = new(AssetsFilter.GetAllFactsFromSoundBlackboards);

            if (Search(id: $"{id}_s_", hasInitialValue, selected, values: candidates, SearchBoxFlags.None, out SoundFact chosen))
            {
                return chosen.Equals(default(Fact)) ? null : chosen;
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

            Lazy<Dictionary<string, Fact>> candidates = new(AssetsFilter.GetAllFactsFromBlackboards);

            if (Search(id: $"{id}_s_", hasInitialValue, selected, values: candidates, SearchBoxFlags.None, out Fact chosen))
            {
                return chosen;
            }

            return default;
        }

        public static bool SearchEnum<T>(IEnumerable<T> valuesToSearch, [NotNullWhen(true)] out T? chosen) where T : Enum
        {
            string selected = "Add kind";

            Lazy<Dictionary<string, T>> candidates = new(() => valuesToSearch.ToDictionary(v => Enum.GetName(typeof(T), v)!, v => v));
            return Search(id: "s_", hasInitialValue: false, selected, values: candidates, SearchBoxFlags.None, out chosen);
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

            Lazy<Dictionary<string, Guid>> candidates = new(() =>
            {
                // Manually add each key so we don't have problems with duplicates.
                // I think this is better than listing all the duplicates for instances in the world?

                Dictionary<string, Guid> result = new();

                HashSet<string> duplicateKeys = new();
                foreach (Guid g in world.Instances)
                {
                    string name = GetName(g, world);
                    if (duplicateKeys.Contains(name))
                    {
                        continue;
                    }

                    if (result.ContainsKey(name))
                    {
                        duplicateKeys.Add(name);
                        result.Remove(name);

                        continue;
                    }

                    result[name] = g;
                }

                return result;
            });

            if (Search(id: "a_", hasInitialValue, selected, values: candidates, SearchBoxFlags.None, out Guid chosen))
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
            Lazy<Dictionary<string, T>> values,
            SearchBoxFlags flags,
            [NotNullWhen(true)] out T? result)
        {
            result = default;

            bool modified = false;
            bool clicked = false;
            bool isUnfolded = flags.HasFlag(SearchBoxFlags.Unfolded);

            // No selector for unfolded search
            if (!isUnfolded)
            {

                if (hasInitialValue)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.White);

                    if (ImGuiHelpers.IconButton('', $"search_{id}"))
                    {
                        result = default;
                        modified = true;
                    }
                    ImGuiHelpers.HelpTooltip("Reset value");

                    ImGui.SameLine();



                    if (values.Value.TryGetValue(selected, out T? tAsset))
                    {
                        if (tAsset is SpriteAsset spriteAsset)
                        {
                            if (spriteAsset.AsepriteFileInfo != null)
                            {
                                if (ImGuiHelpers.IconButton('', $"search_{id}"))
                                {
                                    Process.Start("Aseprite", $"\"{spriteAsset.AsepriteFileInfo.Value.Source}\"");
                                }
                                ImGuiHelpers.HelpTooltip("Run Aseprite to edit this sprite");

                                ImGui.SameLine();
                            }
                        }

                        if (tAsset is GameAsset asset)
                        {
                            if (ImGuiHelpers.IconButton('', $"search_{id}"))
                            {
                                if (Architect.Instance?.ActiveScene is EditorScene editorScene)
                                {
                                    editorScene.OpenAssetEditor(asset, false);
                                }
                            }
                            ImGuiHelpers.HelpTooltip("Open asset");

                            ImGui.SameLine();
                        }
                    }
                }
                else
                {
                    clicked = ImGuiHelpers.IconButton('\uf055', $"search_{id}");
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
                    _searchBoxSelection = 0;
                }
                ImGui.PopStyleColor(2);

                if (ImGui.IsItemHovered() && hasInitialValue)
                {
                    if (values.Value.TryGetValue(selected, out var raw) && raw is IPreview preview)
                    {
                        ImGui.BeginTooltip();

                        if (values.Value.TryGetValue(selected, out T? tAsset) && tAsset is GameAsset hoveredAsset)
                        {
                            ImGui.TextColored(Game.Profile.Theme.HighAccent, hoveredAsset.Name);
                            ImGui.TextColored(Game.Profile.Theme.Faded, $"{hoveredAsset.Guid}");
                        }

                        EditorAssetHelpers.DrawPreview(preview);
                        ImGui.EndTooltip();
                    }
                }
            }
            else
            {
                ImGui.BeginChild(id + "_search_frame", new Vector2(150,200), ImGuiChildFlags.None, ImGuiWindowFlags.NoMove);
            }
            var pos = ImGui.GetItemRectMin();

            // This is the searchbox window:
            if (isUnfolded || ImGui.BeginPopup(id + "_search", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoBackground))
            {
                pos = new(pos.X, pos.Y + Math.Min(0, ImGui.GetWindowViewport().Size.Y - pos.Y - 400));
                ImGui.SetWindowPos(pos);

                ImGui.BeginChild("##Searchbox_containter", new Vector2(250, 400), ImGuiChildFlags.Border);

                if (ImGui.IsWindowAppearing())
                {
                    ImGui.SetKeyboardFocusHere();
                }
                ImGui.SetNextItemWidth(-1);
                bool enterPressed = ImGui.InputText("##ComboWithFilter_inputText", ref _tempSearchText, 256, ImGuiInputTextFlags.EnterReturnsTrue);


                int count = 0;
                foreach (var (name, asset) in values.Value)
                {
                    if (name.Contains(_tempSearchText, StringComparison.InvariantCultureIgnoreCase))
                    {
                        bool item_selected = count++ == _searchBoxSelection;
                        ImGui.PushID("comboItem" + name);
                        if (ImGui.Selectable(name, item_selected) || (enterPressed && item_selected))
                        {
                            modified = true;
                            result = asset;

                            ImGui.CloseCurrentPopup();
                        }
                        if (item_selected)
                        {
                            ImGuiHelpers.DrawBorderOnPreviousItem(Game.Profile.Theme.HighAccent, 0);
                        }

                        if (ImGui.IsItemHovered())
                        {
                            if (asset is GameAsset hoveredLineAsset)
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextColored(Game.Profile.Theme.Green, hoveredLineAsset.Name);
                                ImGui.TextColored(Game.Profile.Theme.Faded, $"{hoveredLineAsset.Guid}");
                                ImGui.EndTooltip();
                            }
                            else
                            {
                                ImGui.BeginTooltip();
                                ImGui.TextColored(Game.Profile.Theme.Green, name);
                                ImGui.EndTooltip();
                            }
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

                // Handle keyboard arrows
                if (count > 0)
                {
                    if (ImGui.IsKeyPressed(ImGuiKey.UpArrow))
                    {
                        _searchBoxSelection = Calculator.WrapAround(_searchBoxSelection - 1, 0, count - 1);
                    }
                    if (ImGui.IsKeyPressed(ImGuiKey.DownArrow))
                    {
                        _searchBoxSelection = Calculator.WrapAround(_searchBoxSelection + 1, 0, count - 1);
                    }
                }
                else
                {
                    _searchBoxSelection = 0;
                }

                ImGui.EndChild();

                if (isUnfolded)
                {
                    ImGui.EndChild();
                }
                else
                {
                    ImGui.EndPopup();
                }
            }

            return modified;
        }


        public static Type? SearchConstraints()
        {
            string selected = "Select a constraint";

            // Find all non-repeating components
            IEnumerable<Type> types = ReflectionHelper.SafeGetAllTypesInAllAssemblies()
                .Where(p => !p.IsInterface && typeof(IConstraint).IsAssignableFrom(p));

            Lazy<Dictionary<string, Type>> candidates = new(CollectionHelper.ToStringDictionary(types, t => t.Name, t => t));

            return Search(id: "constraint_search", hasInitialValue: false, selected, values: candidates, SearchBoxFlags.None, out Type? chosen) ? chosen : default;
        }
    }
}