using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainHUD : MonoBehaviour
{
    public RectTransform FillBar;

    private float maximum;

    // Start is called before the first frame update
    void Start()
    {
        maximum = FillBar.anchorMax.x - FillBar.anchorMin.x;
        // Debug.Log(maximum);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerController.ActiveController != null)
        {
            FillBar.anchorMax = new Vector2(FillBar.anchorMin.x + ((PlayerController.ActiveController.Gas / PlayerController.ActiveController.MaxGas) * maximum), FillBar.anchorMax.y);
        }
    }
}
