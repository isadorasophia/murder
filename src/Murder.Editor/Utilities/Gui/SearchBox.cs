using Bang.Components;
using ImGuiNET;
using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core.Dialogs;
using Murder.Core.Geometry;
using Murder.Core.Sounds;
using Murder.Diagnostics;
using Murder.Editor.Utilities;
using Murder.Prefabs;
using Murder.Utilities;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text;
using static Murder.Editor.ImGuiExtended.SearchBox;

namespace Murder.Editor.ImGuiExtended
{
    [Flags]
    public enum SearchBoxFlags
    {
        None = 0,
        Unfolded = 1 << 1,
        IconOnly = 1 << 2
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
            SearchBoxSettings<GameAsset> settings = new(initialText: defaultText ?? "Select an asset");

            if (Game.Data.TryGetAsset(guid) is GameAsset selectedAsset)
            {
                if (selectedAsset.GetType().IsAssignableTo(info.AssetType))
                {
                    settings.InitialSelected = new(selectedAsset.Name, selectedAsset);
                }
                else
                {
                    settings.InitialSelected = new($"INVALID({selectedAsset.Name})", selectedAsset);
                }
            }

            Lazy<Dictionary<string, GameAsset>> candidates = new(() =>
            {
                IEnumerable<GameAsset> assets = Architect.EditorData.FilterAllAssetsWithImplementation(info.AssetType).Values
                    .Where(a => ignoreAssets == null || !ignoreAssets.Contains(a.Guid));

                if (filter is not null)
                {
                    assets = assets.Where(filter);
                }

                return CollectionHelper.ToStringDictionary(assets, a => a.Name, a => a);
            });

            if (Search(id: "a_", settings, values: candidates, flags, out GameAsset? chosen))
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
            SearchBoxSettings<Type> settings = new(initialText: "Select a shape");

            // Find all non-repeating components
            IEnumerable<Type> types = ReflectionHelper.SafeGetAllTypesInAllAssemblies()
                .Where(p => !p.IsInterface && typeof(IShape).IsAssignableFrom(p));

            Lazy<Dictionary<string, Type>> candidates = new(CollectionHelper.ToStringDictionary(types, t => t.Name, t => t));

            if (Search(id: "c_", settings, values: candidates, SearchBoxFlags.None, out Type? chosen))
            {
                return chosen;
            }

            return default;
        }

        public static Type? SearchComponent(IEnumerable<IComponent>? excludeComponents = default, IComponent? initialValue = default) =>
            SearchComponentType(excludeComponents, initialValue?.GetType());

        public static Type? SearchComponentType(IEnumerable<IComponent>? excludeComponents = default, Type? t = default, string? initialText = null)
        {
            SearchBoxSettings<Type> settings = new(initialText: initialText ?? "Select a component");

            if (t is not null)
            {
                settings.InitialSelected = new(t.IsGenericType ? t.GenericTypeArguments[0].Name : t.Name, t);
            }

            Lazy<Dictionary<string, Type>> candidates = new(() =>
            {
                // Find all non-repeating components
                IEnumerable<Type> types = AssetsFilter.GetAllComponents()
                    .Where(t => excludeComponents?.FirstOrDefault(c => c.GetType() == t) is null && !t.IsGenericType);

                Dictionary<string, Type> result = CollectionHelper.ToStringDictionary(types, t => t.Name, t => t);

                AssetsFilter.FetchStateMachines(result, subtypeOf: null, excludeComponents);
                AssetsFilter.FetchInteractions(result, excludeComponents);

                return result;
            });

            if (Search(id: "c_", settings, values: candidates, SearchBoxFlags.None, out Type? chosen))
            {
                return chosen;
            }

            return default;
        }

        public static Type? SearchInteractions(Type? initialValue = null)
        {
            SearchBoxSettings<Type> settings = new(initialText: "Select an interaction");

            if (initialValue is not null)
            {
                settings.InitialSelected = new(initialValue.Name, initialValue);
            }

            Lazy<Dictionary<string, Type>> candidates = new(() =>
            {
                Dictionary<string, Type> result = new();
                AssetsFilter.FetchInteractions(result, excludeComponents: null);

                return result;
            });

            if (Search(id: "i_", settings, values: candidates, SearchBoxFlags.None, out Type? chosen))
            {
                return chosen;
            }

            return default;
        }

