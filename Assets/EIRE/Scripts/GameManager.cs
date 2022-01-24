using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static Player[] Players => Instance._playerData.Keys.ToArray();
    private static DriverPool driverPool;
    public static GameObject[] Drivers => driverPool.FetchAll();
    public static Driver<T>[] GetDriversOfType<T>() where T : IDriveable => driverPool.Fetch<T>();
    GameSystem[] _systems;
    Dictionary<Player, GameData[]> _playerData;

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

        var p1 = new Player();
        var p2 = new Player();
        RegisterPlayer(p1);
        RegisterPlayer(p2);
        InputController ic = new InputController();
        RegisterSystem(ic);
        ic.InitPlayers();
        CharacterManager cm = new CharacterManager();
        RegisterSystem(cm);
        cm.InitPlayers();
        BattleManager bm = new BattleManager(GameWorld_temp.transform, (20 * 2) / 5f);
        RegisterSystem(bm);
        var d1 = driverPool.Request<CharacterDriver, Player>(true).gameObject;
        p1.AcceptDriver(d1);
        var d2 = driverPool.Request<CharacterDriver, Player>(true).gameObject;
        p2.AcceptDriver(d2);

        Log("beb");
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        _systems[2].onFixedUpdate();
    }

    private static int SystemIndex(GameSystem s) => Array.IndexOf(Instance._systems, s);

    private void RegisterSystem(GameSystem s)
    {
        //TODO: ensure update on all players game data array
        GameData[] temp;
        GameSystem[] gsa = new GameSystem[_systems.Length + 1];
        _systems.CopyTo(gsa, 0);
        gsa[_systems.Length] = s;
        _systems = gsa;

        foreach (Player p in Players)
        {
            var pd = _playerData[p];
            if (!pd.Any(d => d?.GetType() == s.DataType))
            {
                temp = new GameData[pd.Length + 1];
                pd.CopyTo(temp, 0);
                _playerData[p] = temp;
            }
        }
    }

    private void RegisterPlayer(Player p)
    {
        _playerData.Add(p, new GameData[_systems.Length]);
    }

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

    internal static GameData[] GetPlayerData(Player p) => Instance._playerData[p];
    internal static T GetPlayerData<T>(Player p) where T : GameData
    {
        GameSystem s = Instance._systems.FirstOrDefault(sys => sys.DataType == typeof(T));
        if (s != null)
            return (Instance._playerData[p][SystemIndex(s)] as T);
        return null;
    }

    internal static U RequestDriver<T, U>(T context) where T : IDriveable where U : Driver<T> => driverPool.Request<U, T>(false);
    internal static void UnmountDriver<T>(Driver<T> driver) where T : IDriveable => driverPool.Release(driver);

    public static Transform RequestTarget(Player requestor)
    {
        // DO NOTE: this is very stiff and wants there to be 2 players
        int targetIndex = Math.Abs(1 - requestor.index); // This only works for 2 players
        return Players[targetIndex].Driver.transform;
    }
    public static void Log(object msg) => Debug.Log(msg);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (_systems != null && _systems.OfType<BattleManager>().Count() > 0)
            Gizmos.DrawWireSphere(GameWorld_temp.transform.position, _systems.OfType<BattleManager>().First().StageRadius);
    }
}
