using Murder.ImGuiExtended;

namespace Murder.Editor.CustomEditors
{
    /// <summary>
    /// This is a class that allows the user to define its own custom fields
    /// for each of the <see cref="GameAsset"/> targets.
    /// </summary>
    public abstract class CustomEditor
    {
        public abstract object Target { get; }

        public abstract void OpenEditor(
            ImGuiRenderer imGuiRenderer, object target);

        public abstract ValueTask DrawEditor();

        public virtual void PrepareForSaveAsset() { }
    }
}
