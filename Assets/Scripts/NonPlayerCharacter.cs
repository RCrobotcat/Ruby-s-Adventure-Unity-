using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NonPlayerCharacter : MonoBehaviour
{
    public float displayTime = 5.0f;
    public GameObject dialogBox;
    public GameObject tipBox;
    float timerDisplay;

    public TextMeshProUGUI DiaText;

    public AudioSource audioSource;
    public AudioClip questClip; // ���������Ч
    public AudioClip questStart; // ����ʼ��Ч
    public bool hasPlayed_1;
    public bool hasplayed;

    RubyController rubyController;

    // Start is called before the first frame update
    void Start()
    {
        dialogBox.SetActive(false);
        tipBox.SetActive(false);
        timerDisplay = -1.0f;

        rubyController = FindObjectOfType<RubyController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timerDisplay >= 0)
        {
            timerDisplay -= Time.deltaTime;
            if (timerDisplay < 0)
            {
                dialogBox.SetActive(false);
                rubyController.pressF = false;
            }
        }
    }

    // ������ʾ�Ի���
    public void DisplayDialog()
    {
        timerDisplay = displayTime;
        dialogBox.SetActive(true);
        UIHealthBar.instance.hasTask = true;
        UIBullet.instance.bulletUIvisibility(true);
        if(!hasplayed)
        {
            audioSource.PlayOneShot(questStart);
            hasplayed = true;
        }
        if (UIHealthBar.instance.fixedNum >= 12)
        {
            // �������
            DiaText.text = "Thank you for fixing those BROKEN ROBOTS!!";
            rubyController.pressF_success = true;
            if (!hasPlayed_1)
            {
                audioSource.PlayOneShot(questClip);
                hasPlayed_1 = true;
            }
        }
        else
        {
            DiaText.text = "Hi! \r\nHelp me fix those broken robots, really appreciate it!!";
        }
    }

    public void DisplayTip()
    {
        tipBox.SetActive(true); // ��ʾ��ʾ��
    }

    public void HideTip()
    {
        tipBox.SetActive(false); // ������ʾ��
    }
}
