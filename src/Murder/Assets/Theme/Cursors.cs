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
        public readonly NineSliceInfo BoxBg = new(image: new("e58ce1db-62a9-53fb-f78d-50b702b5d0bf"));
        [GameAssetId<SpriteAsset>]
        public readonly NineSliceInfo BoxBgHovered = new(image: new("6a647e1f-4940-5a06-a4c1-bd16ff1216ca"));
        [GameAssetId<SpriteAsset>]
        public readonly NineSliceInfo BoxBgSelected = new(image: new("166732e0-d5db-5f97-d853-41b4acec58de"));
        [GameAssetId<SpriteAsset>]
        public readonly NineSliceInfo BoxBgGrayed= new(image: new("15ab60cf-58b0-3167-75f6-44f5fd2e9b5d"));

        // Dialogue
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueIconHello = new("daf91d39-f280-4c83-f2ac-cef2f8c16dc8");
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueIconExit = new("f55bdadc-8f2b-65d5-f9b1-5975d0b14f48");
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueIconBaloon = new("e1e30903-b346-cd23-a4b1-4c87e091cfae");
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueIconAction = new("fa6c06b0-c493-d9a2-976d-aaa945e699e4");
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueIconFlow = new("ddc15cae-7e6e-33c5-6713-188799cf63bb");

        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueIconEdgeNext = new("96cab105-ef05-7da9-0ca9-a4781aee3f20");
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueIconEdgeRandom = new("8adcbd0a-9cca-e2f1-f2dc-cb0d4db2acba");
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueIconEdgeScore = new("e46c489e-4242-72c9-d518-44ab96591d15");
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueIconEdgeChoice = new("0f5036f0-4db3-4c4d-4b8f-16cc2ea995e7");
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueIconEdgeIf = new("a8d14412-3b6a-f782-a94c-0acaf695e288");

        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueBtnPlay = new("2264b5ce-f60e-fff6-27f4-1955dd00b5a4");
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueBtnStepBack = new("acdd67d5-64da-333f-f96c-dfadf9857dce");
        [GameAssetId<SpriteAsset>]
        public readonly Guid DialogueBtnStepForward = new("bbbaf440-1261-2598-8af2-b8d0e9e6922e");

    }
}
