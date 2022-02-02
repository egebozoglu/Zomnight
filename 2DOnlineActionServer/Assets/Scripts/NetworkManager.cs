using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public List<Vector3> zombiePositions = new List<Vector3>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        System.Random rand = new System.Random();

        for (int i = 0; i < 90; i++)
        {
            float randX = rand.Next(-120, 120);
            float randY = rand.Next(-120, 120);

            Vector3 position = new Vector3(randX, randY, 0f);

            zombiePositions.Add(position);
        }

        //#if UNITY_EDITOR
        //        Debug.Log("Build the project to start the server!!!");
        //#else
        //        Server.Start(50, 1080)
        //#endif

        Server.Start(5000, 1080);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
