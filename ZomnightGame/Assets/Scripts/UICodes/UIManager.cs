using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject menuCanvas, inGameCanvas;
    public GameObject loginScene, menuScene, createRoomScene, joinRandomGameScene, roomScene, joinRoomScene, settingsScene, versionErrorScene,
        quitScene;
    public InputField nicknameInputField;
    public Text nicknameText;
    public Text zombieKillsEndText, playerKillsEndText, coinGainEndText;
    public GameObject statsArea;
    public Text coinText, energyText;
    public InputField roomNameInputField, roomPasswordInputField;
    public Text userRoomed1, userRoomed2, userRoomed3;
    public GameObject playCoopButton;
    public GameObject roomListContainer;
    public Text joinRoomRoomIDText;
    public Text joinRoomRoomPasswordText;
    public List<GameObject> instantiatedRoomCells = new List<GameObject>();
    public List<GameObject> searchedRoomCells = new List<GameObject>();
    public List<string> roomList = new List<string>();
    public InputField searchRoomInputField;
    public GameObject roomCellPrefab;
    public InputField joinRoomRoomPasswordInputField;

    public GameObject messageListContainer;
    public List<GameObject> instantiatedMessageCells = new List<GameObject>();
    public List<RoomMessages> roomMessages = new List<RoomMessages>();
    public InputField roomMessageInputField;
    public GameObject messageCellPrefab;

    public GameObject errorMessagePrefab;

    public Image mainHintImage;
    public List<Sprite> hintImages = new List<Sprite>();
    int hintIndex = 0;
    string hintDetail = string.Empty;
    public Text hintDetailText;

    public GameObject englishTickImage, turkishTickImage;

    public bool coinBoostClicked = false;
    public Button boostEnergyButton;

    public Text connectionStatus;

    public bool versionMatch = false;

    public Text versionErrorText;

    private void Awake()
    {
        PlayerPrefs.SetString("AppVersion", "2.7");
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("Nickname")))
        {
            Client.instance.ConnectToServer();
            loginScene.SetActive(false);
            menuScene.SetActive(true);
            nicknameText.text = "Nickname: " + PlayerPrefs.GetString("Nickname");
        }

        //PlayerPrefs.SetString("Coin", "200000");
        //PlayerPrefs.SetString("Energy", "20000");

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("Coin")))
        {
            coinText.text = PlayerPrefs.GetString("Coin");
        }
        else
        {
            PlayerPrefs.SetString("Coin", "400");
            coinText.text = PlayerPrefs.GetString("Coin");
        }

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("Energy")))
        {
            energyText.text = PlayerPrefs.GetString("Energy");
        }
        else
        {
            PlayerPrefs.SetString("Energy", "3");
            energyText.text = PlayerPrefs.GetString("Energy");
        }
    }

    // Update is called once per frame
    void Update()
    {
        VersionCheck();

        energyText.text = PlayerPrefs.GetString("Energy");
        coinText.text = PlayerPrefs.GetString("Coin");

        if (GameManager.instance.roomedCount >= 2)
        {
            try
            {
                if (GameManager.instance.userRoomed[0] == PlayerPrefs.GetString("Nickname"))
                {
                    playCoopButton.SetActive(true);
                }
                else
                {
                    playCoopButton.SetActive(false);
                }
            }
            catch
            {

            }
        }
        else
        {
            playCoopButton.SetActive(false);
        }

        

        HintShow();

        if (PlayerPrefs.GetString("LocalLang") == "English")
        {
            englishTickImage.SetActive(true);
            turkishTickImage.SetActive(false);
        }
        else
        {
            englishTickImage.SetActive(false);
            turkishTickImage.SetActive(true);
        }

        if (int.Parse(PlayerPrefs.GetString("Energy")) == 3)
        {
            boostEnergyButton.interactable = false;
        }
        else
        {
            boostEnergyButton.interactable = true;
        }
    }

    public void InstantiateError(string message)
    {
        GameObject errorMessage;

        errorMessage = Instantiate(errorMessagePrefab);
        errorMessage.transform.SetParent(menuCanvas.transform, false);

        errorMessage.GetComponent<ErrorMessageController>().errorText.text = message;
    }

    public void HintShow()
    {
        if (hintIndex == 0)
        {
            if (LocalizationController.instance.localLang == "English")
            {
                hintDetail = "You have to stay in dark area. Otherwise, you die so fast.";
            }
            else
            {
                hintDetail = "Karanlık alanda kalmalısın. Yoksa çok hızlı bir şekilde ölürsün.";
            }
        }
        else if (hintIndex == 1)
        {
            if (LocalizationController.instance.localLang == "English")
            {
                hintDetail = "You can find the center of the area on the minimap. The star symbolizes it. And the blue dot is your partner.";
            }
            else
            {
                hintDetail = "Alanın merkezini radarda yıldız şeklinde görebilirsin, mavi nokta ise senin ortağını temsil ediyor.";
            }
        }
        else
        {
            if (LocalizationController.instance.localLang == "English")
            {
                hintDetail = "Weapons you find on the ground make you stronger against zombies.";
            }
            else
            {
                hintDetail = "Yerde bulduğun silahlar seni zombilere karşı daha güçlü yapar.";
            }
        }

        mainHintImage.sprite = hintImages[hintIndex];
        hintDetailText.text = hintDetail;
    }

    public void VersionCheck()
    {
        if (!string.IsNullOrEmpty(RemoteConfigController.instance.appVersion))
        {
            if (RemoteConfigController.instance.appVersion == PlayerPrefs.GetString("AppVersion"))
            {
                versionMatch = true;
            }
            else
            {
                versionMatch = false;
            }

            if (!versionMatch)
            {
                VersionError();
            }
        }
    }

    public void VersionError()
    {
        versionErrorScene.SetActive(true);
        if (LocalizationController.instance.localLang == "English")
        {
            versionErrorText.text = "Please download the new version.";
        }
        else
        {
            versionErrorText.text = "Lütfen yeni sürümü indirin.";
        }
    }

    // BUTTON FUNCTIONS

    public void LoginClick()
    {
        if (!string.IsNullOrEmpty(nicknameInputField.text))
        {
            Client.instance.ConnectToServer();
            loginScene.SetActive(false);
            menuScene.SetActive(true);
            PlayerPrefs.SetString("Nickname", nicknameInputField.text);
            nicknameText.text = "Nickname: " + PlayerPrefs.GetString("Nickname");
        }
        else
        {
            if (LocalizationController.instance.localLang=="English")
            {
                InstantiateError("Please type your nickname.");
            }
            else
            {
                InstantiateError("Lütfen nickname giriniz.");
            }
        }
    }

    public void StatsClick()
    {
        if (connectionStatus.text == "Connection: Active")
        {
            ClientSend.StatsRequest();
        }
        else
        {
            if (LocalizationController.instance.localLang == "English")
            {
                InstantiateError("No connection. Please restart the game.");
            }
            else
            {
                InstantiateError("Bağlantınız aktif değil. Lütfen oyunu tekrar başlatın.");
            }
        }
    }

    public void NextHintClick()
    {
        hintIndex++;
        if (hintIndex==3)
        {
            hintIndex = 0;
        }
    }

    public void PreviousHintClick()
    {
        hintIndex--;
        if (hintIndex==-1)
        {
            hintIndex = 2;
        }
    }

    public void SinglePlayClick()
    {
        if (int.Parse(PlayerPrefs.GetString("Energy")) != 0)
        {
            menuCanvas.SetActive(false);
            inGameCanvas.SetActive(true);
            GameManager.instance.SinglePlayMode();
            PlayerPrefs.SetString("Energy", (int.Parse(PlayerPrefs.GetString("Energy")) - 1).ToString());
        }
        else
        {
            if (LocalizationController.instance.localLang == "English")
            {
                InstantiateError("Not enough energy, please watch a video.");
            }
            else
            {
                InstantiateError("Yeterli enerjiniz yok, lütfen video izleyin.");
            }
        }
    }

    public void JoinaRoomSceneClick()
    {
        if (connectionStatus.text == "Connection: Active")
        {
            if (int.Parse(PlayerPrefs.GetString("Energy")) != 0)
            {
                menuScene.SetActive(false);
                joinRoomScene.SetActive(true);

                joinRoomRoomIDText.text = "Room ID: Not Picked";

                try
                {
                    foreach (GameObject cell in instantiatedRoomCells)
                    {
                        Destroy(cell, 0f);
                    }
                }
                catch
                {

                }

                instantiatedRoomCells.Clear();
                roomList.Clear();

                ClientSend.RoomCellRequest();
                Debug.Log("RoomCellRequest Sent");
                //PlayerPrefs.SetString("Energy", (int.Parse(PlayerPrefs.GetString("Energy")) - 1).ToString());
            }
            else
            {
                if (LocalizationController.instance.localLang == "English")
                {
                    InstantiateError("Not enough energy, please watch a video.");
                }
                else
                {
                    InstantiateError("Yeterli enerjiniz yok, lütfen video izleyin.");
                }
            }
        }

        else
        {
            if (LocalizationController.instance.localLang == "English")
            {
                InstantiateError("No connection. Please restart the game.");
            }
            else
            {
                InstantiateError("Bağlantınız aktif değil. Lütfen oyunu tekrar başlatın.");
            }
        }
        
        
    }

    public void JoinRoomClick()
    {
        if (joinRoomRoomPasswordInputField.text == joinRoomRoomPasswordText.text)
        {
            ClientSend.JoinPrivateRoomRequest();
        }
        else
        {
            if (LocalizationController.instance.localLang == "English")
            {
                InstantiateError("Please check your password.");
            }
            else
            {
                InstantiateError("Lütfen şifrenizi kontrol ediniz.");
            }
        }
    }

    public void SendRoomMessageClick()
    {
        if (!string.IsNullOrEmpty(roomMessageInputField.text))
        {
            ClientSend.RoomMessage(roomMessageInputField.text);
        }

        roomMessageInputField.text = string.Empty;
    }

    public void PlayCoopClick()
    {
        ClientSend.PlayCoopRequest();
    }

    public void SearchRoomClick()
    {
        if (!string.IsNullOrEmpty(searchRoomInputField.text))
        {
            for (int i = 0; i < instantiatedRoomCells.Count; i++)
            {
                if (!instantiatedRoomCells[i].GetComponent<RoomListManager>().roomNameText.text.Contains(searchRoomInputField.text))
                {
                    Destroy(instantiatedRoomCells[i], 0f);
                }
            }
        }
    }

    public void JoinRandomGameClick()
    {
        if (connectionStatus.text == "Connection: Active")
        {
            if (int.Parse(PlayerPrefs.GetString("Energy")) != 0)
            {

                ClientSend.PlayRequest();
                joinRandomGameScene.SetActive(true);
                //PlayerPrefs.SetString("Energy", (int.Parse(PlayerPrefs.GetString("Energy")) - 1).ToString());
            }
            else
            {
                if (LocalizationController.instance.localLang == "English")
                {
                    InstantiateError("Not enough energy, please watch a video.");
                }
                else
                {
                    InstantiateError("Yeterli enerjiniz yok, lütfen video izleyin.");
                }
            }
        }
        else
        {
            if (LocalizationController.instance.localLang == "English")
            {
                InstantiateError("No connection. Please restart the game.");
            }
            else
            {
                InstantiateError("Bağlantınız aktif değil. Lütfen oyunu tekrar başlatın.");
            }
        }
    }

    public void CancelRandomRequestClick()
    {
        ClientSend.CancelRandomRequest();
        joinRandomGameScene.SetActive(false);
        menuScene.SetActive(true);
    }

    public void CreateRoomSceneClick()
    {
        if (connectionStatus.text == "Connection: Active")
        {
            if (int.Parse(PlayerPrefs.GetString("Energy")) != 0)
            {
                if (int.Parse(PlayerPrefs.GetString("Coin")) >= 200)
                {
                    menuScene.SetActive(false);
                    createRoomScene.SetActive(true);
                }
                else
                {
                    if (LocalizationController.instance.localLang == "English")
                    {
                        InstantiateError("Not enough coin, please watch a video.");
                    }
                    else
                    {
                        InstantiateError("Yeterli coininiz yok, lütfen video izleyin.");
                    }
                }
                //PlayerPrefs.SetString("Energy", (int.Parse(PlayerPrefs.GetString("Energy")) - 1).ToString());
            }
            else
            {
                if (LocalizationController.instance.localLang == "English")
                {
                    InstantiateError("Not enough energy, please watch a video.");
                }
                else
                {
                    InstantiateError("Yeterli enerjiniz yok, lütfen video izleyin.");
                }
            }
        }
        else
        {
            if (LocalizationController.instance.localLang == "English")
            {
                InstantiateError("No connection. Please restart the game.");
            }
            else
            {
                InstantiateError("Bağlantınız aktif değil. Lütfen oyunu tekrar başlatın.");
            }
        }
    }

    public void CreateRoomClick()
    {
        if (connectionStatus.text == "Connection: Active")
        {
            if (!string.IsNullOrEmpty(roomNameInputField.text) && !string.IsNullOrEmpty(roomPasswordInputField.text))
            {
                ClientSend.CreateCoopRoomRequest();
            }
            else
            {
                if (LocalizationController.instance.localLang == "English")
                {
                    InstantiateError("Please fill the blanks.");
                }
                else
                {
                    InstantiateError("Lütfen boşlukları doldurunuz.");
                }
            }
        }
        else
        {
            if (LocalizationController.instance.localLang == "English")
            {
                InstantiateError("No connection. Please restart the game.");
            }
            else
            {
                InstantiateError("Bağlantınız aktif değil. Lütfen oyunu tekrar başlatın.");
            }
        }
        
    }

    public void BackFromRoomSceneClick()
    {
        // burada odadan çıkartma isteği gönder sunucuya
        ClientSend.ExitRoomRequest();
        GameManager.instance.userRoomed.Clear();
        userRoomed1.text = string.Empty;
        userRoomed2.text = string.Empty;
        userRoomed3.text = string.Empty;
        playCoopButton.SetActive(false);
        roomScene.SetActive(false);
        menuScene.SetActive(true);
        Debug.Log("Left from room.");
        foreach (GameObject cell in instantiatedMessageCells)
        {
            Destroy(cell, 0f);
        }
        instantiatedMessageCells.Clear();
        roomMessages.Clear();
    }

    public void BackToTheMenuClick()
    {
        menuScene.SetActive(true);
        createRoomScene.SetActive(false);
        joinRandomGameScene.SetActive(false);
        joinRoomScene.SetActive(false);
    }

    public void StatsOkButtonClick()
    {
        statsArea.SetActive(false);
    }

    public void SettingsClick()
    {
        settingsScene.SetActive(true);
    }

    public void EnglishClick()
    {
        PlayerPrefs.SetString("LocalLang", "English");
        LocalizationController.instance.localLang = "English";
        englishTickImage.SetActive(true);
        turkishTickImage.SetActive(false);
        LocalizationController.instance.SetLanguageText();
    }

    public void TurkishClick()
    {
        PlayerPrefs.SetString("LocalLang", "Turkish");
        LocalizationController.instance.localLang = "Turkish";
        englishTickImage.SetActive(false);
        turkishTickImage.SetActive(true);
        LocalizationController.instance.SetLanguageText();
    }

    public void SettingsOkButtonClick()
    {
        settingsScene.SetActive(false);
    }

    public void BoostEnergyClick()
    {
        coinBoostClicked = false;
    }

    public void BoostCoinClick()
    {
        coinBoostClicked = true;
    }

    public void BoostCoinOrEnergy()
    {
        if (coinBoostClicked)
        {
            PlayerPrefs.SetString("Coin", (int.Parse(PlayerPrefs.GetString("Coin")) + 200).ToString());
        }
        else
        {
            PlayerPrefs.SetString("Energy", "3");
        }
    }

    public void UpdateVersionClick()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.BozGames.Zomnight");
    }

    public void LogOutButtonClick()
    {
        menuScene.SetActive(false);
        loginScene.SetActive(true);
        PlayerPrefs.DeleteKey("Nickname");

        try
        {
            Client.instance.Disconnect();
        }
        catch
        {

        }
    }

    public void QuitCoopGame()
    {
        menuCanvas.SetActive(true);
        menuScene.SetActive(true);
        joinRandomGameScene.SetActive(false);
        inGameCanvas.SetActive(false);
        ClientSend.Killed();
        GameManager.instance.DestroyPlayer(Client.instance.myId);
        GameManager.instance.QuitCoopMode();
    }

    public void ExitButtonClick()
    {
        quitScene.SetActive(true);
    }

    public void ExitYesClick()
    {
        Application.Quit();
    }

    public void ExitNoClick()
    {
        quitScene.SetActive(false);
    }
}
