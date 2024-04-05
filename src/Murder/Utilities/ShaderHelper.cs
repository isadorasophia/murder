using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using System.Numerics;

namespace Murder.Utilities
{
    public static class ShaderHelper
    {
        public static void SetTechnique(this Effect effect, string id)
        {
            if (effect.CurrentTechnique != effect.Techniques[id])
            {
                effect.CurrentTechnique = effect.Techniques[id];
            }

            // Funny enough, this is consuming way too much memory.
            // GameLogger.Verify(effect.CurrentTechnique != null, $"Skipping technique {id} for shader effect.");
        }

        public static void TrySetParameter(this Effect effect, string id, bool val)
        {
            if (effect.Parameters[id] != null)
            {
                effect.Parameters[id].SetValue(val);
            }
        }

        public static void SetParameter(this Effect effect, string id, bool val)
        {
            if (effect.Parameters[id] != null)
            {
                effect.Parameters[id].SetValue(val);
            }
            else
            {
                GameLogger.Warning($"Shader param '{id}' wasn't found");
            }
        }

        public static void TrySetParameter(this Effect effect, string id, int val)
        {
            if (effect.Parameters[id] != null)
            {
                effect.Parameters[id].SetValue(val);
            }
        }

        public static void SetParameter(this Effect effect, string id, int val)
        {
            if (effect.Parameters[id] != null)
            {
                effect.Parameters[id].SetValue(val);
            }
            else
            {
                GameLogger.Warning($"Shader param '{id}' wasn't found");
            }
        }

        public static void TrySetParameter(this Effect effect, string id, float val)
        {
            if (effect.Parameters[id] != null)
            {
                effect.Parameters[id].SetValue(val);
            }
        }
        public static void SetParameter(this Effect effect, string id, float val)
        {
            if (effect.Parameters[id] != null)
            {
                effect.Parameters[id].SetValue(val);
            }
            else
            {
                GameLogger.Warning($"Shader param '{id}' wasn't found");
            }
        }

        public static void SetParameter(this Effect effect, string id, Microsoft.Xna.Framework.Vector3[] val)
        {
            if (effect.Parameters[id] != null)
            {
                effect.Parameters[id].SetValue(val);
            }
            else
            {
                GameLogger.Warning($"Shader param '{id}' wasn't found");
            }
        }

        public static void SetParameter(this Effect effect, string id, Point val) =>
            SetParameter(effect, id, new Microsoft.Xna.Framework.Vector2(val.X, val.Y));
        public static void SetParameter(this Effect effect, string id, Vector2 val) =>
            SetParameter(effect, id, new Microsoft.Xna.Framework.Vector2(val.X, val.Y));

        public static void SetParameter(this Effect effect, string id, Microsoft.Xna.Framework.Vector3 val)
        {
            if (effect.Parameters[id] != null)
            {
                try
                {
                    effect.Parameters[id].SetValue(val);
                }
                catch (Exception e)
                {
                    GameLogger.Error($"Failed to set shader param '{id}' to {val}: {e}");
                }
            }
            else
            {
                GameLogger.Warning($"Shader param '{id}' wasn't found");
            }
        }

        public static void SetParameter(this Effect effect, string id, Microsoft.Xna.Framework.Vector2 val)
        {
            if (effect.Parameters[id] != null)
            {
                try
                {
                    effect.Parameters[id].SetValue(val);
                }
                catch (Exception e)
                {
                    GameLogger.Error($"Failed to set shader param '{id}' to {val}: {e}");
                }
            }
            else
            {
                GameLogger.Warning($"Shader param '{id}' wasn't found");
            }
        }

        public static void SetParameter(this Effect effect, string id, Texture2D val)
        {
            if (effect.Parameters[id] != null)
            {
                effect.Parameters[id].SetValue(val);
            }
            else
            {
                GameLogger.Warning($"Shader param '{id}' wasn't found");
            }
        }
    }
}