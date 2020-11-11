using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlowManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private TextMeshProUGUI cardText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private void Start()
    {
        var eventCard = EventGenerator.GenerateRandomEvent();
        cardText.text = eventCard.requestText;
        var npc = NPCGenerator.GenerateNPC(new[] {"Army"}, spawnPoint);
        var npcBehavior = npc.GetComponent<NPCBehavior>();
        yesButton.onClick.AddListener(npcBehavior.CloseEventPanel);
        noButton.onClick.AddListener(npcBehavior.CloseEventPanel);
    }
}
