using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core.Graphics;

namespace Murder.Assets
{
    [Serializable]
    public class EditorAssets
    {
        // Cursor
        [GameAssetId<SpriteAsset>]
        public Guid Normal = new("4faebd9a-d9a5-3033-363a-88ed8a5e7946");

        [GameAssetId<SpriteAsset>]
        public Guid Point = new("6b32b897-322c-21a0-2788-d50b890e23cc");

        [GameAssetId<SpriteAsset>]
        public Guid Hand = new("fc4818c5-bbb0-83b5-0b56-9c0b08cf7d12");

        [GameAssetId<SpriteAsset>]
        public Guid Eye = new("52826fff-e1a5-54a7-89c2-fb8996da272f");

        // Cutscenes
        [GameAssetId<SpriteAsset>]
        public readonly Guid CutsceneImage = new("9cc41c2e-7a73-844e-8146-1c6dacfc1db9");

        [GameAssetId<SpriteAsset>]
        public readonly Guid AnchorImage = new("cc2d6ddd-9d67-b912-430a-9787bab45b33");

        // Generic
        [GameAssetId<SpriteAsset>]
        public readonly NineSliceInfo BoxBg = new(Guid.Empty);
        [GameAssetId<SpriteAsset>]
        public readonly NineSliceInfo BoxBgHovered = new(Guid.Empty);
        [GameAssetId<SpriteAsset>]
        public readonly NineSliceInfo BoxBgSelected = new(Guid.Empty);
        [GameAssetId<SpriteAsset>]
        public readonly NineSliceInfo BoxBgGrayed= new(Guid.Empty);

        // Dialogue
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueIconHello = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueIconExit = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueIconBaloon = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueIconAction = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueIconFlow = Guid.Empty;

        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueIconEdgeNext = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueIconEdgeRandom = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueIconEdgeScore = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueIconEdgeChoice = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueIconEdgeIf = Guid.Empty;

        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueBtnPlay = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueBtnStepBack = Guid.Empty;
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueBtnStepForward = Guid.Empty;

    }
}
