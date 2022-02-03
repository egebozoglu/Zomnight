using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<GameObject> bullets = new List<GameObject>();

    public GameObject localPlayerPrefab;
    public GameObject otherPlayerPrefab;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    
    public string RoomID;

    public System.Random rand = new System.Random();


    public Image weaponImage;
    public List<Sprite> weaponSprites = new List<Sprite>();

    public FixedJoystick joystick;
    public GameObject mainCamera;
    public Button fireButton, reloadButton;
    public Text bulletInfoText, healthInfoText;
    public Text zombiesCountText, playersCountText;

    public List<GameObject> fireSounds = new List<GameObject>();

    public GameObject bloodEffectPrefab;

    public GameObject zombiePrefab;

    public GameObject bossZombiePrefab;

    public GameObject lootButton;

    public List<GameObject> lootObjects = new List<GameObject>();

    public List<GameObject> environmentPrefabs = new List<GameObject>();

    public List<GameObject> characters = new List<GameObject>();
    public List<GameObject> environments = new List<GameObject>();
    public List<GameObject> zombies = new List<GameObject>();

    public List<Vector3> zombiePositions = new List<Vector3>();

    public bool inOnlineGame = false;

    public GameObject quitGameButton, quitCoopButton;

    public List<PlayerStats> playerStats = new List<PlayerStats>();
    public List<ZombieStats> zombieStats = new List<ZombieStats>();
    public List<BossZombieStats> bossZombieStats = new List<BossZombieStats>();

    public Text zombieKillsText, playerKillsText;

    public List<string> userRoomed = new List<string>();
    public int roomedCount;

    public GameObject circleAreaPrefab;
    public List<GameObject> instantiatedCircleAreas = new List<GameObject>();

    public bool gameStarted = false;
    public bool zombiesInstantiated = false;

    public GameObject footStepSoundPrefab;

    public GameObject backgroundSoundPrefab;
    public List<GameObject> instantiatedBackgroundSound;

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
        PlayBackgroundSound();
    }

    // Update is called once per frame
    void Update()
    {
        zombiesCountText.text = "Zombies: " + zombies.Count.ToString();
        if (inOnlineGame)
        {
            quitGameButton.SetActive(false);
            quitCoopButton.SetActive(true);
            playersCountText.text = "Players: " + characters.Count.ToString();
        }
        else
        {
            quitGameButton.SetActive(true);
            quitCoopButton.SetActive(false);
            playersCountText.text = string.Empty;
        }

        if (zombies.Count == 90)
        {
            zombiesInstantiated = true;
        }
        else
        {
            zombiesInstantiated = false;
        }

        zombieKillsText.text = "Zombie Kills: " + (zombieStats.Where(x => x.KillerID == Client.instance.myId).ToList().Count + bossZombieStats.Where(x => x.KillerID == Client.instance.myId).ToList().Count).ToString();
        playerKillsText.text = "Player Kills: " + playerStats.Where(x => x.KillerID == Client.instance.myId).ToList().Count.ToString();
    }

    public void PlayBackgroundSound()
    {
        GameObject backgroundSound;

        backgroundSound = Instantiate(backgroundSoundPrefab);

        instantiatedBackgroundSound.Add(backgroundSound);
    }

    public void SpawnPlayer(int id, string userName, Vector3 position, Quaternion rotation)
    {
        GameObject player;

        if (id == Client.instance.myId)
        {
            player = Instantiate(localPlayerPrefab, position, rotation);
            player.GetComponent<CharController>().joystick = joystick;
            player.GetComponent<CharController>().fireButton = fireButton;
            player.GetComponent<CharController>().reloadButton = reloadButton;
            player.GetComponent<CharController>().mainCamera = mainCamera;
        }
        else
        {
            player = Instantiate(otherPlayerPrefab, position, rotation);
            player.GetComponent<OtherCharController>().nicknameTextMesh.GetComponent<TextMesh>().text = userName;
        }

        player.GetComponent<PlayerManager>().id = id;
        player.GetComponent<PlayerManager>().userName = userName;
        players.Add(id, player.GetComponent<PlayerManager>());
        characters.Add(player);
        Debug.Log($"ID: {id}, Username: {userName} oyuna bağlandı.");
    }

    public void DestroyPlayer(int id)
    {
        try
        {
            var character = characters.Where(x => x.GetComponent<PlayerManager>().id == id).FirstOrDefault();

            if (character.GetComponent<PlayerManager>().id == Client.instance.myId)
            {
                Debug.Log("It's me...");
                foreach (var item in characters)
                {
                    Destroy(item, 0f);
                }

                characters.Clear();
                players.Clear();
            }
            else
            {
                try
                {
                    Debug.Log(character.GetComponent<PlayerManager>().userName + " oyundan ayrıldı.");
                    Destroy(character, 0f);
                    characters.Remove(character);
                }
                catch
                {

                }

                players.Remove(id);
            }
        }
        catch
        {

        }
    }

    public void InstantiateBullet(int bulletType, Vector3 position, Quaternion rotation, int ownerID)
    {
        GameObject bullet;
        GameObject fireSound;

        bullet = Instantiate(bullets[bulletType], position, rotation);
        fireSound = Instantiate(fireSounds[bulletType], position, Quaternion.identity);
        bullet.GetComponent<BulletController>().ownerID = ownerID;

        Destroy(fireSound, 1f);
    }

    public void InstantiateBlood(Vector3 position)
    {
        GameObject blood;

        blood = Instantiate(bloodEffectPrefab, position, Quaternion.identity);

        Destroy(blood, 0.2f);
    }

    public void InstantiateFootStepSound(Vector3 position)
    {
        GameObject footStepSound;

        footStepSound = Instantiate(footStepSoundPrefab, position, Quaternion.identity);

        Destroy(footStepSound, 3f);
    }

    public void InstantiateZombie(Vector3 position)
    {
        GameObject zombie;

        zombie = Instantiate(zombiePrefab, position, Quaternion.identity);
        zombies.Add(zombie);
    }

    public void InstantiateBossZombie(Vector3 position)
    {
        GameObject bossZombie;

        bossZombie = Instantiate(bossZombiePrefab, position, Quaternion.identity);
        zombies.Add(bossZombie);
    }

    //public void InstantiateLootObjects(Vector3 position, int pickLoot)
    //{
    //    GameObject lootObject;

    //    lootObject = Instantiate(lootObjects[pickLoot], position, Quaternion.identity);
    //    loots.Add(lootObject);
    //}

    public void InstantiateCircleArea(Vector3 position)
    {
        GameObject circleArea;

        circleArea = Instantiate(circleAreaPrefab, position, Quaternion.identity);

        instantiatedCircleAreas.Add(circleArea);
    }

    public void PositionGeneratorZombie(int quantity)
    {
        System.Random rand = new System.Random();
        for (int i = 0; i < 5; i++)
        {
            float randX = rand.Next(-70, 70);
            float randY = rand.Next(-70, 70);

            Vector3 position = new Vector3(randX, randY, 0f);

            InstantiateBossZombie(position);
            zombiePositions.Add(position);
        }

        for (int i = 5; i < quantity; i++)
        {
            float randX = rand.Next(-60, 60);
            float randY = rand.Next(-60, 60);

            Vector3 position = new Vector3(randX, randY, 0f);

            InstantiateZombie(position);
            zombiePositions.Add(position);
        }
    }

    //public void PositionGeneratorLoot(int quantity)
    //{
    //    System.Random rand = new System.Random();
    //    for (int i = 0; i < quantity; i++)
    //    {
    //        float randX = rand.Next(-80, 80);
    //        float randY = rand.Next(-80, 80);

    //        Vector3 position = new Vector3(randX, randY, 0f);
    //        int pickLoot = UnityEngine.Random.Range(0, 2);
    //        InstantiateLootObjects(position, pickLoot);
    //    }
    //}

    public void LootClick()
    {
        if (CharController.instance.rifleLoot)
        {
            CharController.instance.bulletType = 1;
            CharController.instance.weaponBullet = 30;
            CharController.instance.magazineBullet = 90;
        }
        else
        {
            CharController.instance.bulletType = 2;
            CharController.instance.weaponBullet = 2;
            CharController.instance.magazineBullet = 8;
        }

        //Destroy(CharController.instance.objectToLoot, 0f);
        lootObjects.Add(CharController.instance.objectToLoot);
        CharController.instance.objectToLoot.SetActive(false);
    }

    // GAME MODE CODES

    public void SinglePlayMode()
    {
        Destroy(instantiatedBackgroundSound[0], 0f);
        if (UIManager.instance.connectionStatus.text == "Connection: Not Active" && zombiesInstantiated == false)
        {
            PositionGeneratorZombie(90);
            zombiesInstantiated = true;
        }
        gameStarted = true;
        playerStats.Clear();
        zombieStats.Clear();
        bossZombieStats.Clear();
        inOnlineGame = false;
        float randX = rand.Next(-30, 30);
        float randY = rand.Next(-30, 30);
        float randX2 = rand.Next(-30, 30);
        float randY2 = rand.Next(-30, 30);
        InstantiateCircleArea(new Vector3(randX, randY, 0f));
        SpawnPlayer(Client.instance.myId, PlayerPrefs.GetString("Nickname"), new Vector3(randX2, randY2, 0), Quaternion.Euler(0, 0, 0));
    }

    public void QuitSinglePlayMode()
    {
        PlayBackgroundSound();
        gameStarted = false;
        Destroy(characters[0], 0f);

        foreach (GameObject environment in environments)
        {
            Destroy(environment, 0f);
        }

        foreach (GameObject zombie in zombies)
        {
            Destroy(zombie, 0f);
        }

        foreach (GameObject area in instantiatedCircleAreas)
        {
            Destroy(area, 0f);
        }

        instantiatedCircleAreas.Clear();
        characters.Clear();
        players.Clear();
        environments.Clear();
        zombies.Clear();

        UIManager.instance.menuCanvas.SetActive(true);
        UIManager.instance.inGameCanvas.SetActive(false);
        UIManager.instance.zombieKillsEndText.text = "Zombie Kills: " + (zombieStats.Where(x => x.KillerID == Client.instance.myId).ToList().Count + bossZombieStats.Where(x => x.KillerID == Client.instance.myId).ToList().Count).ToString();
        UIManager.instance.playerKillsEndText.text = "Player Kills: " + playerStats.Where(x => x.KillerID == Client.instance.myId).ToList().Count.ToString();
        UIManager.instance.coinGainEndText.text = "Coins: 0";
        UIManager.instance.statsArea.SetActive(true);

        for (int i = 0; i < 5; i++)
        {
            InstantiateBossZombie(zombiePositions[i]);
        }

        for (int i = 5; i < zombiePositions.Count; i++)
        {
            InstantiateZombie(zombiePositions[i]);
        }

        foreach (GameObject loot in lootObjects)
        {
            loot.SetActive(true);
        }

        lootObjects.Clear();
    }

    public void QuitCoopMode()
    {
        PlayBackgroundSound();
        inOnlineGame = false;
        
        foreach (GameObject environment in environments)
        {
            Destroy(environment, 0f);
        }

        foreach (GameObject zombie in zombies)
        {
            Destroy(zombie, 0f);
        }

        foreach (GameObject area in instantiatedCircleAreas)
        {
            Destroy(area, 0f);
        }

        instantiatedCircleAreas.Clear();
        environments.Clear();
        zombies.Clear();
        UIManager.instance.zombieKillsEndText.text = "Zombie Kills: " + (zombieStats.Where(x => x.KillerID == Client.instance.myId).ToList().Count + bossZombieStats.Where(x => x.KillerID == Client.instance.myId).ToList().Count).ToString();
        UIManager.instance.playerKillsEndText.text = "Player Kills: " + playerStats.Where(x => x.KillerID == Client.instance.myId).ToList().Count.ToString();
        int coinGained = (2 * bossZombieStats.Where(x => x.KillerID == Client.instance.myId).ToList().Count)
            + zombieStats.Where(x => x.KillerID == Client.instance.myId).ToList().Count
            - (2 * (playerStats.Where(x => x.KillerID == Client.instance.myId).ToList().Count));
        if (coinGained<0)
        {
            coinGained = 0;
        }
        UIManager.instance.coinGainEndText.text = "Coins: " + coinGained.ToString();
        PlayerPrefs.SetString("Coin", (int.Parse(PlayerPrefs.GetString("Coin")) + coinGained).ToString());
        UIManager.instance.statsArea.SetActive(true);
        gameStarted = false;
        zombieStats.Clear();
        playerStats.Clear();
        bossZombieStats.Clear();

        for (int i = 0; i < 5; i++)
        {
            InstantiateBossZombie(zombiePositions[i]);
        }

        for (int i = 5; i < zombiePositions.Count; i++)
        {
            InstantiateZombie(zombiePositions[i]);
        }

        foreach (GameObject loot in lootObjects)
        {
            loot.SetActive(true);
        }

        lootObjects.Clear();
    }
}
