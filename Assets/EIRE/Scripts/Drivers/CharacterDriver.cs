using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEditor;
using System;

public class CharacterDriver : Driver<Player>
{
    private GameData[] Data => GameManager.GetPlayerData(context);
    private CharacterData charData => Data.OfType<CharacterData>().First(); //(data => data.GetType() == typeof(CharacterData))
    SphereCollider hurtSphere;
    Rigidbody rigid;
    PlayerInput pInput;

    Transform target;
    public Transform Target => target;

    protected override void Awake()
    {
        hurtSphere = gameObject.AddComponent<SphereCollider>();
        hurtSphere.radius = transform.localScale.magnitude * 0.1f;
        rigid = gameObject.AddComponent<Rigidbody>();
        rigid.useGravity = false;
        rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        pInput = gameObject.AddComponent<PlayerInput>();
        pInput.actions = ScriptableObject.CreateInstance<InputActionAsset>();
        pInput.notificationBehavior = PlayerNotifications.InvokeUnityEvents;
        this.enabled = false;

        base.Awake();
    }

    void Start()
    {
        gameObject.layer = Data.OfType<BattleData>().First().layer;

        var inputs = Data.OfType<InputData>().First();
        pInput.actions.AddActionMap(inputs.Actions.Clone());
        pInput.currentActionMap = pInput.actions.actionMaps[0];
        AddSubDriver<Puppet>();
        var move = pInput.currentActionMap.FindAction(PlayerActions.Move.ToString());
        var a = pInput.currentActionMap.FindAction(PlayerActions.ButtonA.ToString());
        var dash = pInput.currentActionMap.FindAction(PlayerActions.Dash.ToString());

        AssignAction(move, null, ctx => Move(ctx.ReadValue<Vector2>()), ctx => Move(Vector3.zero));
        AssignAction(dash, onPerformed: ctx => Dash());
        AssignSpawnAction(a, charData.AttackProperties[0]);

        GameManager.SetData<InputData>(context, inputs);

        var shield = AddSubDriver<Shield>();
        var block = pInput.currentActionMap.FindAction(PlayerActions.Block.ToString());
        AssignAction(block, null, ctx => Block(shield.gameObject, true), ctx => Block(shield.gameObject, false));

        target = GameManager.RequestTarget(context);
    }
    protected override void FixedUpdate()
    {
        charData.Speed = Vector3.ClampMagnitude(rigid.velocity + charData.Direction, charData.MaxSpeed) * charData.DashMult;
        charData.Speed *= 0.95f;
        rigid.velocity = charData.Speed;
    }

    public void Move(Vector3 dir) => charData.Direction = dir;
    public void Dash()
    {
        charData.DashMult = 3;
        StartCoroutine(GameManager.ExecuteWithDelay(() => charData.DashMult = 1, 0.2f));
    }

    public void Block(GameObject shield, bool active)
    {
        var d = GameManager.GetPlayerData<ResourceData>(MountContext);
        d.RoundResources[2].RegenLock = !active;
        GameManager.SetData<ResourceData>(MountContext, d);
        shield.SetActive(active);
    }

    private void AssignAction(InputAction action, Action<InputAction.CallbackContext> onStarted = null,
                                                    Action<InputAction.CallbackContext> onPerformed = null,
                                                    Action<InputAction.CallbackContext> onCanceled = null)
    {
        if (!(onStarted is null))
            action.started += onStarted;
        if (!(onPerformed is null))
            action.performed += onPerformed;
        if (!(onCanceled is null))
            action.canceled += onCanceled;
    }

    private void AssignSpawnAction(InputAction action, AttackProps attackProps)
    {
        SpawnStrategy.SpawnTable[attackProps.spawnStrat].SetSpawn(action, attackProps, charData.playerIndex);
    }

    void OnCollisionEnter(Collision collision)
    {
    }

    public void AcceptBounce(Vector3 bounce)
    {
        rigid.velocity = bounce;
    }

    public void FlipX(bool isFlip) => subDrivers.OfType<Puppet>().FirstOrDefault()?.FlipX(isFlip);
}
