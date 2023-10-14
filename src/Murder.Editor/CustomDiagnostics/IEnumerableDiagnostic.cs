using System.Collections;

namespace Murder.Editor.CustomDiagnostics
{
    [CustomDiagnostic(typeof(IEnumerable))]
    internal class IEnumerableDiagnostic : ICustomDiagnostic
    {
        public bool IsValid(string identifier, in object target, bool outputResult)
        {
            bool isValid = true;

            IEnumerable array = (IEnumerable)target;
            foreach (var item in array)
            {
                isValid |= CustomDiagnostic.ScanAllMembers(identifier, item, outputResult);
            }

            return isValid;
        }
    }
}