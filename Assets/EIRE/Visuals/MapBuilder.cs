using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;

public class MapBuilder : MonoBehaviour
{
    public Texture2D Scheme;
    private static GameObject Map;
    public GameObject tree, building;
    public Material treeMat, buildmat;
    // Start is called before the first frame update
    void Start()
    {

        tree.GetComponent<Renderer>().sharedMaterial = treeMat;
        building.GetComponent<Renderer>().sharedMaterial = buildmat;
        if (Scheme)
        {
            Generate();

        }

    }

    public void Generate()
    {
        if (Map)
            Clear();
        Map = new GameObject("Map");
        Map.transform.parent = transform;
        Map.transform.localPosition = Vector3.zero;
        Map.transform.localScale = Vector3.one;
        float x, z, w, h;
        x = 0;
        z = 0;
        w = Scheme.width;
        h = Scheme.height;
        float offsetw = w / 2 * -1;
        float offseth = h / 2 * -1;

        foreach (var tile in Scheme.GetPixels())
        {
            switch (tile)
            {

                case ({ r: 0, g: 1, b: 0, a: 1 }):
                    {
                        Debug.Log("green");
                        var obj = GameObject.Instantiate(tree, Map.transform);
                        obj.transform.localPosition = new Vector3((x + offsetw) / 2, 0, (z + offseth) / 2);
                        obj.transform.localScale = Vector3.one * 0.1f;
                        break;
                    }

                case ({ r: 0, g: 0, b: 0, a: 1 }):
                    {
                        Debug.Log("black");
                        var obj = GameObject.Instantiate(building, Map.transform);
                        obj.transform.localPosition = new Vector3((x + offsetw) / 2, 0, (z + offseth) / 2);
                        obj.transform.localScale = new Vector3(0.02f, 0.03f, 0.02f);
                        break;
                    }

                default:
                    break;
            }
            x = (x + 1) % w;
            z += x == 0 ? 1 : 0;
        }
    }

    public static void Clear() => GameObject.DestroyImmediate(Map);
}

[CustomEditor(typeof(MapBuilder))]
public class MapBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MapBuilder builder = (MapBuilder)target;

        if (GUILayout.Button("Generate"))
            builder.Generate();
        if (GUILayout.Button("Clear"))
            MapBuilder.Clear();
    }
}