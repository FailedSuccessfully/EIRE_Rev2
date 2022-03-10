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
        foreach (Player p in GameManager.Players)
        {
            var d = GameManager.GetPlayerData<ResourceData>(p);
            RegenResources(d.RoundResources);
            GameManager.SetData<ResourceData>(p, d);

        }
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

    public static void LoseResource(PlayerResource resource, Player player, float amount)
    {
        var d = GameManager.GetPlayerData<ResourceData>(player);
        int index = (int)resource;
        d.RoundResources[index].Current = Mathf.Clamp(d.RoundResources[index].Current - amount, 0, d.RoundResources[index].Max);
        GameManager.SetData<ResourceData>(player, d);
        if (resource == PlayerResource.Health) // temporary measures
            EventsManager.CheckEvent(GameEvent.HPZero, d.RoundResources[index].Current);
        if (resource == PlayerResource.Mana) //temporrary
            BlockRegenForTime(resource, player, 0.3f);
    }

    private void RegenResources(Resource[] resources)
    {
        for (var i = 0; i < resources.Length; i++)
        {
            if (resources[i].Rate != 0)
            {
                resources[i].TimerLock = resources[i].BlockTimer <= 0 ? true : false;
                resources[i].Current += resources[i].IsRegen() && resources[i].Current < resources[i].Max ? resources[i].Rate * Time.fixedDeltaTime : 0f; //this overflows max, need to clamp it to size
                resources[i].BlockTimer -= !resources[i].TimerLock && resources[i].BlockTimer >= 0 ? Time.fixedDeltaTime : 0f;
            }

        }
    }

    private static void BlockRegenForTime(PlayerResource resource, Player player, float time)
    {
        var d = GameManager.GetPlayerData<ResourceData>(player);
        int index = (int)resource;
        d.RoundResources[index].BlockTimer = time;
        d.RoundResources[index].TimerLock = false;
        GameManager.SetData<ResourceData>(player, d);
    }

}
