using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : GameSystem
{

    public UIController()
    {
        DataType = typeof(UIData);
    }

    private UIDocument MainDoc;

    public override void SortConnections(List<Component> response)
    {
        MainDoc = response.OfType<UIDocument>().FirstOrDefault();
    }
    public void Initialize()
    {
        GameManager.RequestConnection(this, new System.Type[] { typeof(UIDocument) });
        Debug.Log(MainDoc ? "Success" : "Failure");
    }
}
