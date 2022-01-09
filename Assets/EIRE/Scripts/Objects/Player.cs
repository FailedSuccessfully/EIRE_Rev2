using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private static int counter = 0;
    public int index;

    public Player()
    {
        index = counter;
        counter++;
    }
}
