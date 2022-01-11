using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.AddressableAssets;
public enum PlayerActions
{
    Move,
    ButtonA,
    ButtonB,
    ButtonC,
    Dash,
    Block
}

public class InputController : GameSystem
{
    public InputController()
    {
        DataType = typeof(InputData);
    }

    public void InitPlayers()
    {
        InputData P1, P2;
        var m = Addressables.LoadAssetAsync<InputActionAsset>("Input/Default");
        m.WaitForCompletion();
        P1 = new InputData() { Actions = m.Result.FindActionMap("Player 1") };
        P2 = new InputData() { Actions = m.Result.FindActionMap("Player 2") };
        GameManager.CreateData<InputData>(GameManager.Players[0], this);
        GameManager.SetData<InputData>(GameManager.Players[0], P1);
        //GameManager.SetData<InputData>(GameManager.Players[1], P2);
    }
}