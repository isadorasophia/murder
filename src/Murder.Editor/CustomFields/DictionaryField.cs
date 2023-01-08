using ImGuiNET;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
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

        private T? _cachedModifiedKey = default;

        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;
            IDictionary<T, U> dictionary = (IDictionary<T, U>)fieldValue!;

            if (AddNewKey(member, ref dictionary)) return (true, dictionary);

            IEnumerable<(T Key, U Value)> values = dictionary.Select(t => (t.Key, t.Value)).OrderBy(t => t.Key);

            foreach (var kv in values)
            {
                using RectangleBox box = new();

                if (ImGuiHelpers.DeleteButton($"delete_{kv.Key}"))
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

                ImGui.SameLine();
                ImGui.PushID($"change-key {kv.Key}");

                int width = 200;
                
                ImGui.BeginChild($"key_field_{member.Name}", new(width, ImGui.GetFontSize() * 1.5f));
                ImGui.SetNextItemWidth(width);

                T key = kv.Key;

                EditorMember keyMember = member.CreateFrom(typeof(T), "Key", isReadOnly: !CanModifyKeys());
                if (DrawValue(keyMember, key, out T? modifiedKey))
                {
                    _cachedModifiedKey = modifiedKey;
                }

                ImGui.EndChild();

                bool submitKeyChange = false;
                if (CanModifyKeys())
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

                ImGui.PushID($"change-value {kv.Key}");

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

                ImGui.PopID();
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
