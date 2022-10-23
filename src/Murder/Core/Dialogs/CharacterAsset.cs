using Murder.Assets;
using Murder.Attributes;
using Newtonsoft.Json;
using System.Collections.Immutable;

namespace Murder.Core.Dialogs
{
    public class CharacterAsset : GameAsset
    {
        public override char Icon => '\uf075';
        public override string EditorFolder => "#\uf518Story\\#\uf075Characters";

        [GameAssetId(typeof(SpeakerAsset))]
        public readonly Guid Owner = Guid.Empty;

        /// <summary>
        /// List of tasks or events that the <see cref="Situations"/> may do.
        /// </summary>
        [JsonProperty]
        private readonly SortedList<int, Situation> _situations = new();

        [JsonProperty]
        private int _nextId = 0;

        private ImmutableArray<Situation>? _cachedSituations;

        public ImmutableArray<Situation> Situations
        {
            get
            {
                _cachedSituations ??= _situations.Values.ToImmutableArray();
                return _cachedSituations.Value;
            }
        }

        public int AddNewSituation(string name)
        {
            int id = _nextId++;
            _situations.Add(id, new Situation(id, name));

            _cachedSituations = null;

            return id;
        }

        public Situation? TryFetchSituation(int id)
        {
            if (_situations.TryGetValue(id, out Situation value))
            {
                return value;
            }

            return null;
        }

        public void SetSituationAt(int id, Situation dialog)
        {
            _situations[id] = dialog;
            _cachedSituations = null;
        }

        public void RemoveSituationAt(int id)
        {
            _situations.Remove(id);
            _cachedSituations = null;
        }
    }
}
