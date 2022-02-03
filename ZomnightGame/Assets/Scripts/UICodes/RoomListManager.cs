using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomListManager : MonoBehaviour, IPointerClickHandler
{
    public static RoomListManager instance;

    public Text roomNameText;
    public Button pickButton;
    public string roomID;
    public string roomPassword;

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
        PickButtonEvent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PickButtonEvent()
    {
        EventTrigger eventTrigger = pickButton.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((data) => { OnPointerClick((PointerEventData)data); });
        eventTrigger.triggers.Add(pointerClick);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.joinRoomRoomIDText.text = roomID;
        UIManager.instance.joinRoomRoomPasswordText.text = roomPassword;
    }
}
