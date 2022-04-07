using System;
public abstract class GameSystem
{
    public Type DataType;
    public GameSystem()
    {
        DataType = typeof(GameSystem);
    }

    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }

    public virtual void OnStart() { }
    public virtual void InitPlayers() { }

    //?public virtual void RequestDriver() { }
}
