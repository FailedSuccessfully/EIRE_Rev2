public struct Resource
{
    public float Max;
    public float Current;
    public bool Regen;
    public float Rate;

    public float BlockTimer;
}
public enum PlayerResource
{
    Health,
    Mana,
    Barrier
}