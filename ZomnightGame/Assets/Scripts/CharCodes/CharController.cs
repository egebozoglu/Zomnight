using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public static CharController instance;

    public GameObject mainCamera;
    public GameObject bulletExitPoint;

    public FixedJoystick joystick;
    public Button fireButton, reloadButton;

    Rigidbody2D rg;

    public List<Sprite> walkingSprites = new List<Sprite>();
    public bool walking = false;
    public Sprite idleSprite;
    float walkingRate = 0f;
    float walkingTime = 0.35f;
    int walkingAnim = 0;

    float speed = 0.04f;

    public int bulletType = 0;

    bool firePressed = false;
    bool readyToFire = true;
    bool readyToReload = false;
    float fireRate = 0f;
    float fireTime = 0.3f;
    float reloadRate = 0f;
    float reloadTime = 0.8f;

    public int health = 100;

    public int weaponBullet = 7, magazineBullet = 14;

    bool attackedByZombie = false;
    float zombieRate = 0f;
    float zombieTime = 0.6f;
    bool bossAttacked = false;

    public bool rifleLoot = false;

    public GameObject objectToLoot;

    public GameObject reloadingSound;

    public int killerID = -1;

    bool inArea = true;

    public GameObject hurtSoundPrefab;

    GameObject circleArea;

    Color redColor = new Color(0.1058f, 0f, 0f, 0.9490f);
    Color blackColor = new Color(0f, 0f, 0f, 0.78f);
    float hurtRate = 0f;
    float hurtTime = 0.5f;
    bool hurted = false;

    public GameObject flashlight;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        rg = transform.GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        circleArea = GameManager.instance.instantiatedCircleAreas[0].GetComponent<CircleAreaController>().circle;
        ReloadButtonEvent();
        FireButtonEvents();
    }

    // Update is called once per frame
    void Update()
    {
        InGameInfoAreaUpdate();
        if (hurted)
        {
            HurtColor();
        }
    }

    private void FixedUpdate()
    {
        if (bulletType == 1)
        {
            fireTime = 0.1f;
        }
        else
        {
            fireTime = 0.5f;
        }

        if (health <= 0 && GameManager.instance.inOnlineGame == true)
        {
            UIManager.instance.menuCanvas.SetActive(true);
            UIManager.instance.menuScene.SetActive(true);
            UIManager.instance.joinRandomGameScene.SetActive(false);
            UIManager.instance.inGameCanvas.SetActive(false);
            ClientSend.Killed();
            GameManager.instance.DestroyPlayer(Client.instance.myId);
            GameManager.instance.QuitCoopMode();
        }

        else if (health<=0 && GameManager.instance.inOnlineGame == false)
        {
            UIManager.instance.menuCanvas.SetActive(true);
            UIManager.instance.menuScene.SetActive(true);
            UIManager.instance.joinRandomGameScene.SetActive(false);
            UIManager.instance.inGameCanvas.SetActive(false);
            GameManager.instance.QuitSinglePlayMode();
        }

        else if (GameManager.instance.zombieStats.Count == 100)
        {
            UIManager.instance.menuCanvas.SetActive(true);
            UIManager.instance.menuScene.SetActive(true);
            UIManager.instance.joinRandomGameScene.SetActive(false);
            UIManager.instance.inGameCanvas.SetActive(false);
            GameManager.instance.QuitSinglePlayMode();
        }

        if (!inArea)
        {
            flashlight.SetActive(false);
            health -= 1;
            if (health < 0)
            {
                health = 0;
            }
        }
        else
        {
            flashlight.SetActive(true);
        }

        Movement();
        Animation();
        Fire();
        ReloadWeapon();
        AttackedByZombie();
    }

    public void Movement()
    {
        var horizontal = joystick.Horizontal;
        var vertical = joystick.Vertical;

        if (horizontal != 0 || vertical != 0)
        {
            var joyAngle = Mathf.Atan2(horizontal, vertical);

            joyAngle = joyAngle * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, 0f, -joyAngle);

            if (horizontal > 0.5f || horizontal < -0.5f || vertical < -0.5f || vertical > 0.5f)
            {
                transform.Translate(Vector2.up * speed);
                walking = true;
            }
        }
        else
        {
            walking = false;
        }

        mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -10);

        if (GameManager.instance.inOnlineGame == true && GameManager.instance.gameStarted == true)
        {
            ClientSend.PlayerMovement();
        }
    }

    public void Animation()
    {
        if (walking)
        {
            walkingRate += Time.deltaTime;
            if (walkingRate > walkingTime)
            {
                GameManager.instance.InstantiateFootStepSound(transform.position);
                walkingRate = 0f;
                walkingAnim++;
                if (walkingAnim > 1)
                {
                    walkingAnim = 0;
                }
                transform.GetComponent<SpriteRenderer>().sprite = walkingSprites[walkingAnim];
            }
        }
        else
        {
            walkingRate = 0f;
            walkingAnim = 0;
            transform.GetComponent<SpriteRenderer>().sprite = idleSprite;
        }
    }

    public void Fire()
    {
        if (firePressed && readyToFire && weaponBullet > 0)
        {
            readyToFire = false;
            GameManager.instance.InstantiateBullet(bulletType, bulletExitPoint.transform.position, bulletExitPoint.transform.rotation,
                Client.instance.myId);
            weaponBullet--;
            if (GameManager.instance.inOnlineGame == true)
            {
                ClientSend.Fire(bulletExitPoint.transform.position, bulletExitPoint.transform.rotation, bulletType);
            }
            if (weaponBullet == 0)
            {
                readyToReload = true;
            }
        }

        if (readyToFire == false)
        {
            fireRate += Time.deltaTime;
            if (fireRate > fireTime)
            {
                fireRate = 0;
                readyToFire = true;
            }
        }
    }

    public void ReloadWeapon()
    {
        if (readyToReload)
        {
            reloadRate += Time.deltaTime;
            if (reloadRate > reloadTime)
            {
                reloadRate = 0f;
                if (bulletType == 0)
                {
                    if (magazineBullet >= 7)
                    {
                        magazineBullet -= 7 - weaponBullet;
                        weaponBullet = 7;
                        GameObject reloadSound;
                        reloadSound = Instantiate(reloadingSound, transform.position, Quaternion.identity);
                        Destroy(reloadSound, 3f);
                    }
                    else if (magazineBullet < 7 && magazineBullet != 0)
                    {
                        weaponBullet = magazineBullet;
                        magazineBullet = 0;
                        GameObject reloadSound;
                        reloadSound = Instantiate(reloadingSound, transform.position, Quaternion.identity);
                        Destroy(reloadSound, 3f);
                    }
                }
                else if (bulletType == 1)
                {
                    if (magazineBullet >= 30)
                    {
                        magazineBullet -= 30 - weaponBullet;
                        weaponBullet = 30;
                        GameObject reloadSound;
                        reloadSound = Instantiate(reloadingSound, transform.position, Quaternion.identity);
                        Destroy(reloadSound, 3f);
                    }
                    else if (magazineBullet < 30 && magazineBullet != 0)
                    {
                        weaponBullet = magazineBullet;
                        magazineBullet = 0;
                        GameObject reloadSound;
                        reloadSound = Instantiate(reloadingSound, transform.position, Quaternion.identity);
                        Destroy(reloadSound, 3f);
                    }
                }
                else
                {
                    if (magazineBullet >= 2)
                    {
                        magazineBullet -= 2 - weaponBullet;
                        weaponBullet = 2;
                        GameObject reloadSound;
                        reloadSound = Instantiate(reloadingSound, transform.position, Quaternion.identity);
                        Destroy(reloadSound, 3f);
                    }
                    else if (magazineBullet < 2 && magazineBullet != 0)
                    {
                        weaponBullet = magazineBullet;
                        magazineBullet = 0;
                        GameObject reloadSound;
                        reloadSound = Instantiate(reloadingSound, transform.position, Quaternion.identity);
                        Destroy(reloadSound, 3f);
                    }
                }

                readyToReload = false;
            }
        }
    }

    public void AttackedByZombie()
    {
        if (attackedByZombie)
        {
            SetCircleColor(redColor);
            zombieRate += Time.deltaTime;
            if (zombieRate > zombieTime)
            {
                zombieRate = 0;
                if (bossAttacked)
                {
                    health -= 25;
                }
                else
                {
                    health -= 9;
                }
                GameManager.instance.InstantiateBlood(transform.position);
                HurtSound();
                SetCircleColor(blackColor);
            }
        }
        else
        {
            SetCircleColor(blackColor);
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "pistolBullet")
        {
            hurted = true;
            HurtSound();
            health -= 12;
            GameManager.instance.InstantiateBlood(transform.position);
            if (health < 0)
            {
                PlayerStats playerStat = new PlayerStats(killerID);
                GameManager.instance.playerStats.Add(playerStat);
                health = 0;
            }
        }
        else if (collision.gameObject.tag == "rifleBullet")
        {
            hurted = true;
            HurtSound();
            health -= 23;
            GameManager.instance.InstantiateBlood(transform.position);
            if (health < 0)
            {
                PlayerStats playerStat = new PlayerStats(killerID);
                GameManager.instance.playerStats.Add(playerStat);
                health = 0;
            }
        }
        else if (collision.gameObject.tag == "shotgunBullet")
        {
            hurted = true;
            HurtSound();
            health -= 38;
            GameManager.instance.InstantiateBlood(transform.position);
            if (health < 0)
            {
                PlayerStats playerStat = new PlayerStats(killerID);
                GameManager.instance.playerStats.Add(playerStat);
                health = 0;
            }
        }

        if (collision.gameObject.tag == "rifleLoot")
        {
            GameManager.instance.lootButton.SetActive(true);
            rifleLoot = true;
            objectToLoot = collision.gameObject;
        }

        if (collision.gameObject.tag == "shotgunLoot")
        {
            GameManager.instance.lootButton.SetActive(true);
            rifleLoot = false;
            objectToLoot = collision.gameObject;
        }


        if (health < 0)
        {
            health = 0;
        }
    }

    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "zombie")
        {
            attackedByZombie = true;
            bossAttacked = false;
        }

        if (collision.gameObject.tag == "bossZombie")
        {
            attackedByZombie = true;
            bossAttacked = true;
        }

        if (collision.gameObject.tag == "circleArea")
        {
            inArea = true;
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "zombie")
        {
            attackedByZombie = false;
        }

        if (collision.gameObject.tag == "bossZombie")
        {
            attackedByZombie = false;
            bossAttacked = false;
        }

        if (collision.gameObject.tag == "rifleLoot")
        {
            GameManager.instance.lootButton.SetActive(false);
        }

        if (collision.gameObject.tag == "shotgunLoot")
        {
            GameManager.instance.lootButton.SetActive(false);
        }

        if (collision.gameObject.tag == "circleArea")
        {
            inArea = false;
            killerID = -1;
        }
    }

    public void HurtColor()
    {
        SetCircleColor(redColor);
        hurtRate += Time.deltaTime;
        if (hurtRate>hurtTime)
        {
            hurtRate = 0;
            SetCircleColor(blackColor);
            hurted = false;
        }
    }

    public void SetCircleColor(Color newColor)
    {
        circleArea.GetComponent<SpriteRenderer>().color = newColor;
    }

    public void InGameInfoAreaUpdate()
    {
        GameManager.instance.weaponImage.sprite = GameManager.instance.weaponSprites[bulletType];
        GameManager.instance.healthInfoText.text = health.ToString();
        GameManager.instance.bulletInfoText.text = weaponBullet.ToString() + "/" + magazineBullet.ToString();
    }

    void ReloadButtonEvent()
    {
        EventTrigger eventTrigger = reloadButton.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((data) => { OnPointerClick((PointerEventData)data); });
        eventTrigger.triggers.Add(pointerClick);
    }

    void FireButtonEvents()
    {
        EventTrigger eventTrigger = fireButton.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        eventTrigger.triggers.Add(pointerDownEntry);

        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
        eventTrigger.triggers.Add(pointerUpEntry);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        firePressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        firePressed = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        readyToReload = true;
    }

    public void HurtSound()
    {
        GameObject hurtSound;

        hurtSound = Instantiate(hurtSoundPrefab, transform.position, Quaternion.identity);

        Destroy(hurtSound, 2f);
    }
}
