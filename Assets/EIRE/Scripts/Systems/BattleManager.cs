using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : GameSystem
{
    static float phaseTimer;
    static float Radius = 0;
    static float bounceOffset = 0.5f;
    public float StageRadius => Radius;
    static Transform Stage;
    CharacterDriver Player1, Player2;
    static BattlePhase Phase;
    Camera cam => Camera.main;

    public BattleManager(Transform stage, float radius)
    {
        DataType = typeof(BattleData);
        Stage = stage;
        Radius = radius;
        Phase = new Wait();
        Phase.SetPhase();
        phaseTimer = Time.fixedTime + Phase.Time;
        EventsManager.RegisterToEvent(GameEvent.TimerZero, () => { Phase.EndPhase(); Phase = Phase.MovePhase(); phaseTimer = Time.fixedTime + Phase.Time; });
    }

    public void InitPlayers()
    {
        BattleData P1, P2;
        P1 = new BattleData()
        {
            layer = LayerMask.NameToLayer("P1"),
            wins = 0
        };
        P2 = new BattleData()
        {
            layer = LayerMask.NameToLayer("P2"),
            wins = 0
        };
        GameManager.CreateData<BattleData>(GameManager.Players[0], this);
        GameManager.SetData<BattleData>(GameManager.Players[0], P1);
        Player1 = GameManager.GetDriversOfType<Player>()[0] as CharacterDriver;
        GameManager.CreateData<BattleData>(GameManager.Players[1], this);
        GameManager.SetData<BattleData>(GameManager.Players[1], P2);
        Player2 = GameManager.GetDriversOfType<Player>()[1] as CharacterDriver;
    }

    public override void OnFixedUpdate()
    {
        foreach (CharacterDriver driver in GameManager.GetDriversOfType<Player>())
        {
            if (!InsideStageBoundaries(driver.gameObject))
            {
                CharacterData d = GameManager.GetPlayerData<CharacterData>(driver.MountContext);
                driver.AcceptBounce(BounceIn(driver.transform, d.Speed));
                GameManager.SetData<CharacterData>(driver.MountContext, d);
            }
        }

        bool isFlip = Player1.transform.position.x - Player2.transform.position.x <= 0;
        Player1.FlipX(isFlip);
        Player2.FlipX(!isFlip);
        foreach (AttackDriver driver in GameManager.GetDriversOfType<AttackProps>().Where(driver => driver.enabled))
        {
            driver.transform.position = MoveStrategy.MoveTable[driver.MountContext.moveStrat].Move(driver);
            if (driver.isTTL) RequestRelease(driver as Driver<AttackProps>);
        }
        if (EventsManager.CheckEvent(GameEvent.TimerZero, phaseTimer - Time.fixedTime))
            Debug.Log(Phase.GetType().ToString());
    }

    public static bool InsideStageBoundaries(GameObject toCheck, Vector3 stagePosition) => Vector3.Distance(toCheck.transform.position, stagePosition) <= Radius - bounceOffset;
    public static bool InsideStageBoundaries(GameObject toCheck) => Stage ? InsideStageBoundaries(toCheck, Stage.position) : false;
    public void SetStage(Transform stage) => Stage = stage;
    public static Vector3 BounceIn(Transform toBounce, Vector3 movement)
    {
        Vector3 orgToObj = (toBounce.position - Stage.position) * Radius / Vector3.Distance(toBounce.position, Stage.position);
        Vector3 reflection = Vector3.Reflect(movement, (Stage.position + toBounce.position).normalized);
        return reflection * 2f;
    }

    public static AttackDriver RequestAttack(AttackProps attackProps, int playerIndex)
    {
        var driver = GameManager.RequestDriver<AttackProps, AttackDriver>(attackProps);
        if (driver)
        {
            attackProps.AcceptDriver(driver.gameObject);
            driver.transform.position = GameManager.Players[playerIndex].Driver.transform.position;

            // Set targeting
            TargetStrategy.TargetTable[attackProps.targetStrat].SetTargeting(driver, GameManager.Players[playerIndex].Driver.Target);

            // Set layer
            driver.gameObject.layer = GameManager.Players[playerIndex].Driver.Target.gameObject.layer;
        }
        return driver;
    }
    public static void RequestRelease(Driver<AttackProps> driver) => GameManager.UnmountDriver(driver);
}