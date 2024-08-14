using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core.Graphics;

namespace Murder.Assets
{
    public class EditorAssets
    {
        // Cursor
        [GameAssetId<SpriteAsset>]
        public Guid Normal = new("6969ba27-bf7a-5a96-bfd4-295703c6bed2");

        [GameAssetId<SpriteAsset>]
        public Guid Point = new("6fd7342b-ae38-092f-c7b7-e80d98dc6b83");

        [GameAssetId<SpriteAsset>]
        public Guid Hand = new("eba43d66-d90a-b69e-5ecd-2eeca4d1162f");

        [GameAssetId<SpriteAsset>]
        public Guid Eye = new("5977841c-6962-360b-3712-55a9e352072d");

        // Cutscenes
        [GameAssetId<SpriteAsset>]
        public readonly Guid CutsceneImage = new("e5084a6c-dcc6-09e0-628b-996637c46224");

        [GameAssetId<SpriteAsset>]
        public readonly Guid AnchorImage = new("e098676d-e88e-086a-42a8-b2cd0d42e217");

        // Generic
        [GameAssetId<SpriteAsset>]
        public readonly NineSliceInfo BoxBg = new(image: new("e58ce1db-62a9-53fb-f78d-50b702b5d0bf"));
        [GameAssetId<SpriteAsset>]
        public readonly NineSliceInfo BoxBgHovered = new(image: new("6a647e1f-4940-5a06-a4c1-bd16ff1216ca"));
        [GameAssetId<SpriteAsset>]
        public readonly NineSliceInfo BoxBgSelected = new(image: new("166732e0-d5db-5f97-d853-41b4acec58de"));
        [GameAssetId<SpriteAsset>]
        public readonly NineSliceInfo BoxBgGrayed = new(image: new("15ab60cf-58b0-3167-75f6-44f5fd2e9b5d"));

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

        [GameAssetId<SpriteAsset>]
        public readonly Guid SoundImage = new("1ad61775-94d2-0765-a254-f16d6c8a3d4d");

        [GameAssetId<SpriteAsset>]
        public readonly Guid MusicImage = new("2a98ce49-1e0a-9109-026f-3392d992e44d");

        [GameAssetId<SpriteAsset>]
        public readonly Guid ListenerImage = new("2a98ce49-1e0a-9109-026f-3392d992e44d");
    }
}