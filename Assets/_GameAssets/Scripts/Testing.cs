using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject hair;
    private CharacterParts _characterParts;
    
    void Start()
    {
        _characterParts = GetComponent<CharacterParts>();
        _characterParts.ChangeHead(head);
        _characterParts.ChangeHair(hair);
    }
}
