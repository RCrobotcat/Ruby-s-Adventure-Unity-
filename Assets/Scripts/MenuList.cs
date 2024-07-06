using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuList : MonoBehaviour
{
    public GameObject menuList; // �˵��б�

    [SerializeField] private bool menuKey = true;
    [SerializeField] private AudioSource BGM;
    [SerializeField] private Slider volumeSlider; // ��������

    RubyController rubyController;

    // Start is called before the first frame update
    void Start()
    {
        rubyController = FindObjectOfType<RubyController>();
        menuKey = !menuList.activeSelf; // ��ʼ״̬�²˵��б��ǹرյ�

        // ���ر��������
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume");
            BGM.volume = savedVolume;
            volumeSlider.value = savedVolume;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuKey)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None; // ��ʾ���
                menuList.SetActive(true);
                menuKey = false;
                Time.timeScale = (0); // ��ͣ��Ϸ
                BGM.Pause();
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked; // �������
                menuList.SetActive(false);
                menuKey = true;
                Time.timeScale = (1); // �ָ���Ϸ
                BGM.Play();
            }
        }
    }

    public void OnVolumeChange()
    {
        BGM.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("MusicVolume", volumeSlider.value); // MusicVolume �Ǽ���volumeSlider.value ��ֵ
        PlayerPrefs.Save(); // ȷ�����ı�����
    }

    public void Return()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // �������
        menuList.SetActive(false);
        menuKey = true;
        Time.timeScale = (1); // �ָ���Ϸ
        BGM.Play();
    }

    public void Restart() // ���¿�ʼ
    {
        SceneManager.LoadScene(1);
        if(rubyController.pressF_success == true)
        {
            rubyController.pressF_success = false;
        }
        if(rubyController.isDead == true)
        {
            rubyController.isDead = false;
        }
        Time.timeScale = (1); // �ָ���Ϸ
    }

    public void MainMenu() // �������˵�
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1); // ������һ������
    }

    public void Quit()
    {
        Application.Quit();
    }
}
