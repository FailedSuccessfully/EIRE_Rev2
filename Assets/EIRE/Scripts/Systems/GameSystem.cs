using System;
public abstract class GameSystem
{
    public Type DataType;
    public GameSystem()
    {
        DataType = typeof(GameSystem);
    }
}
