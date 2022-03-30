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
    Animator animator;
    Vector3 rigidDir => rigid.velocity;

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
        //pInput.actions.AddActionMap(inputs.Actions.Clone());
        pInput.actions = inputs.Default;
        pInput.SwitchCurrentControlScheme(pInput.actions.controlSchemes[0].name, Keyboard.current);
        Debug.Log(pInput.currentControlScheme);
        pInput.actions.actionMaps[0].Enable();
        pInput.actions.actionMaps[1].Enable();
        Puppet pup = AddSubDriver<Puppet>();
        animator = pup.GetComponentInChildren<Animator>();
        var move = pInput.actions.actionMaps[1].FindAction(PlayerActions.Move.ToString());
        var a = pInput.actions.actionMaps[0].FindAction(PlayerActions.B1.ToString());
        var dash = pInput.actions.actionMaps[1].FindAction(PlayerActions.Dash.ToString());

        AssignAction(move, null, ctx => Move(ctx.ReadValue<Vector2>()), ctx => Move(Vector3.zero));
        AssignAction(dash, onPerformed: ctx => Dash());
        AssignSpawnAction(a, charData.AttackProperties[0]);

        GameManager.SetData<InputData>(context, inputs);

        var shield = AddSubDriver<Shield>();
        var block = pInput.actions.actionMaps[0].FindAction(PlayerActions.Shield.ToString());
        AssignAction(block, null, ctx => Block(shield.gameObject, true), ctx => Block(shield.gameObject, false));

        target = GameManager.RequestTarget(context);
    }
    protected override void FixedUpdate()
    {
        charData.Speed = Vector3.ClampMagnitude(rigid.velocity + charData.Direction, charData.MaxSpeed) * charData.DashMult;
        charData.Speed *= 0.95f;
        rigid.velocity = charData.Speed;
    }

    protected override void Update()
    {
        base.Update();
        animator.SetFloat("VecX", rigidDir.normalized.x * subDrivers.OfType<Puppet>().First().transform.localScale.x);
        animator.SetFloat("VecY", rigidDir.normalized.y);
        animator.SetBool("Moving", charData.Direction.magnitude > 0 && rigidDir.magnitude > 1f);

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
        d.RoundResources[2].Rate *= active ? 0.05f : 20f;
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

    public void FlipX(bool flip) => subDrivers.OfType<Puppet>().First().transform.localScale = new Vector3(flip ? -1 : 1, 1, 1);

}
