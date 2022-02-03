using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;

public class RemoteConfigController : MonoBehaviour
{
    public static RemoteConfigController instance;
    public struct userAttributes { }
    public struct appAttributes { }

    public string appVersion;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        ConfigManager.FetchCompleted += GetAppVersion;
        ConfigManager.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
    }

    void GetAppVersion(ConfigResponse response)
    {
        appVersion = ConfigManager.appConfig.GetString("targetVersion");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
