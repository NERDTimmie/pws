using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseOverlayButton : MonoBehaviour
{

    public Canvas OverlayCanvas;
 
    void Start()
    {
        var backButton = GetComponent<Button>();
        backButton.onClick.AddListener(OnClick);
    }
    
    void OnClick()
    {
        OverlayCanvas.enabled = false;
    }
}
