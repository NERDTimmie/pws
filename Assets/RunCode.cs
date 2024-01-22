using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;


public class RunCode : MonoBehaviour
{

    public Button runButton;
    public TMP_InputField runServer, code;
    public Canvas errorOverlayCanvas, lessonOverlayCanvas;
    public TMP_Text errorOverlayText;

    // Start is called before the first frame update
    void Start()
    {
        runButton.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClick()
    {
        StartCoroutine(postRequest(runServer.text, code.text));        
    }

    IEnumerator postRequest(string uri, string code)
    {
        runButton.interactable = false;
        lessonOverlayCanvas.enabled = false;

        var uwr = new UnityWebRequest(uri, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(code);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "text/plain");
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
            runButton.interactable = true;
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);

            if (uwr.responseCode == 200) {
                var world_raw = uwr.downloadHandler.text;

                WorldData worldData = JsonConvert.DeserializeObject<WorldData>(world_raw);

                DataStorage dataStorage = FindObjectOfType<DataStorage>();

                if(dataStorage != null){
                    DataManager.SetWorldData(worldData.world);
                    DataManager.SetCode(code);
                    SceneManager.LoadScene("output");
                    runButton.interactable = true;
                }
            } if (uwr.responseCode == 405)
            {
                errorOverlayText.text = uwr.downloadHandler.text;
                runButton.interactable = true;
                errorOverlayCanvas.enabled = true;

            }
        }
        runButton.interactable = true;
    }
}


public class WorldData
{
    public int[ , , ] world;
}