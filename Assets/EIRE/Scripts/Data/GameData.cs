using UnityEngine;

public class GameData
{
    public GameData() { }
}

public class InputData : GameData
{
    public UnityEngine.InputSystem.InputActionMap Actions;
}

public class CharacterData : GameData
{
    public enum PlayerResource
    {
        Health,
        Mana,
        Barrier
    }
    public int playerIndex;
    public Vector3 Direction, Speed;
    public float BaseSpeed, MaxSpeed;
    public Resource Health, Mana, Barrier;
    public AttackProps[] AttackProperties;

}

public class BattleData : GameData
{
    public int layer;
}

public class ResourceData : GameData
{
    public Resource[] RoundResources;
}

public class UIData : GameData
{

}