using System;
public abstract class GameSystem
{
    public Type DataType;
    public GameSystem()
    {
        DataType = typeof(GameSystem);
    }

    public virtual void onUpdate() { }
    public virtual void onFixedUpdate() { }

    //?public virtual void RequestDriver() { }
}
