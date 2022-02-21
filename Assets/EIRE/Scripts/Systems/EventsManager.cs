using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameEvent
{
    HPZero,
    TimerZero,
    EnoughWins
}
public class EventsManager : GameSystem
{
    public static Dictionary<GameEvent, UnityEvent> Events;
    private static Dictionary<GameEvent, System.Func<float, bool>> Checks;

    public EventsManager()
    {
        // take heed that this might lead to unexpected results
        Events = Events is null ? new Dictionary<GameEvent, UnityEvent>() : Events;
        Checks = Checks is null ? new Dictionary<GameEvent, System.Func<float, bool>>() : Checks;
        Init();
    }

    private void Init()
    {
        foreach (GameEvent ev in System.Enum.GetValues(typeof(GameEvent)))
        {
            Events.Add(ev, new UnityEvent());
        }

        Checks[GameEvent.HPZero] = hp => hp <= 0;
        Checks[GameEvent.TimerZero] = timer => timer <= 0;
        Checks[GameEvent.EnoughWins] = wins => wins == 2;
    }

    public static void RegisterToEvent(GameEvent ev, UnityAction func)
    {
        Events[ev].AddListener(func);
    }

    public static void DeregisterEvent(GameEvent ev, UnityAction func)
    {
        Events[ev].RemoveListener(func);
    }

    internal static bool CheckEvent(GameEvent ev, float value)
    {
        bool res = Checks[ev].Invoke(value);
        if (res)
            Events[ev].Invoke();
        return res;
    }
    internal static void ForceInvoke(GameEvent ev){
        Events[ev].Invoke();
    }
}
