using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private TextAsset[] levelFiles;

    public void LoadSelectedLevel()
    {
       /* int selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        selectedLevel = Mathf.Clamp(selectedLevel, 1, levelFiles.Length);

        int shots = 28 + selectedLevel * 2;
        int colorCount = Mathf.Min(3 + (selectedLevel - 1) / 3, 5);

        GameManager.Instance.ConfigureLevel(selectedLevel, shots, colorCount);
        GridManager.Instance.LoadLevel(levelFiles[selectedLevel - 1]);
    }

    public void UnlockNextLevel(int currentLevel)
    {
        int unlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);
        int next = Mathf.Min(currentLevel + 1, levelFiles.Length);

        if (next > unlocked)
        {
            PlayerPrefs.SetInt("UnlockedLevel", next);
            PlayerPrefs.Save();
        }*/
    }
}