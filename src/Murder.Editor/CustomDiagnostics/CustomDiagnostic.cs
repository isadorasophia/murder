using Murder.Diagnostics;
using Murder.Editor.CustomComponents;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;

namespace Murder.Editor.CustomDiagnostics
{
    public class CustomDiagnostic
    {
        /// <summary>
        /// Scan all members from a certain <paramref name="target"/> with an <paramref name="identifier"/>.
        /// </summary>
        /// <param name="outputResult">Whether it should show the results to the output window.</param>
        public static bool ScanAllMembers(string identifier, object target, bool outputResult)
        {
            if (target is null)
            {
                return true;
            }

            // Start by the target itself
            if (CustomEditorsHelper.TryGetCustomDiagnostic(target.GetType(), out ICustomDiagnostic? targetDiagnostic))
            {
                Type t = target.GetType();

                if (!targetDiagnostic.IsValid(identifier, target, outputResult))
                {
                    if (outputResult)
                    {
                        GameLogger.Warning($"\uf071 Found invalid component of type '{t.Name}' on '{identifier}'.");
                    }
                }
            }

            IList<(string, EditorMember)> members = CustomComponent.GetMembersOf(target.GetType(), exceptForMembers: null);
            if (members.Count == 0)
            {
                return true;
            }

            bool isValid = true;
            foreach ((string name, EditorMember member) in members)
            {
                object? fieldValue = member.GetValue(target);
                if (fieldValue is null)
                {
                    continue;
                }

                if (CustomEditorsHelper.TryGetCustomDiagnostic(fieldValue.GetType(), out ICustomDiagnostic? diagnostic))
                {
                    Type t = fieldValue.GetType();

                    if (!diagnostic.IsValid(identifier, fieldValue, outputResult))
                    {
                        isValid = false;

                        if (outputResult)
                        {
                            GameLogger.Warning($"\uf071 Found invalid field '{name}' of type '{t.Name}' on '{identifier}'.");
                        }
                    }
                }
            }

            return isValid;
        }
    }
}