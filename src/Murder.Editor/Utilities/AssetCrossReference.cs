using Murder.Assets;
using Murder.Editor.Assets;
using Murder.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Editor.Utilities
{

    public class AssetCrossReference()
    {
        public readonly List<Guid> References = [];
        public bool Initialized = false;

        public void Clear()
        {
            References.Clear();
            Initialized = false;
        }
    }

    public static class FilteredAssetsForCrossReference
    {
        // temporary cache list of all the assets that reference this.
        private static readonly Dictionary<Guid, AssetCrossReference> _references = [];

        private static List<Guid>? _eligibleAssets = null;

        public static List<Guid> FetchAssetReferences(Guid target)
        {
            if (_eligibleAssets is null)
            {
                _eligibleAssets = [];

                foreach (GameAsset asset in Game.Data.GetAllAssets())
                {
                    if (FilteredAssetsForLocalization.ShouldSkip(asset))
                    {
                        continue;
                    }

                    _eligibleAssets.Add(asset.Guid);
                }
            }

            if (!_references.TryGetValue(target, out AssetCrossReference? reference))
            {
                reference = new();
                _references[target] = reference;
            }

            string key = target.ToString();

            if (!reference.Initialized)
            {
                reference.Initialized = true;

                foreach (Guid guid in _eligibleAssets)
                {
                    GameAsset? asset = Game.Data.TryGetAsset(guid);
                    if (asset is null)
                    {
                        continue;
                    }

                    string json = FileManager.SerializeToJson(asset);
                    if (json.Contains(key))
                    {
                        reference.References.Add(guid);
                    }
                }
            }

            return reference.References;
        }

        public static void Clear()
        {
            _eligibleAssets = null;

            foreach (var info in _references)
            {
                info.Value.Clear();
            }
        }
    }
}
