using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Utilities;
using Newtonsoft.Json;
using SharpFont;
using System.Text.RegularExpressions;

namespace Murder.Assets.Graphics
{
    public class SpriteAsset : GameAsset
    {
        public override string EditorFolder => "#Sprites";
        public override char Icon => '';

        [AtlasTexture]
        public string FirstFrame = string.Empty;

        public AtlasId Atlas;

        public string Frames = string.Empty;
        public float FramesPerSecond = 12;

        [HideInEditor, JsonIgnore]
        private Animation _animation = new Animation();

        public Animation Animation => _animation;

        internal override void AfterDeserialized()
        {
            RecalculateAnimation();
        }

        public void RecalculateAnimation()
        {
            (string alphaPart, string numberPart) = SplitNumbers(FirstFrame);

            if (numberPart.Length > 0)
            {
                var intFrames = CreateFrameList();
                var fileNames = CreateFileList(intFrames).ToArray();

                _animation = new Animation(
                    fileNames,
                    Calculator.RepeatingArray(100f / FramesPerSecond, fileNames.Length)
                );
            }
            else
            {
                _animation = new Animation(
                    new string[] { FirstFrame },
                    Calculator.RepeatingArray(100f / FramesPerSecond, 1)
                );
            }
        }

        public IEnumerable<string> CreateFileList(IEnumerable<int> frames)
        {
            (string alphaPart, string numberPart) = SplitNumbers(FirstFrame);
            int padding = numberPart.Length;

            foreach (var frame in frames)
            {
                var frameName = alphaPart + frame.ToString().PadLeft(padding, '0');
                yield return frameName;
            }
        }

        private (string alphaPart, string numberPart) SplitNumbers(string firstFrame)
        {
            Regex re = new Regex(@"([a-zA-Z\\/_]+)(\d+)");
            Match result = re.Match(firstFrame);

            return (alphaPart: result.Groups[1].Value, numberPart: result.Groups[2].Value);
        }

        // Creates an int list using the format "1,2,3-8,9*3"
        public IEnumerable<int> CreateFrameList()
        {
            foreach (var frameGroup in Frames.Split(","))
            {
                var list = frameGroup.Split('-');
                if (list.Length > 1 && !string.IsNullOrWhiteSpace(list[1]))
                {
                    if (int.TryParse(list[0], out var min) && int.TryParse(list[1], out var max))
                    {
                        foreach (var frame in Enumerable.Range(min, max - min + 1))
                            yield return frame;
                    }
                }
                else
                {
                    var stack = frameGroup.Split('*');
                    if (stack.Length > 1)
                    {
                        if (int.TryParse(stack[0], out var val) && int.TryParse(stack[1], out var multiplier))
                        {
                            for (int i = 0; i < multiplier; i++)
                                yield return val;
                        }
                    }
                    else if (int.TryParse(frameGroup, out var frame))
                    {
                        yield return frame;
                    }
                }

            }
        }
        internal void Draw(Batch2D spriteBatch, Vector2 position, float sort)
        {
            var (imgPath, complete) = Animation.Evaluate(0f, Game.Instance.ElapsedTime);
            if (string.IsNullOrWhiteSpace(imgPath))
                imgPath = FirstFrame;

            if (Atlas == Data.AtlasId.None)
            {
                var texture = Game.Data.FetchTexture(FirstFrame);

                spriteBatch.Draw(texture, new Rectangle(position.X, position.Y, texture.Width, texture.Height), Color.White, sort);
            }
            else
            {
                var atlas = Game.Data.FetchAtlas(Atlas);
                var texture = atlas.Get(imgPath);

                texture.Draw(spriteBatch, new Rectangle(position.X, position.Y, texture.Width, texture.Height),
                    Color.White, sort);
            }
        }

        internal void Draw(Batch2D spriteBatch, Rectangle destinationBox, Color white, float sort)
        {
            var (imgPath, complete) = Animation.Evaluate(0f, Game.Instance.ElapsedTime);
            if (string.IsNullOrWhiteSpace(imgPath))
                imgPath = FirstFrame;

            if (Atlas == Data.AtlasId.None)
            {
                var texture = Game.Data.FetchTexture(FirstFrame);

                spriteBatch.Draw(texture, destinationBox, Color.White, sort);
            }
            else
            {
                var atlas = Game.Data.FetchAtlas(Atlas);
                var texture = atlas.Get(imgPath);

                texture.Draw(spriteBatch, destinationBox,
                    Color.White, sort);
            }
        }
    }
}