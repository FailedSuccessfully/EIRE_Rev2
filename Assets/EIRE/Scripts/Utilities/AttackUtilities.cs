using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AttackUtilities
{
    public static class Movement
    {
        public static void MoveForward(Transform transform, float speed, Vector2 direction)
        {
            transform.Translate((Vector2)direction * speed * Time.deltaTime);
        }
    }
    public static class Targeting { }
}
