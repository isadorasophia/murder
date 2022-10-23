using InstallWizard.Data;
using InstallWizard.Util;
using InstallWizard.Core.Graphics;
using InstallWizard.Data.Prefabs;
using System.Collections.Immutable;

namespace Editor.CustomEditors
{
    [CustomEditorOf(typeof(SavedWorld))]
    internal class SavedWorldInspector : WorldAssetEditor
    {
        private SavedWorld? _savedWorld;

        protected override ImmutableArray<Guid> Instances => _savedWorld?.Instances ?? ImmutableArray<Guid>.Empty;

        internal override void OpenEditor(ImGuiRenderer imGuiRenderer, object target)
        {
            GameAsset newTarget = (GameAsset)target;

            _asset = newTarget;
            _savedWorld = (SavedWorld)target;

            // TODO: Validate instances?

            if (!Stages.ContainsKey(_savedWorld.Guid))
            {
                InitializeStage(new(imGuiRenderer, _savedWorld), _asset.Guid);
            }
        }

        protected override EntityInstance? TryFindInstance(Guid guid) => _savedWorld?.TryGetInstance(guid);

        protected override bool ShouldDrawSystems => false;

        protected override bool CanAddInstance => false;

        /// <summary>
        /// Not supported as of now.
        /// </summary>
        protected override bool CanDeleteInstance(IEntity? parent, IEntity entity) => false;

        protected override void DeleteInstance(IEntity? parent, Guid instanceGuid) => throw new NotImplementedException();
    }
}