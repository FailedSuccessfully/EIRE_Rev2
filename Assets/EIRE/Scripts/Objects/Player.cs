using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : IDriveable
{
    private static int counter = 0;
    public int index;
    private Driver<Player> myDriver;
    public CharacterDriver Driver => (CharacterDriver)myDriver;

    public Player()
    {
        index = counter;
        counter++;
    }

    public void AcceptDriver(GameObject driver)
    {

        if (driver.TryGetComponent<Driver<Player>>(out Driver<Player> driveComponent))
        {
            myDriver = driveComponent.Mount(this);
        }
    }
}
