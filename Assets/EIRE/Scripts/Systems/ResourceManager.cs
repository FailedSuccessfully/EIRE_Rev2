using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ResourceManager : GameSystem
{
    public ResourceManager()
    {
        DataType = typeof(ResourceData);
    }

    public override void OnFixedUpdate()
    {
        string toLog = "";
        foreach (Player p in GameManager.Players)
        {
            var d = GameManager.GetPlayerData<ResourceData>(p);
            RegenResources(d.RoundResources);
            GameManager.SetData<ResourceData>(p, d);
            toLog += $"Player {p.index + 1} HP: {d.RoundResources[0].Current}" + Environment.NewLine;
        }
        Debug.Log(toLog);
    }

    public void InitPlayers()
    {
        foreach (Player p in GameManager.Players)
        {
            InitPlayer(p);
        }
    }
    private void InitPlayer(Player p)
    {
        ResourceData d = new ResourceData();
        d.RoundResources = BuildResources(GameManager.GetPlayerData<CharacterData>(p));
        GameManager.CreateData<ResourceData>(p, this);
        GameManager.SetData<ResourceData>(p, d);
    }

    private Resource[] BuildResources(CharacterData data)
    {
        Resource[] rs = new Resource[] { data.Health, data.Mana, data.Barrier };
        for (var i = 0; i < rs.Length; i++)
        {
            rs[i].BlockTimer = 0f;
        }
        return rs;
    }

    public static void LoseResource(CharacterData.PlayerResource resource, Player player, float amount)
    {
        var d = GameManager.GetPlayerData<ResourceData>(player);
        int index = (int)resource;
        d.RoundResources[index].Current -= amount;
        GameManager.SetData<ResourceData>(player, d);
    }

    private void RegenResources(Resource[] resources)
    {
        for (var i = 0; i < resources.Length; i++)
        {
            if (resources[i].Rate != 0)
            {
                resources[i].Regen = resources[i].BlockTimer <= 0 ? true : false;
                resources[i].Current += resources[i].Regen && resources[i].Current < resources[i].Max ? resources[i].Rate * Time.fixedDeltaTime : 0f; //this overflows max, need to clamp it to size
                resources[i].BlockTimer -= !resources[i].Regen && resources[0].BlockTimer > 0 ? Time.fixedDeltaTime : 0f;
            }

        }
    }

    private void BlockRegenForTime(CharacterData.PlayerResource resource, Player player, float time)
    {
        var d = GameManager.GetPlayerData<ResourceData>(player);
        int index = (int)resource;
        d.RoundResources[index].BlockTimer = time;
        d.RoundResources[index].Regen = false;
        GameManager.SetData<ResourceData>(player, d);
    }

}