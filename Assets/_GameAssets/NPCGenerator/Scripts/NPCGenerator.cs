using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCGenerator : MonoBehaviour
{
    private static GameObject[] _bodies;
    private static GameObject[] _heads;
    private static GameObject[] _hairs;
    private static readonly List<string[]> BodiesDescriptors = new List<string[]>();
    private static readonly List<string[]> HeadsDescriptors = new List<string[]>();
    private static readonly List<string[]> HairsDescriptors = new List<string[]>();
    private static readonly string[] Races = new[] {"African", "Caucasian"};
    private static readonly string[] Sexes = new[] {"Male", "Female"};
    private static readonly string[] Colors = new[] {"Red", "White", "Black", "Purple", "Green", "Blue", "Pink", "Yellow", "Orange", "Brown"};
    private static readonly string[] Emotions = new[] {"Normal", "Shy", "Shock", "Crying"};
    private static readonly string[] Roles = new[] {"Army", "Citizen", "General", "Hunter", "Royal", "Noble", "Barbarian", "King"};

    private void Awake()
    {
        _bodies = Resources.LoadAll<GameObject>("Parts/Body");
        _heads = Resources.LoadAll<GameObject>("Parts/Head");
        _hairs = Resources.LoadAll<GameObject>("Parts/Hair");

        foreach (GameObject body in _bodies)
        {
            BodiesDescriptors.Add(GetPartDescriptors(body));
        }

        foreach (GameObject head in _heads)
        {
            HeadsDescriptors.Add(GetPartDescriptors(head));
        }
        
        foreach (GameObject hair in _hairs)
        {
            HairsDescriptors.Add(GetPartDescriptors(hair));
        }
    }

    public static GameObject GenerateNPC(string[] descriptors, Transform location)
    {
        var body = GenerateBase(descriptors);
        var npc = Instantiate(body, location);
        var unionDescriptors = descriptors.Union(GetPartDescriptors(body)).ToArray();
        var head = GenerateHead(unionDescriptors);
        var hair = GenerateHair(unionDescriptors);
        npc.GetComponent<CharacterParts>().ChangeHead(head);
        npc.GetComponent<CharacterParts>().ChangeHair(hair);
        return npc;
    }

    private static GameObject GenerateBase(string[] descriptors = null)
    {
        GameObject body = null;
        do
        {
            var chosenDescriptors = descriptors == null || descriptors.Length == 0 ? GetRandomDescriptors() : descriptors;
            body = GenerateBody(chosenDescriptors);
        } while (body == null);
        
        return body;
    }

    private static GameObject GenerateBody(string[] descriptors)
    {
        var similarityCount = new List<int>();

        foreach (string[] bodyDescriptors in BodiesDescriptors)
        {
            similarityCount.Add(GetSimilarDescriptorsCount(descriptors, bodyDescriptors));
        }

        if (similarityCount.Max() == 0) return null;
        var maxIndices = GetMaxIndicesArray(similarityCount);
        var selectedBodyIndex = GetRandomElementInList(maxIndices);
        return _bodies[selectedBodyIndex];
    }

    private static GameObject GenerateHead(string[] descriptors)
    {
        var newDescriptors = GetAppropriateHeadDescriptors(descriptors);
        var similarityCount = new List<int>();

        foreach (string[] headDescriptors in HeadsDescriptors)
        {
            similarityCount.Add(GetSimilarDescriptorsCount(newDescriptors, headDescriptors));
        }
        
        var maxIndices = GetMaxIndicesArray(similarityCount);
        var selectedHeadIndex = GetRandomElementInList(maxIndices);
        return _heads[selectedHeadIndex];
    }

    private static GameObject GenerateHair(string[] descriptors)
    {
        var newDescriptors = GetAppropriateHairDescriptors(descriptors);
        var similarityCount = new List<int>();

        foreach (string[] hairDescriptors in HairsDescriptors)
        {
            similarityCount.Add(GetSimilarDescriptorsCount(newDescriptors, hairDescriptors));
        }
        
        var maxIndices = GetMaxIndicesArray(similarityCount);
        var selectedHairIndex = GetRandomElementInList(maxIndices);
        return _hairs[selectedHairIndex];
    }
    
    private static int[] GetMaxIndicesArray(List<int> similarityArray)
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

    private static string[] GetAppropriateHeadDescriptors(string[] descriptors)
    {
        var newDescriptors = new List<string>();

        newDescriptors.Add(descriptors.Intersect(Races).Any()
            ? descriptors.Intersect(Races).First()
            : GetRandomElementInList(Races));

        newDescriptors.Add(descriptors.Intersect(Emotions).Any() ? descriptors.Intersect(Races).First() : "Normal");

        return newDescriptors.ToArray();
    }
    
    private static string[] GetAppropriateHairDescriptors(string[] descriptors)
    {
        var newDescriptors = new List<string>();

        newDescriptors.Add(descriptors.Intersect(Sexes).Any()
            ? descriptors.Intersect(Sexes).First()
            : GetRandomElementInList(Sexes));

        newDescriptors.Add(descriptors.Intersect(Colors).Any()
            ? descriptors.Intersect(Colors).First()
            : GetRandomElementInList(Colors));

        return newDescriptors.ToArray();
    }

    private static T GetRandomElementInList<T>(T[] list)
    {
        var random = new Random();
        var index = UnityEngine.Random.Range(0, list.Length);
        return list[index];
    }

    private static string[] GetRandomDescriptors()
    {
        var randomDescriptors = new List<string>();
        randomDescriptors.Add(GetRandomElementInList(Sexes));
        randomDescriptors.Add(GetRandomElementInList(Colors));
        randomDescriptors.Add(GetRandomElementInList(Roles));
        return randomDescriptors.ToArray();
    }

    private static string[] GetPartDescriptors(GameObject part)
    {
        return part.name.Split('_');
    }

    private static int GetSimilarDescriptorsCount(string[] target, string[] part)
    {
        var intersections = target.Intersect(part);
        return intersections.Count();
    }
}
