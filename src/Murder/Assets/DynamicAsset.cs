namespace Murder.Assets
{
    /// <summary>
    /// These are game assets that will be used in-game.
    /// TODO: Should dynamic objects have an attribute that point to the IComponent they replace...? Or not?
    /// E.g.: IComponent DynamicAsset.ProduceComponent()
    /// </summary>
    public abstract class DynamicAsset : GameAsset
    {
        public override string EditorFolder => "Dynamics/";

        internal abstract void Initialize();
    }
}
