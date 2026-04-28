using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public void Play()
    {
        if (!PlayerPrefs.HasKey("SelectedLevel"))
            PlayerPrefs.SetInt("SelectedLevel", 1);

        SceneManager.LoadScene("GameScene");
    }

    public void SelectLevel(int level)
    {
        int unlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);
        if (level > unlocked) return;

        PlayerPrefs.SetInt("SelectedLevel", level);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameScene");
    }

    public void SetSound(bool isOn)
    {
        PlayerPrefs.SetInt("SoundOn", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}