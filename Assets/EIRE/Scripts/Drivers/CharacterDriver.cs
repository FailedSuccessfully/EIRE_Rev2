using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDriver : Driver<Player>
{
    private GameData[] Data => GameManager.GetPlayerData(context);
    public override void Drive()
    {
        throw new System.NotImplementedException();
    }
    public override void Mount(Player ctx)
    {
        base.Mount(ctx);
    }
}
