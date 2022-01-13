using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : GameSystem
{
    static float Radius = 0;
    static float bounceOffset = 0.5f;
    public float StageRadius => Radius;
    static Transform Stage;

    public BattleManager(Transform stage, float radius)
    {
        Stage = stage;
        Radius = radius;
    }

    public override void onFixedUpdate()
    {
        foreach (CharacterDriver driver in GameManager.GetDriversOfType<Player>())
        {
            if (!InsideStageBoundaries(driver.gameObject))
            {
                Debug.Log(Vector3.Distance(driver.transform.position, Stage.position));
                CharacterData d = GameManager.GetPlayerData<CharacterData>(driver.MountContext);
                d.Speed = BounceIn(driver.transform, d.Speed);
                GameManager.SetData<CharacterData>(driver.MountContext, d);
            }
        }
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
}
