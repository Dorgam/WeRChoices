using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCGenerator : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private string[] sampleDescriptors;
    
    private GameObject[] _bodies;
    private GameObject[] _heads;
    private GameObject[] _hairs;
    private readonly List<string[]> _bodiesDescriptors = new List<string[]>();
    private readonly List<string[]> _headsDescriptors = new List<string[]>();
    private readonly List<string[]> _hairsDescriptors = new List<string[]>();
    private readonly string[] _races = new[] {"African", "Caucasian"};
    private readonly string[] _sexes = new[] {"Male", "Female"};
    private readonly string[] _colors = new[] {"Red", "White", "Black", "Purple", "Green", "Blue", "Pink", "Yellow", "Orange", "Brown"};
    private readonly string[] _emotions = new[] {"Normal", "Shy", "Shock", "Crying"};
    private readonly string[] _roles = new[] {"Army", "Citizen", "General", "Hunter", "Royal", "Noble", "Barbarian", "King"};

    private void Awake()
    {
        _bodies = Resources.LoadAll<GameObject>("Parts/Body");
        _heads = Resources.LoadAll<GameObject>("Parts/Head");
        _hairs = Resources.LoadAll<GameObject>("Parts/Hair");

        foreach (GameObject body in _bodies)
        {
            _bodiesDescriptors.Add(GetPartDescriptors(body));
        }

        foreach (GameObject head in _heads)
        {
            _headsDescriptors.Add(GetPartDescriptors(head));
        }
        
        foreach (GameObject hair in _hairs)
        {
            _hairsDescriptors.Add(GetPartDescriptors(hair));
        }
    }

    private void Start()
    {
        var body = GenerateBase(sampleDescriptors);
        var npc = Instantiate(body, spawnPoint.position, Quaternion.identity);
        var unionDescriptors = sampleDescriptors.Union(GetPartDescriptors(body)).ToArray();
        var head = GenerateHead(unionDescriptors);
        var hair = GenerateHair(unionDescriptors);
        npc.GetComponent<CharacterParts>().ChangeHead(head);
        npc.GetComponent<CharacterParts>().ChangeHair(hair);
        
        /*var body = GenerateBase(sampleDescriptors);
        var unionDescriptors = sampleDescriptors.Union(GetPartDescriptors(body)).ToArray();
        var npc = Instantiate(GenerateBase(sampleDescriptors), spawnPoint.position, Quaternion.identity);
        var head = Instantiate(GenerateHead(unionDescriptors), spawnPoint.position, Quaternion.identity);
        var hair = Instantiate(GenerateHair(unionDescriptors), spawnPoint.position, Quaternion.identity);
        npc.GetComponent<CharacterParts>().ChangeHead(head);
        npc.GetComponent<CharacterParts>().ChangeHair(hair);*/
    }

    public GameObject GenerateBase(string[] descriptors = null)
    {
        GameObject body = null;
        do
        {
            var chosenDescriptors = descriptors == null || descriptors.Length == 0 ? GetRandomDescriptors() : descriptors;
            body = GenerateBody(chosenDescriptors);
        } while (body == null);
        
        return body;
    }

    private GameObject GenerateBody(string[] descriptors)
    {
        var similarityCount = new List<int>();

        foreach (string[] bodyDescriptors in _bodiesDescriptors)
        {
            similarityCount.Add(GetSimilarDescriptorsCount(descriptors, bodyDescriptors));
        }

        if (similarityCount.Max() == 0) return null;
        var maxIndices = GetMaxIndicesArray(similarityCount);
        var selectedBodyIndex = GetRandomElementInList(maxIndices);
        return _bodies[selectedBodyIndex];
    }

    private GameObject GenerateHead(string[] descriptors)
    {
        var newDescriptors = GetAppropriateHeadDescriptors(descriptors);
        var similarityCount = new List<int>();

        foreach (string[] headDescriptors in _headsDescriptors)
        {
            similarityCount.Add(GetSimilarDescriptorsCount(newDescriptors, headDescriptors));
        }
        
        var maxIndices = GetMaxIndicesArray(similarityCount);
        var selectedHeadIndex = GetRandomElementInList(maxIndices);
        return _heads[selectedHeadIndex];
    }

    private GameObject GenerateHair(string[] descriptors)
    {
        var newDescriptors = GetAppropriateHairDescriptors(descriptors);
        var similarityCount = new List<int>();

        foreach (string[] hairDescriptors in _hairsDescriptors)
        {
            similarityCount.Add(GetSimilarDescriptorsCount(newDescriptors, hairDescriptors));
        }
        
        var maxIndices = GetMaxIndicesArray(similarityCount);
        var selectedHairIndex = GetRandomElementInList(maxIndices);
        return _hairs[selectedHairIndex];
    }
    
    private int[] GetMaxIndicesArray(List<int> similarityArray)
    {
        var maxSimilarityValue = similarityArray.Max();
        var maxIndices = new List<int>();
        for (int i = 0; i < similarityArray.Count(); i++)
        {
            if(similarityArray[i] == maxSimilarityValue)
                maxIndices.Add(i);
        }

        return maxIndices.ToArray();
    }

    private string[] GetAppropriateHeadDescriptors(string[] descriptors)
    {
        var newDescriptors = new List<string>();

        newDescriptors.Add(descriptors.Intersect(_races).Any()
            ? descriptors.Intersect(_races).First()
            : GetRandomElementInList(_races));

        newDescriptors.Add(descriptors.Intersect(_emotions).Any() ? descriptors.Intersect(_races).First() : "Normal");

        return newDescriptors.ToArray();
    }
    
    private string[] GetAppropriateHairDescriptors(string[] descriptors)
    {
        var newDescriptors = new List<string>();

        newDescriptors.Add(descriptors.Intersect(_sexes).Any()
            ? descriptors.Intersect(_sexes).First()
            : GetRandomElementInList(_sexes));

        newDescriptors.Add(descriptors.Intersect(_colors).Any()
            ? descriptors.Intersect(_colors).First()
            : GetRandomElementInList(_colors));

        return newDescriptors.ToArray();
    }

    private static T GetRandomElementInList<T>(T[] list)
    {
        var random = new Random();
        var index = UnityEngine.Random.Range(0, list.Length);
        return list[index];
    }

    private string[] GetRandomDescriptors()
    {
        var randomDescriptors = new List<string>();
        randomDescriptors.Add(GetRandomElementInList(_sexes));
        randomDescriptors.Add(GetRandomElementInList(_colors));
        randomDescriptors.Add(GetRandomElementInList(_roles));
        return randomDescriptors.ToArray();
    }

    private string[] GetPartDescriptors(GameObject part)
    {
        return part.name.Split('_');
    }

    private int GetSimilarDescriptorsCount(string[] target, string[] part)
    {
        var intersections = target.Intersect(part);
        if (intersections == null) return 0;
        return intersections.Count();
    }
}
