using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 附加到每个方格中，用于定义方格行为
/// </summary>
public class NumberSprite : MonoBehaviour
{
    private Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
    }
    public void SetImage(int number)
    {
        //2,4,8.... --> sprite --> 设置到Image中
        img.sprite = ResourceManager.LoadSprite(number);
    }

    public void CreateEffect()
    {
        //由小到大
        img.rectTransform.DOScale(0f, 0.3f).From();
    }

    public void MergeEffect()
    {
        img.rectTransform.DOScale(1.5f, 0.3f);
        img.rectTransform.DOScale(1f, 0.3f);
    }
}
