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
    static CharacterDriver Player1, Player2;
    static BattlePhase Phase;
    static Camera cam => Camera.main;
    static Vector3 velocity, target;

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

    public override void InitPlayers()
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

    public override void OnStart()
    {
        base.OnStart();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        MaintainCamera();
    }

    public override void OnFixedUpdate()
    {
        EventsManager.CheckEvent(GameEvent.SwitchDirections, -1 * (Player1.transform.position - Player2.transform.position).x);
        CalculateCamera();
        foreach (CharacterDriver driver in GameManager.GetDriversOfType<Player>())
        {
            if (!InsideStageBoundaries(driver.gameObject))
            {
                CharacterData d = GameManager.GetPlayerData<CharacterData>(driver.MountContext);
                driver.AcceptBounce(BounceIn(driver.transform, d.Speed));
                GameManager.SetData<CharacterData>(driver.MountContext, d);
            }
        }

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

    public static SpellDriver RequestSpell(SpellData data, int playerIndex)
    {
        SpellDriver driver = GameManager.RequestDriver<SpellData, SpellDriver>(data);
        if (driver)
        {
            data.AcceptDriver(driver.gameObject);
            driver.target = GameManager.Players[playerIndex].Driver.Target.position;
            driver.transform.position = GameManager.Players[playerIndex].Driver.transform.position;
            driver.gameObject.layer = GameManager.Players[playerIndex].Driver.Target.gameObject.layer;
        }
        return driver;
    }

    public static void RequestRelease(Driver<AttackProps> driver) => GameManager.UnmountDriver(driver);
    static void CalculateCamera()
    {
        Vector3 midPlayers = Vector3.Lerp(Player1.transform.position, Player2.transform.position, 0.5f);
        float distance = Vector3.Distance(Player1.transform.position, Player2.transform.position);
        Vector3 LerpX = Vector3.right * Mathf.Lerp(-20, 20, midPlayers.x / 40 + 0.5f);
        Vector3 LerpY = Vector3.up * Mathf.Lerp(-20, 20, midPlayers.y / 40 + 0.5f);
        Vector3 LerpZ = Vector3.forward * Mathf.Lerp(-40, -120, distance / 80);
        target = LerpX + LerpY + LerpZ;
    }
    static void MaintainCamera()
    {
        cam.transform.localPosition = Vector3.SmoothDamp(cam.transform.localPosition, target, ref velocity, 0.1f);
    }
    public static void StartBattle()
    {
        if (Phase is Wait)
        {
            EventsManager.ForceInvoke(GameEvent.TimerZero);
        }
    }
}