        public static bool SearchStateMachines(Type? initialValue, out Type? chosen, Type? subtypeOf = null)
        {
            SearchBoxSettings<Type> settings = new(initialText: "Select a state machine");

            if (initialValue is not null)
            {
                string? name = initialValue.GetGenericArguments().FirstOrDefault()?.Name;
                settings.InitialSelected = new(name ?? initialValue.Name, initialValue);
            }

            Lazy<Dictionary<string, Type>> candidates = new(() =>
            {
                Dictionary<string, Type> result = new();
                AssetsFilter.FetchStateMachines(result, subtypeOf, excludeComponents: null);

                return result;
            });

            if (Search(id: "s_", settings, values: candidates, SearchBoxFlags.None, out chosen))
            {
                return true;
            }

            return false;
        }

        public static Guid? SearchInstantiableEntities(IEntity? entityToExclude = default)
        {
            SearchBoxSettings<Guid> settings = new(initialText: "New entity");

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

            if (Search(id: "e_", settings, values: candidates, SearchBoxFlags.None, out Guid chosen))
            {
                return chosen;
            }

            return null;
        }

        public static Type? SearchInterfaces(Type @interface, Type? initialValue = null)
        {
            SearchBoxSettings<Type> settings = new(initialText: $"Create {@interface.Name}");

            if (initialValue is not null)
            {
                settings.InitialSelected = new(initialValue.Name, initialValue);
            }

            Lazy<Dictionary<string, Type>> candidates = new(() => CollectionHelper.ToStringDictionary(
                AssetsFilter.GetFromInterface(@interface), s => s.Name, s => s));

            if (Search(id: "s_", settings, values: candidates, SearchBoxFlags.None, out Type? chosen))
            {
                return chosen;
            }

            return default;
        }

        public static Type? SearchSystems(IEnumerable<Type>? systemsToExclude = default)
        {
            SearchBoxSettings<Type> settings = new(initialText: "Add system");

            Lazy<Dictionary<string, Type>> candidates = new(() => CollectionHelper.ToStringDictionary(
                AssetsFilter.GetAllSystems()
                    .Where(s => systemsToExclude is null || !systemsToExclude.Contains(s)),
                s => s.Name,
                s => s));

            if (Search(id: "s_", settings, values: candidates, SearchBoxFlags.None, out Type? chosen))
            {
                return chosen;
            }

            return default;
        }

        public static SoundFact? SearchSoundFacts(string id, SoundFact? current)
        {
            SearchBoxSettings<SoundFact> settings = new(initialText: " Choose a field to track");

            if (current is SoundFact && id is not null && !string.IsNullOrWhiteSpace(current.Value.Name))
            {
                settings.InitialSelected = new(current.Value.Name, current.Value);
            }

            Lazy<Dictionary<string, SoundFact>> candidates = new(AssetsFilter.GetAllFactsFromSoundBlackboards);

            if (Search(id: $"{id}_s_", settings, values: candidates, SearchBoxFlags.None, out SoundFact chosen))
            {
                return chosen.Equals(default(Fact)) ? null : chosen;
            }

            return default;
        }

        public static Fact? SearchFacts(string id, Fact? current, BlackboardKind kind = BlackboardKind.All)
        {
            SearchBoxSettings<Fact> settings = new(initialText: "Select fact");

            if (current is Fact && id is not null && !string.IsNullOrWhiteSpace(current.Value.EditorName))
            {
                settings.InitialSelected = new(current.Value.EditorName, current.Value);
            }

            Lazy<Dictionary<string, Fact>> candidates = new(() => AssetsFilter.GetAllFactsFromBlackboards(kind));

            if (Search(id: $"{id}_s_", settings, values: candidates, SearchBoxFlags.None, out Fact chosen))
            {
                return chosen;
            }

            return default;
        }

