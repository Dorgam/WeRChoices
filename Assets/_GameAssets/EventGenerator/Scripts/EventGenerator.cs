using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EventGenerator : MonoBehaviour
{
    private static List<EventCard> _eventCards = new List<EventCard>();

    private void Awake()
    {
        var textFiles = Resources.LoadAll<TextAsset>("Cards");

        foreach (var textFile in textFiles)
        {
            _eventCards.Add(JsonUtility.FromJson<EventCard>(textFile.text));
        }
    }

    public static EventCard GenerateRandomEvent()
    {
        return GetRandomElementInList<EventCard>(_eventCards.ToArray());
    }
    
    private static T GetRandomElementInList<T>(T[] list)
    {
        var random = new Random();
        var index = UnityEngine.Random.Range(0, list.Length);
        return list[index];
    }
}
