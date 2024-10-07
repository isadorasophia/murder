using Murder.Data;

namespace Murder.Assets
{
    /// <summary>
    /// This is an interface implemented by assets which has a preview in the editor.
    /// </summary>
    public interface IPreview
    {
        /// <summary>
        /// Returns the preview id to show this image.
        /// </summary>
        public (string, string) GetPreviewId();
    }
}