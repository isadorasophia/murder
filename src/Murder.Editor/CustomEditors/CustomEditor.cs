using Murder.Core.Graphics;
using Murder.Editor.ImGuiExtended;

namespace Murder.Editor.CustomEditors
{
    /// <summary>
    /// This is a class that allows the user to define its own custom fields
    /// for each of the <see cref="GameAsset"/> targets.
    /// </summary>
    public abstract class CustomEditor : IDisposable
    {
        public abstract object Target { get; }

        public abstract void OpenEditor(
            ImGuiRenderer imGuiRenderer, RenderContext renderContext, object target, bool overwrite);
        
        public abstract void DrawEditor();

        /// <summary>
        /// Reload all active editors.
        /// </summary>
        public virtual void ReloadEditor() { }

        public virtual void CloseEditor(Guid target) { }

        public virtual void PrepareForSaveAsset() { }

        public virtual void Dispose() { }
    }
}
