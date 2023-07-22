using ImGuiNET;
using Murder.Attributes;
using Murder.Core.Sounds;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.CustomFields
{
    public abstract class DictionaryField<T, U> : CustomField where T : notnull
    {
        protected virtual List<T> GetCandidateKeys(EditorMember member, IDictionary<T, U> fieldValue) => 
            new();

        protected virtual bool Add(IList<T> candidates, [NotNullWhen(true)] out (T Key, U Value)? element)
        {
            element = (default!, default!);
            return true;
        }

        protected virtual bool AddNewKey(EditorMember member, ref IDictionary<T, U> dictionary)
        {
            bool added = false;

            List<T> candidateResources = GetCandidateKeys(member, dictionary);
            
            ImGui.PushID($"Add ${member.Name}");

            if (candidateResources.Count != 0 && Add(candidateResources, out var element) && 
                !dictionary.ContainsKey(element.Value.Key))
            {
                if (dictionary is ImmutableDictionary<T, U> immutable)
                {
                    dictionary = immutable.Add(element.Value.Key, element.Value.Value);
                }
                else
                {
                    dictionary.Add(element.Value.Key, element.Value.Value);
                }

                added = true;
            }
            else if (candidateResources.Count == 0)
            {
                ImGuiHelpers.DisabledButton("All keys have been added!");
            }

            ImGui.PopID();

            return added;
        }

        protected virtual bool CanModifyKeys() => false;

        /// <summary>
        /// Whether the dictionary should wait for "Ok" or just submit the key.
        /// </summary>
        protected virtual bool ShouldAutomaticallySubmitKey() => 
            typeof(T) == typeof(Guid) || typeof(T) == typeof(SoundFact) || typeof(T).IsAssignableTo(typeof(Enum));

        private T? _cachedModifiedKey = default;

        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;
            IDictionary<T, U> dictionary = (IDictionary<T, U>)fieldValue!;

            if (AddNewKey(member, ref dictionary)) return (true, dictionary);

            IEnumerable<(T Key, U Value)> values = dictionary.Select(t => (t.Key, t.Value)).OrderBy(t => t.Key);

            int index = 0;
            foreach (var kv in values)
            {
                using RectangleBox box = new();

                if (ImGui.Button($"\uf1f8 Delete Item##{index}"))
                {
                    if (dictionary is ImmutableDictionary<T, U> immutable)
                    {
                        dictionary = immutable.Remove(kv.Key);
                    }
                    else
                    {
                        dictionary.Remove(kv.Key);
                    }

                    return (true, dictionary);
                }

                string? keyLabel = null;
                string? valueLabel = null;

                if (AttributeExtensions.TryGetAttribute(member, out EditorLabelAttribute? label))
                {
                    keyLabel = label.Label1;
                    valueLabel = label.Label2;
                }

                string? keyTooltip = null;
                string? valueTooltip = null;

                if (AttributeExtensions.TryGetAttribute(member, out EditorTupleTooltipAttribute? tooltip))
                {
                    keyTooltip = tooltip.Tooltip1;
                    valueTooltip = tooltip.Tooltip2;
                }

                // ======
                // Key label
                // ======
                if (keyLabel is not null)
                {
                    ImGui.Text(keyLabel);
                    ImGui.SameLine();
                }

                ImGui.PushID($"change-key-{index}-{member.Name}");

                int width = 300;
                
                ImGui.BeginChild($"key_field_{member.Name}_{index}", new(width, ImGui.GetFontSize() * 1.5f));
                ImGui.SetNextItemWidth(width);

                T key = kv.Key;

                bool submitKeyChange = false;
                EditorMember keyMember = member.CreateFrom(typeof(T), "Key", isReadOnly: !CanModifyKeys());
                if (DrawValue(keyMember, key, out T? modifiedKey))
                {
                    _cachedModifiedKey = modifiedKey;

                    if (ShouldAutomaticallySubmitKey())
                    {
                        submitKeyChange = true;
                    }
                }

                // ======
                // Key tooltip
                // ======
                if (keyTooltip is not null)
                {
                    ImGuiHelpers.HelpTooltip(keyTooltip);
                }

                ImGui.EndChild();

                if (!ShouldAutomaticallySubmitKey() && CanModifyKeys())
                {
                    ImGui.SameLine();
                    submitKeyChange = ImGui.Button("Ok!") || ImGui.IsKeyPressed(ImGuiKey.Enter);
                }

                if (submitKeyChange && _cachedModifiedKey is not null)
                {
                    if (dictionary.ContainsKey(_cachedModifiedKey))
                    {
                        // Key is already present, ignore!
                    }
                    else if (dictionary is ImmutableDictionary<T, U> immutable)
                    {
                        dictionary = immutable.Remove(kv.Key).Add(_cachedModifiedKey, kv.Value);
                    }
                    else
                    {
                        dictionary.Remove(kv.Key);
                        dictionary[_cachedModifiedKey] = kv.Value;
                    }

                    _cachedModifiedKey = default;
                    modified = true;
                }

                ImGui.PopID(); // change-key {kv.Key}

                if (modified) return (true, dictionary);

                // ======
                // Value label
                // ======
                if (valueLabel is not null)
                {
                    ImGui.Text(valueLabel);
                    ImGui.SameLine();
                }

                ImGui.PushID($"change-value-{index}-{member.Name}");
                ImGui.SetNextItemWidth(120);

                U value = kv.Value;
                if (DrawElementValue(member.CreateFrom(typeof(U), "Value", element: CustomElementTypeOfValue(key)), value, out U? modifiedValue))
                {
                    if (dictionary is ImmutableDictionary<T, U> immutable)
                    {
                        dictionary = immutable.SetItem(kv.Key, modifiedValue);
                    }
                    else
                    {
                        dictionary[kv.Key] = modifiedValue;
                    }

                    modified = true;
                }

                // ======
                // Value tooltip
                // ======
                if (valueTooltip is not null)
                {
                    ImGuiHelpers.HelpTooltip(valueTooltip);
                }

                ImGui.PopID();

                index++;
            }

            return (modified, dictionary);
        }

        /// <summary>
        /// This is optionally implemented by dictionaries that have a collection with a type
        /// that depends on its key.
        /// </summary>
        public virtual Type? CustomElementTypeOfValue(T key) => default;

        public virtual bool DrawElementValue(EditorMember member, U value, [NotNullWhen(true)] out U? modifiedValue)
        {
            return DrawValue(member, value, out modifiedValue);
        }
    }
}
