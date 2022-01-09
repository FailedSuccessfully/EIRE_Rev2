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
    GameSystem[] _systems;
    Dictionary<Player, GameData[]> _playerData;

    void Awake()
    {
        if (GameManager.Instance)
            Destroy(this);

        Instance = this;
        _systems = new GameSystem[0];
        _playerData = new Dictionary<Player, GameData[]>();
    }

    // Start is called before the first frame update
    void Start()
    {
        var p1 = new Player();
        Log(p1);
        var p2 = new Player();
        Log(p2);
        RegisterPlayer(p1);
        RegisterPlayer(p2);
        InputController ic = new InputController();
        RegisterSystem(ic);
        Log(ic.DataType);
        ic.InitPlayers();
        Log(GetPlayerData<InputData>(Players[0]));
    }

    // Update is called once per frame
    void Update()
    {

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
            if (!pd.Any(d => d.GetType() == s.DataType))
            {
                temp = new GameData[pd.Length + 1];
                pd.CopyTo(temp, 0);
                _playerData[p] = temp;
            }
        }
    }

    private void RegisterPlayer(Player p)
    {
        Log($"player is {p}");
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
    internal static GameData GetPlayerData<T>(Player p) where T : GameData
    {
        GameSystem s = Instance._systems.FirstOrDefault(sys => sys.DataType == typeof(T));
        if (s != null)
            return (Instance._playerData[p][SystemIndex(s)] as T);
        return null;
    }

    public static void Log(object msg) => Debug.Log(msg);
}
