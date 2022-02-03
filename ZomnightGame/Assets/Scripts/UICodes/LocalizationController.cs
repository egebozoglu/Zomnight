using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationController : MonoBehaviour
{
    public static LocalizationController instance;

    public string localLang;

    public Text loginButtonText, singleZombieText, joinaRoomText, joinaRandomGameText,
        createaRoomSceneText, comingSoonText, changeNicknameText, joinRoomBackText,
        joinRoomRoomNameText, searchRoomText, refreshRoomText, notPickedText, joinRoomText,
        randomGameBackText, randomGameCenterText, createRoomBackText, createARoomText,
        createRoomNameText, createRoomPasswordText, createText, roomSceneBackText,
        roomSceneMaxPlayerText, roomSceneMessagesText, textHereText, sendText, exitButton, exitYesButton, exitNoButton, exitText;

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
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("LocalLang")))
        {
            PlayerPrefs.SetString("LocalLang", "English");
            localLang = PlayerPrefs.GetString("LocalLang");
        }
        else
        {
            localLang = PlayerPrefs.GetString("LocalLang");
        }

        SetLanguageText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLanguageText()
    {
        if (localLang == "English")
        {
            loginButtonText.text = "Login";
            singleZombieText.text = "Single Zombie Attack";
            joinaRoomText.text = "Join a Room";
            joinaRandomGameText.text = "Join a Random Game";
            createaRoomSceneText.text = "Create a Room";
            comingSoonText.text = "Coming Soon..";
            changeNicknameText.text = "Change Nickname";
            joinRoomBackText.text = "Back";
            joinRoomRoomNameText.text = "Room Name";
            searchRoomText.text = "Search";
            refreshRoomText.text = "Refresh";
            notPickedText.text = "Room ID: Not Picked";
            joinRoomText.text = "Join";
            randomGameBackText.text = "Back";
            randomGameCenterText.text = "Searching Available Rooms...";
            createRoomBackText.text = "Back";
            createARoomText.text = "Create a Room";
            createRoomNameText.text = "Room Name";
            createRoomPasswordText.text = "Room Password";
            createText.text = "Create";
            roomSceneBackText.text = "Back";
            roomSceneMaxPlayerText.text = "Max 3 Players";
            roomSceneMessagesText.text = "Room Messages";
            textHereText.text = "Text Here";
            sendText.text = "Send";
            exitButton.text = "Exit";
            exitNoButton.text = "No";
            exitYesButton.text = "Yes";
            exitText.text = "Do you want to quit?";
        }
        else
        {
            loginButtonText.text = "Giriş Yap";
            singleZombieText.text = "Tekli Zombi Saldırısı";
            joinaRoomText.text = "Odaya Katıl";
            joinaRandomGameText.text = "Rastgele Oyun Bul";
            createaRoomSceneText.text = "Oda Oluştur";
            comingSoonText.text = "Çok Yakında..";
            changeNicknameText.text = "Nickname Değiştir";
            joinRoomBackText.text = "Geri";
            joinRoomRoomNameText.text = "Oda İsmi";
            searchRoomText.text = "Ara";
            refreshRoomText.text = "Yenile";
            notPickedText.text = "Room ID: Seçilmedi";
            joinRoomText.text = "Katıl";
            randomGameBackText.text = "Geri";
            randomGameCenterText.text = "Müsait Odalar Aranıyor...";
            createRoomBackText.text = "Geri";
            createARoomText.text = "Oda Oluştur";
            createRoomNameText.text = "Oda İsmi";
            createRoomPasswordText.text = "Şifre";
            createText.text = "Oluştur";
            roomSceneBackText.text = "Geri";
            roomSceneMaxPlayerText.text = "Max 3 Oyuncu";
            roomSceneMessagesText.text = "Oda Mesajları";
            textHereText.text = "Mesaj";
            sendText.text = "Gönder";
            exitButton.text = "Çıkış";
            exitNoButton.text = "Hayır";
            exitYesButton.text = "Evet";
            exitText.text = "Çıkmak mı istiyorsun?";
        }
    }
}
