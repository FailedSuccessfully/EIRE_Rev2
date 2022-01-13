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
        Destroy(this);
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
        }
    }

    public U Assign<T, U>(T context) where T : IDriveable where U : Driver<T>
    {
        GameObject obj = FetchFree();
        if (obj)
        {
            free--;
            obj.name += $" ~ {typeof(U).ToString()}";
            var comp = obj.AddComponent<U>().Mount(context);
            return comp as U;
        }
        return null;
    }
    public void Release<T>(Driver<T> driver) where T : IDriveable
    {
        driver.Unmount();
        driver.name = driver.name.Split(' ')[0];
        free++;
    }

    public Driver<T>[] Fetch<T>() where T : IDriveable
    {
        return poolContainer.GetComponentsInChildren<Driver<T>>();
    }
    public GameObject[] FetchAll() => pool;

    private GameObject FetchFree() => free == 0 ? null : pool[poolSize - free];
}