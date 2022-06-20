using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public CharacterInfo[] characters;
    public static Player[] Players => Instance._playerData.Keys.ToArray();
    private static DriverPool driverPool;
    public static GameObject[] Drivers => driverPool.FetchAll();
    public static Driver<T>[] GetDriversOfType<T>() where T : IDriveable => driverPool.Fetch<T>();
    GameSystem[] _systems;
    Dictionary<Player, GameData[]> _playerData;
    UnityAction onUpdate, onFixedUpdate, onStart;
    CinemachineTargetGroup cameraTarget;

    #region  temporary editor properties
    public GameObject GameWorld_temp;
    #endregion

    void Awake()
    {
        if (GameManager.Instance)
            Destroy(this);

        Instance = this;
        driverPool = new DriverPool(64);
        _systems = new GameSystem[0];
        _playerData = new Dictionary<Player, GameData[]>();
    }

    // Start is called before the first frame update
    void Start()
    {
        cameraTarget = GameWorld_temp.GetComponentInChildren<CinemachineTargetGroup>();

        var p1 = new Player();
        var p2 = new Player();
        RegisterPlayer(p1);
        RegisterPlayer(p2);

        RegisterSystem(new EventsManager());
        RegisterSystem(new InputController());
        RegisterSystem(new CharacterManager());
        RegisterSystem(new BattleManager(GameWorld_temp.transform, (80 * 2) / 5f));
        RegisterSystem(new ResourceManager());

        //        DisplayController dc = new DisplayController(GetComponent<UnityEngine.UIElements.UIDocument>());
        //      RegisterSystem(dc);
        //     dc.Init();
        //   dc.InitPlayers();

        var d1 = driverPool.Request<Player, CharacterDriver>(true).gameObject;
        d1.transform.position += Vector3.left * 5;
        p1.AcceptDriver(d1);
        var d2 = driverPool.Request<Player, CharacterDriver>(true).gameObject;
        d2.transform.position += Vector3.right * 5;
        p2.AcceptDriver(d2);
        cameraTarget.AddMember(d1.transform, 1, 0);
        cameraTarget.AddMember(d2.transform, 1, 0);

        foreach (GameSystem sys in _systems)
            sys?.InitPlayers();

        onStart.Invoke();
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
    }

    #region Unity Callbacks
    void Update() => onUpdate?.Invoke();

    void FixedUpdate() => onFixedUpdate?.Invoke();

    #endregion

    #region  System Core Management
    private void RegisterSystem(GameSystem s)
    {
        // Add system to array
        GameSystem[] gsa = new GameSystem[_systems.Length + 1];
        _systems.CopyTo(gsa, 0);
        gsa[_systems.Length] = s;
        _systems = gsa;

        // Define player data
        GameData[] temp;
        foreach (Player p in Players)
        {
            var pd = _playerData[p];

            // Validate no duplicate data types
            if (!pd.Any(d => d?.GetType() == s.DataType))
            {
                temp = new GameData[pd.Length + 1];
                pd.CopyTo(temp, 0);
                _playerData[p] = temp;
            }
        }

        // Subscribe to events
        onUpdate += s.OnUpdate;
        onFixedUpdate += s.OnFixedUpdate;
        onStart += s.OnStart;
    }

    private void RegisterPlayer(Player p) => _playerData.Add(p, new GameData[_systems.Length]);

    internal static GameData CreateData<T>(Player p, GameSystem s) where T : GameData
    {
        int i = SystemIndex(s);
        if (i >= 0)
            return Instance._playerData[p][SystemIndex(s)] = Activator.CreateInstance<T>();
        return null;
    }

    internal static void SetData<T>(Player p, T data) where T : GameData
    {
        GameSystem s = Instance._systems.FirstOrDefault(sys => sys.DataType == typeof(T));
        if (s != null)
            Instance._playerData[p][SystemIndex(s)] = data;
    }

    internal static T GetPlayerData<T>(Player p) where T : GameData
    {
        GameSystem s = Instance._systems.FirstOrDefault(sys => sys.DataType == typeof(T));
        if (s != null)
            return (Instance._playerData[p][SystemIndex(s)] as T);
        return null;
    }
    internal static T[] GetSystemData<T>() where T : GameData
    {
        List<T> list = new List<T>();
        int sysIndex = SystemIndex(Instance._systems.First(sys => sys.DataType == typeof(T)));
        foreach (Player p in Players)
        {
            list.Add(GetPlayerData<T>(p));
        }
        return list.ToArray();
    }

    #endregion

    #region System Internal Utilities
    private static int SystemIndex(GameSystem s) => Array.IndexOf(Instance._systems, s);
    internal static U RequestDriver<T, U>(T context) where T : IDriveable where U : Driver<T> => driverPool.Request<T, U>(false);
    internal static void UnmountDriver<T>(Driver<T> driver) where T : IDriveable => driverPool.Release(driver);
    internal static GameData[] GetPlayerData(Player p) => Instance._playerData[p];

    public static Transform RequestTarget(Player requestor)
    {
        // DO NOTE: this is very stiff and wants there to be 2 players
        int targetIndex = Math.Abs(1 - requestor.index); // This only works for 2 players
        return Players[targetIndex].Driver.transform;
    }
    public static IEnumerator ExecuteWithDelay(UnityAction action, float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        action.Invoke();
        yield return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (_systems != null && _systems.OfType<BattleManager>().Count() > 0)
            Gizmos.DrawWireSphere(GameWorld_temp.transform.position, _systems.OfType<BattleManager>().First().StageRadius);
    }
    #endregion
}
