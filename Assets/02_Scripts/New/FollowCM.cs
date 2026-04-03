using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowCM : MonoBehaviour
{
    RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        if (GameFacade.Instance.Player == null || GameFacade.Instance == null) return; 
        rect.position = Camera.main.WorldToScreenPoint(GameFacade.Instance.Player.transform.position);
    }
}
