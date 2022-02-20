using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayController : GameSystem
{
    VisualElement Root;
    public DisplayController(UIDocument mainDoc)
    {
        Root = mainDoc.rootVisualElement;
        DataType = typeof(DisplayData);
    }

    public void InitPlayers()
    {
        foreach (Player p in GameManager.Players)
        {
            VisualElement Container;
            if (p.index == 0)
                Container = Root.Q("P1");
            else
                Container = Root.Q("P2");
            DisplayData d = new DisplayData();
            d.DisplayDictionary = new Dictionary<PlayerResource, VisualElement>();
            d.DisplayDictionary.Add(PlayerResource.Health, Container.Q("HP"));
            d.DisplayDictionary.Add(PlayerResource.Mana, Container.Q("MP"));

            GameManager.CreateData<DisplayData>(p, this);
            GameManager.SetData<DisplayData>(p, d);
        }
    }

    public override void onUpdate()
    {
        foreach (Player p in GameManager.Players)
        {
            DisplayData dd = GameManager.GetPlayerData<DisplayData>(p);
            ResourceData rd = GameManager.GetPlayerData<ResourceData>(p);

            float HPval = rd.RoundResources[(int)PlayerResource.Health].Current / rd.RoundResources[(int)PlayerResource.Health].Max;
            float MPval = rd.RoundResources[(int)PlayerResource.Mana].Current / rd.RoundResources[(int)PlayerResource.Mana].Max;
            dd.DisplayDictionary[PlayerResource.Health].Q(className: "Bar").style.width = Length.Percent(HPval * 100);
            dd.DisplayDictionary[PlayerResource.Mana].Q(className: "Bar").style.width = Length.Percent(MPval * 100);
        }
    }
}
