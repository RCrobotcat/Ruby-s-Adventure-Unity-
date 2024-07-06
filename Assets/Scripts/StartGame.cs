using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    public AudioSource BGM;
    public Slider volumeSlider; // ��������
    public void StartMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // ������һ������
        if(Time.timeScale == (0))
        {
            Time.timeScale = (1); // �ָ���Ϸ
        }
    }

    public void QuitGame()
    {
        Application.Quit(); // �˳���Ϸ
    }
    // Start is called before the first frame update
    void Start()
    {
        // ���ر��������
        if (PlayerPrefs.HasKey("MusicVolume_title"))
        {
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume_title");
            BGM.volume = savedVolume;
            volumeSlider.value = savedVolume;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnVolumeChange()
    {
        BGM.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("MusicVolume_title", volumeSlider.value); // ��������
        PlayerPrefs.Save();
    }
}
