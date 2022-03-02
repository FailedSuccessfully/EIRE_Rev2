using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayController : GameSystem
{
    static VisualElement Root;
    public DisplayController(UIDocument mainDoc)
    {
        Root = mainDoc.rootVisualElement;
        DataType = typeof(DisplayData);
    }
    public void Init()
    {
        Button start = Root.Q<Button>("Start");
        start.clicked += () =>
        {
            BattleManager.StartBattle();
            Root.Q("Menu").style.display = DisplayStyle.None;
            Root.Q("Game").style.display = DisplayStyle.Flex;
        };
    }

    public void InitPlayers()
    {
        foreach (Player p in GameManager.Players)
        {
            VisualElement Container = GetPlayerContainer(p);
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
    public static void MarkWin(Player p)
    {
        VisualElement Container = GetPlayerContainer(p);
        VisualElement marker = Container.Q("Marker", "Unmarked");
        marker.RemoveFromClassList("Unmarked");
        marker.AddToClassList("Marker-Marked");
    }
    static VisualElement GetPlayerContainer(Player p)
    {
        return Root.Q(p.index == 0 ? "P1" : "P2");
    }
}
