using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageListManager : MonoBehaviour
{
    public static MessageListManager instance;

    public Text contentText;

    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }
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
