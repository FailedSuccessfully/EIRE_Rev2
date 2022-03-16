using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.AddressableAssets;
public enum PlayerActions
{
    Move,
    B1,
    B2,
    B3,
    Dash,
    Shield
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
        var inputs = Addressables.LoadAssetsAsync<InputActionAsset>("Input", asset =>
        {
        });
        inputs.WaitForCompletion();

        P1 = new InputData() { Custom = inputs.Result[0], Default = inputs.Result[1] };
        P2 = new InputData() { Custom = inputs.Result[2], Default = inputs.Result[3] };
        GameManager.CreateData<InputData>(GameManager.Players[0], this);
        GameManager.SetData<InputData>(GameManager.Players[0], P1);
        GameManager.CreateData<InputData>(GameManager.Players[1], this);
        GameManager.SetData<InputData>(GameManager.Players[1], P2);
    }
}