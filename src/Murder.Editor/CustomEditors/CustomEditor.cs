using Murder.ImGuiExtended;

namespace Murder.Editor.CustomEditors
{
    /// <summary>
    /// This is a class that allows the user to define its own custom fields
    /// for each of the <see cref="GameAsset"/> targets.
    /// </summary>
    internal abstract class CustomEditor
    {
        public abstract object Target { get; }
        internal abstract void OpenEditor(
            ImGuiRenderer imGuiRenderer, object target);

        internal abstract ValueTask DrawEditor();

        internal virtual void PrepareForSaveAsset() { }
    }
}
