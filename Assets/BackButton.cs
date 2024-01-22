using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class BackButton : MonoBehaviour
{

    

    void Start()
    {   
        var backButton = GetComponent<Button>();
        backButton.onClick.AddListener(OnClick);
    }
    
    void OnClick()
    {
        SceneManager.LoadScene("Ui");
    }

    
}
