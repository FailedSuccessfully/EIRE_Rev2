using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IDriveable
{
    public void AcceptDriver(GameObject driver);
}


public abstract class Driver<T> : MonoBehaviour where T : IDriveable
{
    protected T context;
    public Type DriverType => typeof(T);
    public T MountContext => context;
    public delegate void Hook();
    internal Hook OnUpdate, OnFixedUpdate, OnEnableHook, OnDisableHook;
    protected List<SubDriver> subDrivers;

    protected virtual void Awake()
    {
        subDrivers = new List<SubDriver>();
    }
    private void Start()
    {
    }
    protected virtual void Update()
    {
        OnUpdate?.Invoke();
    }

    protected virtual void FixedUpdate()
    {
        OnFixedUpdate?.Invoke();
    }
    protected virtual void OnEnable()
    {
        OnEnableHook?.Invoke();
    }
    protected virtual void OnDisable()
    {
        OnDisableHook?.Invoke();
    }
    public virtual Driver<T> Mount(T ctx)
    {
        this.enabled = true;
        context = ctx;
        return this;
    }
    public virtual void Unmount()
    {
        enabled = false;
    }

    protected U AddSubDriver<U>() where U: SubDriver{
        U sub = new GameObject($"SubDriver ~ {typeof(U).ToString()}").AddComponent<U>();
        
        sub.SetContext(gameObject);
        subDrivers.Add(sub);
        return sub;
    }

}

public class DriverPool
{
    int poolSize, free;
    GameObject[] pool;
    GameObject poolContainer;

    public DriverPool(int size)
    {
        poolSize = size;
        free = size;
        pool = new GameObject[size];
        poolContainer = new GameObject("Driver Pool");
        for (int i = 0; i < size; i++)
        {
            pool[i] = new GameObject($"Driver-{i}");
            pool[i].transform.parent = poolContainer.transform;
            pool[i].SetActive(false);
        }
    }

    //TODO: Format this to be consistent with similar generic functions
    public T Request<T, U>(bool setActive) where T : Driver<U> where U : IDriveable
    {
        GameObject obj = FetchFree();
        if (obj)
        {
            free--;
            obj.name += $" ~ {typeof(T).ToString()}";
            if (!obj.TryGetComponent<T>(out T comp))
            {
                foreach (var toRemove in obj.GetComponents<Driver<IDriveable>>())
                {
                    GameObject.Destroy(toRemove);
                }
                comp = obj.AddComponent<T>();
            }
            obj.SetActive(setActive);
            return comp;
        }
        return null;
    }
    public void Release<T>(Driver<T> driver) where T : IDriveable
    {
        driver.Unmount();
        driver.name = driver.name.Split(' ')[0];
        driver.gameObject.SetActive(false);
        var nPool = new GameObject[poolSize];
        pool = pool.OrderBy(obj => obj.activeSelf == true).ToArray();
        free = pool.Count(obj => !obj.activeSelf);
    }

    public Driver<T>[] Fetch<T>() where T : IDriveable
    {
        return poolContainer.GetComponentsInChildren<Driver<T>>();
    }
    public GameObject[] FetchAll() => pool;

    private GameObject FetchFree() => pool.FirstOrDefault(obj => !obj.activeSelf);
    private GameObject FetchExistingOrFree<T>(T context) where T : IDriveable
    {
        Driver<T> driver = Fetch<T>().Where(driver => !driver.enabled)
                                    .FirstOrDefault(driver => driver.MountContext.Equals(context));
        return driver ? driver.gameObject : FetchFree();
    }
}