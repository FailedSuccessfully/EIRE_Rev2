public struct Resource
{
    public float Max;
    public float Current;
    public bool RegenLock, TimerLock;
    public float Rate;

    public float BlockTimer;
    public bool IsRegen()=> RegenLock && TimerLock;
}
public enum PlayerResource
{
    Health,
    Mana,
    Barrier
}