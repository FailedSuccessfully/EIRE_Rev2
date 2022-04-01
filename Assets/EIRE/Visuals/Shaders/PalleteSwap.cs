using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PalleteSwap : MonoBehaviour
{
    public Sprite MainColor, SwapColor;

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        foreach (Color c in MainColor.texture.GetPixels())
        {
            Debug.Log($"{++i}: {c.ToString()} ");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
