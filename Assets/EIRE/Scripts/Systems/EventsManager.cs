using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameEvent
{
    HPZero,
    TimerZero,
    EnoughWins,
    SwitchDirections
}
public class EventsManager : GameSystem
{
    static float distance = 0;
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
        Checks[GameEvent.SwitchDirections] = distance =>
        {
            bool isSwitch = Mathf.Sign(distance) != Mathf.Sign(EventsManager.distance);
            Debug.Log($"{distance} {EventsManager.distance}");
            EventsManager.distance = distance;
            return isSwitch;
        };
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
        bool result = Checks[ev].Invoke(value);
        if (result)
            Events[ev].Invoke();
        return result;
    }
    internal static void ForceInvoke(GameEvent ev)
    {
        Events[ev].Invoke();
    }
}
