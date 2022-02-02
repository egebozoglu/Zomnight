using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    public List<Sprite> walkingSprites = new List<Sprite>();
    public List<Sprite> attackingSprites = new List<Sprite>();

    public GameObject sprite;

    public bool attacking = false;
    public int health = 100;

    float spriteRate = 0f;
    float spriteTime = 0.07f;
    int walkingSpriteNo = 0;
    int attackingSpriteNo = 0;

    float speed = 0.022f;

    public int killerID;

    List<TargetPlayers> targetPlayers = new List<TargetPlayers>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health == 0)
        {
            ZombieStats zombieStat = new ZombieStats(killerID);
            GameManager.instance.zombieStats.Add(zombieStat);
            GameManager.instance.zombies.Remove(this.gameObject);
            Destroy(this.gameObject, 0f);
        }
    }

    private void FixedUpdate()
    {
        Rotation();
        if (GameManager.instance.gameStarted)
        {
            Movement();
        }
        //else if (!GameManager.instance.inOnlineGame && GameManager.instance.gameStarted)
        //{
        //    Movement();
        //}
    }

    public void Movement()
    {
        if (attacking)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = attackingSprites[attackingSpriteNo];
            walkingSpriteNo = 0;
            spriteRate += Time.deltaTime;
            if (spriteRate > spriteTime)
            {
                spriteRate = 0;
                attackingSpriteNo++;
            }
            if (attackingSpriteNo >= 9)
            {
                attackingSpriteNo = 0;
            }
        }
        else
        {
            sprite.GetComponent<SpriteRenderer>().sprite = walkingSprites[walkingSpriteNo];
            attackingSpriteNo = 0;
            spriteRate += Time.deltaTime;
            if (spriteRate > spriteTime)
            {
                spriteRate = 0;
                walkingSpriteNo++;
            }
            if (walkingSpriteNo >= 17)
            {
                walkingSpriteNo = 0;
            }
        }

        if (!attacking)
        {
            try
            {
                Vector3 targetPos = GameManager.instance.characters[0].transform.position;
                transform.Translate(Vector2.up * speed);
            }
            catch
            {

            }

        }
    }

    public void Rotation()
    {
        try
        {
            for (int i = 0; i < GameManager.instance.characters.Count; i++)
            {
                float distance = Vector2.Distance(transform.position, GameManager.instance.characters[i].transform.position);
                TargetPlayers targetPlayer = new TargetPlayers(GameManager.instance.characters[i], distance);
                targetPlayers.Add(targetPlayer);
            }

            targetPlayers = targetPlayers.OrderBy(x => x.Distance).ToList();

            Vector3 targetPos = targetPlayers[0].Player.transform.position;

            var targX = targetPos.x - transform.position.x;
            var targY = targetPos.y - transform.position.y;

            float angle = Mathf.Atan2(-targX, targY) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            targetPlayers.Clear();
        }
        catch
        {

        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "pistolBullet")
        {
            health -= 24;
            GameManager.instance.InstantiateBlood(transform.position);
        }
        else if (collision.gameObject.tag == "rifleBullet")
        {
            health -= 46;
            GameManager.instance.InstantiateBlood(transform.position);
        }
        else if (collision.gameObject.tag == "shotgunBullet")
        {
            health -= 52;
            GameManager.instance.InstantiateBlood(transform.position);
        }

        if (collision.gameObject.tag == "player")
        {
            attacking = true;
        }

        if (health<0)
        {
            
            health = 0;
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "player")
        {
            attacking = false;
        }
    }
}
