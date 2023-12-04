namespace Murder.Assets;

public readonly struct LocalizedString
{
    public readonly Guid Id = Guid.Empty;

    public LocalizedString() { }

    public LocalizedString(Guid id) => Id = id;
}
