using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEditor;

public class CharacterDriver : Driver<Player>
{
    private GameData[] Data => GameManager.GetPlayerData(context);
    private CharacterData charData => Data.OfType<CharacterData>().First(); //(data => data.GetType() == typeof(CharacterData))
    SpriteRenderer spr;
    BoxCollider2D hurtBox;
    PlayerInput pInput;

    private void Awake()
    {
        spr = gameObject.AddComponent<SpriteRenderer>();
        spr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        hurtBox = gameObject.AddComponent<BoxCollider2D>();
        hurtBox.size = Vector2.one;
        pInput = gameObject.AddComponent<PlayerInput>();
        pInput.actions = new InputActionAsset();
        this.enabled = false;
    }

    void Start()
    {
        var inputs = Data.OfType<InputData>().First();
        var move = inputs.Actions.FindAction(PlayerActions.Move.ToString());
        pInput.actions.AddActionMap(inputs.Actions.Clone());
        pInput.currentActionMap = pInput.actions.actionMaps[0];
        pInput.currentActionMap.actions[0].performed += ctx => Move(ctx.ReadValue<Vector2>());
        pInput.currentActionMap.actions[0].canceled += ctx => Move(Vector3.zero);
        GameManager.SetData<InputData>(context, inputs);
    }

    void FixedUpdate()
    {
        charData.Speed = Vector3.ClampMagnitude(charData.Speed + charData.Direction, charData.MaxSpeed);
        transform.position += (charData.Speed * charData.BaseSpeed * Constants.Speed.Coefficient);
        charData.Speed *= 0.95f;
    }

    public void Move(Vector3 dir) => charData.Direction = dir;
}
