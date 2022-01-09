using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : IDriveable
{
    private static int counter = 0;
    public int index;

    public Player()
    {
        index = counter;
        counter++;
    }

    public void AcceptDriver(Driver<IDriveable> driver)
    {
        driver.Mount(this);
    }
}
