using Bang;
using Murder.Assets;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Stages;
using Murder.Prefabs;
using System.Collections.Immutable;

namespace Murder.Editor.CustomEditors
{
    [CustomEditorOf(typeof(SavedWorld))]
    internal class SavedWorldInspector : WorldAssetEditor
    {
        private SavedWorld? _savedWorld;

        protected override ImmutableArray<Guid> Instances => _savedWorld?.Instances ?? ImmutableArray<Guid>.Empty;

        protected override void OnSwitchAsset(ImGuiRenderer imGuiRenderer, RenderContext renderContext, bool forceInit)
        {
            _savedWorld = (SavedWorld)_asset!;

            // TODO: Validate instances?

            if (forceInit || !Stages.ContainsKey(_savedWorld.Guid))
            {
                InitializeStage(new(imGuiRenderer, renderContext, Stage.StageType.None, _savedWorld), _asset!.Guid);
            }
        }

        public override EntityInstance? TryFindInstance(Guid guid) => _savedWorld?.TryGetInstance(guid);

        protected override bool ShouldDrawSystems => false;

        protected override bool CanAddInstance => false;

        /// <summary>
        /// Not supported as of now.
        /// </summary>
        protected override bool CanDeleteInstance(IEntity? parent, IEntity entity) => false;

        protected override void DeleteInstance(IEntity? parent, Guid instanceGuid) => throw new NotImplementedException();
    }
}