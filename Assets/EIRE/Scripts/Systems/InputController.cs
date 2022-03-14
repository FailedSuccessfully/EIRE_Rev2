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
        var inputs = Addressables.LoadAssetsAsync<InputActionAsset>("Input", asset =>
        {
            UnityEngine.Debug.Log(asset.name);
        });
        inputs.WaitForCompletion();

        m.WaitForCompletion();
        P1 = new InputData() { Actions = m.Result.FindActionMap("Player 1"), Custom = inputs.Result[0], Default = inputs.Result[1] };
        P2 = new InputData() { Actions = m.Result.FindActionMap("Player 2"), Custom = inputs.Result[2], Default = inputs.Result[3] };
        GameManager.CreateData<InputData>(GameManager.Players[0], this);
        GameManager.SetData<InputData>(GameManager.Players[0], P1);
        GameManager.CreateData<InputData>(GameManager.Players[1], this);
        GameManager.SetData<InputData>(GameManager.Players[1], P2);
    }
}