using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleAreaController : MonoBehaviour
{
    float scaleX;
    float scaleY;
    public GameObject circle;

    // Start is called before the first frame update
    void Start()
    {
        scaleX = circle.transform.localScale.x;
        scaleY = circle.transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameStarted)
        {
            CircleContraction();
        }
        //else if (!GameManager.instance.inOnlineGame)
        //{
        //    CircleContraction();
        //}
    }

    public void CircleContraction()
    {
        if (scaleX >= 0.1f && scaleY >= 0.1f)
        {
            scaleX -= Time.deltaTime / 15;
            scaleY -= Time.deltaTime / 15;
        }

        circle.transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }
}
