﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Doom_UI : MonoBehaviour
{
    [SerializeField]
    RectTransform hpBar, uhpBar;

    void Update()
    {
        hpBar.sizeDelta = new Vector2(uhpBar.sizeDelta.x * Doom_Player.hp / 100f, uhpBar.sizeDelta.y);
    }
}