        public static bool SearchEnum<T>(IEnumerable<T> valuesToSearch, [NotNullWhen(true)] out T? chosen) where T : Enum
        {
            SearchBoxSettings<T> settings = new(initialText: "Add kind");

            Lazy<Dictionary<string, T>> candidates = new(() => valuesToSearch.ToDictionary(v => Enum.GetName(typeof(T), v)!, v => v));
            return Search(id: "s_", settings, values: candidates, SearchBoxFlags.None, out chosen);
        }

        public static bool SearchInstanceInWorld(ref Guid guid, WorldAsset world)
        {
            SearchBoxSettings<Guid> settings = new(initialText: "Select an instance");

            if (world.TryGetInstance(guid) is EntityInstance instance)
            {
                settings.InitialSelected = new(instance.Name, guid);
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

                Dictionary<string, Guid> result = [];

                HashSet<string> duplicateKeys = [];
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

            if (Search(id: "a_", settings, values: candidates, SearchBoxFlags.None, out Guid chosen))
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
            SearchBoxSettings<T> settings,
            Lazy<Dictionary<string, T>> values,
            SearchBoxFlags flags,
            [NotNullWhen(true)] out T? result
        ) => Search(id, settings, values, flags, SearchBoxConfiguration.Default, out result);

        public struct SearchBoxSettings<T>
        {
            private readonly string _initialUnitializedText;

            public InitialSelectedValue<T>? InitialSelected;

            public readonly string InitialText => InitialSelected?.Name ?? _initialUnitializedText;

            public readonly T? Selected => InitialSelected is null ? default : InitialSelected.Value.Target;

            [MemberNotNullWhen(true, nameof(InitialSelected))]
            public bool HasInitialValue => InitialSelected is not null;

            /// <summary>
            /// If applicable, provide a default value in the search box.
            /// </summary>
            public readonly (string, T)? DefaultInitialization { get; init; } = null;

            public SearchBoxSettings(string initialText) =>
                _initialUnitializedText = initialText;
        }

        public struct InitialSelectedValue<T>
        {
            public readonly string Name;
            public readonly T Target;

            public InitialSelectedValue(string name, T target) =>
                (Name, Target) = (name, target);
        }

        public static bool Search<T>(
            string id,
            SearchBoxSettings<T> settings,
            Lazy<Dictionary<string, T>> values,
            SearchBoxFlags flags,
            SearchBoxConfiguration sizeConfiguration,
            [NotNullWhen(true)] out T? result)
        {
            result = default;

            bool modified = false;
            bool clicked = false;
            bool isUnfolded = flags.HasFlag(SearchBoxFlags.Unfolded);

            // No selector for unfolded search
            if (!isUnfolded)
            {
                if (settings.HasInitialValue)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.White);

                    if (ImGuiHelpers.IconButton('', $"search_{id}"))
                    {
                        result = default;
                        modified = true;
                    }

                    if (!ImGui.IsItemVisible())
                    {
                        ImGui.PopStyleColor();
                        return false;
                    }

                    ImGuiHelpers.HelpTooltip("Reset value");

                    ImGui.SameLine();

                    if (settings.Selected is SpriteAsset spriteAsset)
                    {
                        ImGui.BeginGroup();
                        if (spriteAsset.AsepriteFileInfo == null)
                        {
                            ImGui.BeginDisabled();
                        }

                        if (ImGuiHelpers.IconButton('', $"search_{id}") && spriteAsset.AsepriteFileInfo != null)
                        {
                            try
                            {
                                var process = new Process
                                {
                                    StartInfo = new ProcessStartInfo
                                    {
                                        FileName = spriteAsset.AsepriteFileInfo.Value.Source,
                                        UseShellExecute = true // Use the OS to open the file
                                    }
                                };

                                process.Start();
                            }
                            catch (Exception ex)
                            {
                                // Handle exceptions like file not found, etc.
                                GameLogger.Error($"Exception: {ex.Message}");
                            }
                        }

                        if (spriteAsset.AsepriteFileInfo == null)
                        {
                            ImGui.EndDisabled();
                            ImGui.EndGroup();
                            ImGuiHelpers.HelpTooltip("Aseprite File Info not embeded, check your Editor Settings");
                        }
                        else
                        {
                            ImGui.EndGroup();
                            ImGuiHelpers.HelpTooltip("Open this in an external editor");
                        }
                            
                        ImGui.SameLine();
                    }

                    if (settings.Selected is GameAsset asset)
                    {
                        ImGuiHelpers.AssetButton(asset);
                        ImGui.SameLine();
                    }
                }
                else
                {
                    clicked = ImGuiHelpers.IconButton('\uf055', $"search_{id}");
                    ImGui.SameLine();
                    ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.Accent);
                }

