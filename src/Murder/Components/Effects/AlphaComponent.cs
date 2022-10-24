using Newtonsoft.Json;
using Bang.Components;

namespace Murder.Components
{
    public enum AlphaSources
    {
        Alpha,
        Fade,
        LastSeen
    }

    /// <summary>
    /// Set alpha of a component being displayed in the screen.
    /// </summary>
    internal readonly struct AlphaComponent : IComponent 
    {
        [JsonProperty]
        private readonly float[] _sources = { 1f, 1f, 1f };

        public AlphaComponent() { }

        public AlphaComponent(float[] sources)
        {
            _sources = sources;
        }

        public AlphaComponent(AlphaSources source, float amount)
        {
            Set(source, amount);
        }

        public AlphaComponent Set(AlphaSources source, float amount)
        {
            _sources[(int)source] = amount;
            return new(_sources);
        }

        public float Get(AlphaSources source)
        {
            return _sources[(int)source];
        }

        public float Alpha
        {
            get
            {
                float alpha = 1;
                if (_sources is not null)
                {
                    foreach (var a in _sources)
                    {
                        alpha *= a;
                    }
                }

                return alpha;
            }
        }
    }
}
