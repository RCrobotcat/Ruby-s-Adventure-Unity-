using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public static UIHealthBar instance { get; private set; } // ����ģʽ

    public Image mask;
    float originalSize;

    public bool hasTask; // �Ƿ�������
    // public bool ifCompleteTask; // �Ƿ��������, Ĭ��Ϊ0
    public int fixedNum = 0; // �޸��Ļ���������

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        originalSize = mask.rectTransform.rect.width;
    }

    public void SetValue(float value)
    {
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * value);
    }
}