                ImGui.PushStyleColor(ImGuiCol.Header, Game.Profile.Theme.Bg);
                ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Game.Profile.Theme.Faded);

                string selectedName = settings.InitialText;

                const int padding = 6;
                Vector2 size = new(_searchBoxWidth != -1 ? _searchBoxWidth : ImGui.GetContentRegionAvail().X - padding, ImGui.CalcTextSize(selectedName).Y);
                if (!flags.HasFlag(SearchBoxFlags.IconOnly))
                {
                    if (ImGui.Selectable(selectedName, true, ImGuiSelectableFlags.NoAutoClosePopups, size))
                    {
                        clicked = true;
                    }
                }
                if (clicked)
                {
                    ImGui.OpenPopup(id + "_search");
                    _tempSearchText = string.Empty;
                    _searchBoxSelection = 0;
                }
                ImGui.PopStyleColor(3);

                if (ImGui.IsItemHovered() && settings.HasInitialValue)
                {
                    if (settings.Selected is IPreview preview)
                    {
                        ImGui.BeginTooltip();

                        if (settings.Selected is GameAsset hoveredAsset)
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
                ImGui.BeginChild(id + "_search_frame", sizeConfiguration.SearchFrameSize, ImGuiChildFlags.None, ImGuiWindowFlags.NoMove);
            }
            var pos = ImGui.GetItemRectMin();

            // This is the searchbox window:
            if (isUnfolded || ImGui.BeginPopup(id + "_search", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoBackground))
            {
                pos = new(pos.X, pos.Y + Math.Min(0, ImGui.GetWindowViewport().Size.Y - pos.Y - 400));
                ImGui.SetWindowPos(pos);

                ImGui.BeginChild("##Searchbox_containter", sizeConfiguration.SearchBoxContainerSize, ImGuiChildFlags.Border);

                if (ImGui.IsWindowAppearing())
                {
                    ImGui.SetKeyboardFocusHere();
                }
                ImGui.SetNextItemWidth(-1);
                bool enterPressed = ImGui.InputText("##ComboWithFilter_inputText", ref _tempSearchText, 256, ImGuiInputTextFlags.EnterReturnsTrue);

                if (settings.DefaultInitialization is (string defaultValueName, T value) && ImGui.Button(defaultValueName))
                {
                    modified = true;
                    result = value;

                    _tempSearchText = string.Empty;
                    ImGui.CloseCurrentPopup();
                }

                var orderedKeyAndValue = values.Value.OrderBy(n => n.Key);

                int count = 0;
                foreach ((string name, T asset) in orderedKeyAndValue)
                {
                    if (name.Contains(_tempSearchText, StringComparison.InvariantCultureIgnoreCase))
                    {
                        bool item_selected = count++ == _searchBoxSelection;
                        ImGui.PushID("comboItem" + name);
                        if (ImGui.Selectable(name, item_selected) || (enterPressed && item_selected))
                        {
                            modified = true;
                            result = asset;
                            _tempSearchText = string.Empty;

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

        public readonly record struct SearchBoxConfiguration(
            Vector2 SearchFrameSize,
            Vector2 SearchBoxContainerSize
        )
        {
            public static SearchBoxConfiguration Default = new(
                SearchFrameSize: new Vector2(150, 200),
                SearchBoxContainerSize: new Vector2(250, 400)
            );
        }
    }
}