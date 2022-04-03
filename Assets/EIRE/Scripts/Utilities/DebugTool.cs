using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugTool : MonoBehaviour
{
    public InputActionAsset DebugControls;


    void Awake()
    {
        Assign();
    }

    void Start()
    {
        DebugControls.Enable();
        DebugControls.actionMaps[0].Enable();

    }

    void Assign()
    {
        Color[] Col = { Color.blue, Color.red };
        DebugControls.actionMaps[0].FindAction("Color Players").performed += (ctx) =>
        {
            ctx.action.Disable();
            int colIndex = 0;
            foreach (Player p in GameManager.Players)
            {
                foreach (SpriteRenderer sr in p.Driver.GetComponentsInChildren<SpriteRenderer>())
                {
                    sr.color = sr.color == Color.white ? Col[colIndex] : Color.white;
                }
                colIndex++;
            }
            ctx.action.Enable();
        };
    }
}
