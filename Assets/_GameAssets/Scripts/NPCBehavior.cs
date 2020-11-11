using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    private Animator _animator;
    private GameObject _eventPanel;
    private DOTweenPath _path;
    private Tween _pathTween;

    private void Awake()
    {
        _eventPanel = GameObject.FindWithTag("EventPanel").transform.GetChild(0).gameObject;
        _animator = GetComponent<Animator>();
        _path = GetComponentInParent<DOTweenPath>();
    }

    private void Start()
    {
        _animator.SetTrigger("Walk");
        _pathTween = _path.GetTween();
        _pathTween.OnWaypointChange(OnReachingWaypoint);
        _pathTween.Play();
    }

    private void OnReachingWaypoint(int index)
    {
        if (index != 3) return;
        _animator.SetTrigger("Idle");
        _eventPanel.SetActive(true);
        _pathTween.Pause();
    }

    public void CloseEventPanel()
    {
        _eventPanel.SetActive(false);
        _animator.SetTrigger("Walk");
        _pathTween.Play();
    }
}
