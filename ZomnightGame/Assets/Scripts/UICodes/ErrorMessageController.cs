using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorMessageController : MonoBehaviour
{
    public static ErrorMessageController instance;

    public Text errorText;

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
        Destroy(this.gameObject, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
