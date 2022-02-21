// WAIT - no timer
// READY - timer until start FIGHT
// FIGHT - Timer 99, also ends on HPZero
// ROUND - check if go back to READY or go to WIN
// WIN - fight ends, go to WAIT

public abstract class BattlePhase
{
    protected float time;
    public float Time => time;
    public static BattlePhase Next;
    public virtual void SetPhase() { }
    public virtual void EndPhase()
    {
    }
    public virtual BattlePhase MovePhase()
    {
        BattlePhase phase = Next;
        phase.SetPhase();
        return phase;
    }

}

public class Wait : BattlePhase
{

    public override void SetPhase()
    {
        // player action maps disabled
        Next = new Ready();
        time = 9999f;
        base.SetPhase();
    }
}
public class Ready : BattlePhase
{
    public override void SetPhase()
    {
        // set positions
        // set resources
        // movement enabled
        // actions unenabled
        Next = new Fight();
        time = 4f;
        base.SetPhase();
    }
}
public class Fight : BattlePhase
{

    public override void SetPhase()
    {
        // actions enabled
        Next = new Round();
        time = 99f;
        base.SetPhase();
    }
}
public class Round : BattlePhase
{

    public override void SetPhase()
    {
        // player actions disabled
        EventsManager.RegisterToEvent(GameEvent.EnoughWins, () => Next = new Win());
        Next = new Ready();
        time = 5f;
        base.SetPhase();
    }

    public override void EndPhase()
    {
        UnityEngine.Debug.LogWarning("Performing win check on hard coded 0");
        EventsManager.CheckEvent(GameEvent.EnoughWins, 0 /*TODO: put actual value in*/);
        base.EndPhase();
    }
}
public class Win : BattlePhase
{
    public override void SetPhase()
    {
        // init battle data
        Next = new Wait();
        time = 5f;
        base.SetPhase();
    }
}