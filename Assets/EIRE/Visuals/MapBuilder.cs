using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MapBuilder : MonoBehaviour
{
    public Texture2D Map;
    public GameObject tree, building;
    public Material treeMat, buildmat;
    // Start is called before the first frame update
    void Start()
    {

        tree.GetComponent<Renderer>().sharedMaterial = treeMat;
        building.GetComponent<Renderer>().sharedMaterial = buildmat;
        if (Map)
        {
            float x, z, w, h;
            x = 0;
            z = 0;
            w = Map.width;
            h = Map.height;
            float offsetw = w / 2 * -1;
            float offseth = h / 2 * -1;

            foreach (var tile in Map.GetPixels())
            {
                switch (tile)
                {

                    case ({ r: 0, g: 1, b: 0, a: 1 }):
                        {
                            Debug.Log("green");
                            var obj = GameObject.Instantiate(tree, transform);
                            obj.transform.localPosition = new Vector3((x + offsetw) / 2, 0, (z + offseth) / 2);
                            obj.transform.localScale = Vector3.one * 0.1f;
                            break;
                        }

                    case ({ r: 0, g: 0, b: 0, a: 1 }):
                        {
                            Debug.Log("black");
                            var obj = GameObject.Instantiate(building, transform);
                            obj.transform.localPosition = new Vector3((x + offsetw) / 2, 0, (z + offseth) / 2);
                            obj.transform.localScale = Vector3.one * 0.03f;
                            break;
                        }

                    default:
                        break;
                }
                x = (x + 1) % w;
                z += x == 0 ? 1 : 0;
            }

        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
