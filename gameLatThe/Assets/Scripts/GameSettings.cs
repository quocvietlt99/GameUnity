using UnityEngine;

public static class GameSettings
{
    private const string SelectedLevelKey = "SelectedLevel";
    private const string SoundKey = "SoundEnabled";
    private const string DifficultyKey = "Difficulty";

    public static int SelectedLevelIndex
    {
        get => PlayerPrefs.GetInt(SelectedLevelKey, 0);
        set
        {
            PlayerPrefs.SetInt(SelectedLevelKey, value);
            PlayerPrefs.Save();
        }
    }

    public static bool SoundEnabled
    {
        get => PlayerPrefs.GetInt(SoundKey, 1) == 1;
        set
        {
            PlayerPrefs.SetInt(SoundKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static int Difficulty
    {
        get => PlayerPrefs.GetInt(DifficultyKey, 1);
        set
        {
            PlayerPrefs.SetInt(DifficultyKey, value);
            PlayerPrefs.Save();
        }
    }
}