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
    SpriteRenderer spr;
    SphereCollider hurtSphere;
    Rigidbody rigid;
    PlayerInput pInput;

    Transform target;
    public Transform Target => target;

    private void Awake()
    {
        spr = gameObject.AddComponent<SpriteRenderer>();
        spr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        hurtSphere = gameObject.AddComponent<SphereCollider>();
        hurtSphere.radius = transform.localScale.magnitude * 0.1f;
        rigid = gameObject.AddComponent<Rigidbody>();
        rigid.useGravity = false;
        rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        pInput = gameObject.AddComponent<PlayerInput>();
        pInput.actions = ScriptableObject.CreateInstance<InputActionAsset>();
        pInput.notificationBehavior = PlayerNotifications.InvokeUnityEvents;
        this.enabled = false;
    }

    void Start()
    {
        gameObject.layer = Data.OfType<BattleData>().First().layer;

        var inputs = Data.OfType<InputData>().First();
        pInput.actions.AddActionMap(inputs.Actions.Clone());
        pInput.currentActionMap = pInput.actions.actionMaps[0];
        var move = pInput.currentActionMap.FindAction(PlayerActions.Move.ToString());
        var a = pInput.currentActionMap.FindAction(PlayerActions.ButtonA.ToString());
        //var test = pInput.currentActionMap.FindAction("Tester");

        AssignAction(move, null, ctx => Move(ctx.ReadValue<Vector2>()), ctx => Move(Vector3.zero));
        AssignSpawnAction(a, charData.AttackProperties[0]);
        //if (context.index == 0)
        //    AssignSpawnAction(test, charData.AttackProperties[0]);

        GameManager.SetData<InputData>(context, inputs);

        target = GameManager.RequestTarget(context);
    }
    protected override void FixedUpdate()
    {
        charData.Speed = Vector3.ClampMagnitude(rigid.velocity + charData.Direction, charData.MaxSpeed);
        //transform.position += (charData.Speed * charData.BaseSpeed * Constants.Speed.Coefficient);
        charData.Speed *= 0.95f;
        rigid.velocity = charData.Speed;
    }

    public void Move(Vector3 dir) => charData.Direction = dir;

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
        Debug.Log(collision.gameObject.name);
    }

    public void AcceptBounce(Vector3 bounce)
    {
        rigid.velocity = bounce;
    }
}
