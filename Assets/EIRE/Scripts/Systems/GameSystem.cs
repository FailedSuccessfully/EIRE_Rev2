using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameSystem
{
    public Type DataType;
    public GameSystem()
    {
        DataType = typeof(GameSystem);
    }

    public virtual void onUpdate() { }
    public virtual void OnFixedUpdate() { }
    public virtual void SortConnections(List<Component> response) { }

    //?public virtual void RequestDriver() { }
}
