using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IDriveable
{
    public void AcceptDriver(Driver<IDriveable> driver);
}


public abstract class Driver<T> : MonoBehaviour where T : IDriveable
{
    protected T context;
    public Type DriverType => typeof(T);
    public T MountContext => context;

    private void Start()
    {
        Debug.Log($"start {gameObject.name}");
    }

    public virtual Driver<T> Mount(T ctx)
    {
        context = ctx;
        enabled = true;
        return this;
    }
    public virtual void Unmount()
    {
        enabled = false;
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

    public U Assign<T, U>(T context) where T : IDriveable where U : Driver<T>
    {
        GameObject obj = FetchFree();
        if (obj)
        {
            free--;
            obj.name += $" ~ {typeof(U).ToString()}";
            if (!obj.TryGetComponent<U>(out U comp))
            {
                foreach (var toRemove in obj.GetComponents<Driver<IDriveable>>())
                {
                    GameObject.Destroy(toRemove);
                }
                comp = obj.AddComponent<U>();
            }
            obj.SetActive(true);
            return comp.Mount(context) as U;
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