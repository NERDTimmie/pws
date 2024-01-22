using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStorage : MonoBehaviour
{

    public int[,,] worldData;
    public string code;

    private static DataStorage instance;
    public static DataStorage Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataStorage>();

                if (instance == null)
                {
                    GameObject obj = new GameObject("DataStorage");
                    instance = obj.AddComponent<DataStorage>();
                }

                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
