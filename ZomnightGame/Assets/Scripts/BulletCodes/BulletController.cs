using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    float speed;

    Rigidbody2D rg;

    public GameObject muzzleFlamePrefab;

    public int ownerID;

    private void Awake()
    {
        rg = transform.GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        MuzzleFlame();
        Movement();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Movement()
    {
        if (transform.gameObject.tag == "rifleBullet")
        {
            speed = 500f;
        }
        else
        {
            speed = 350f;
        }

        rg.AddForce(transform.up * speed);
    }

    public void MuzzleFlame()
    {
        GameObject muzzleFlame;

        muzzleFlame = Instantiate(muzzleFlamePrefab, transform.position, transform.rotation);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "rifleLoot" || collision.gameObject.tag == "shotgunLoot" || collision.gameObject.tag== "circleArea")
        {
            
        }
        else if (collision.gameObject.tag == "player")
        {
            try
            {
                collision.gameObject.GetComponent<CharController>().killerID = ownerID;
            }
            catch
            {

            }
            
            Destroy(this.gameObject, 0f);
        }
        else if (collision.gameObject.tag == "zombie")
        {
            collision.gameObject.GetComponent<ZombieController>().killerID = ownerID;

            Destroy(this.gameObject, 0f);
        }

        else if (collision.gameObject.tag == "bossZombie")
        {
            collision.gameObject.GetComponent<BossZombieController>().killerID = ownerID;

            Destroy(this.gameObject, 0f);
        }

        else
        {
            Destroy(this.gameObject, 0f);
        }
    }
}
