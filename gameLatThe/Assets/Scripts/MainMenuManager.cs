using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Level Data")]
    [SerializeField] private LevelData[] levels;

    [Header("UI References")]
    [SerializeField] private Transform levelGridParent;
    [SerializeField] private Button levelButtonPrefab;
    [SerializeField] private TMP_Dropdown difficultyDropdown;
    [SerializeField] private Toggle soundToggle;

    private void Start()
    {
        SetupSettingsUI();
        GenerateLevelButtons();
    }

    private void SetupSettingsUI()
    {
        if (difficultyDropdown != null)
        {
            difficultyDropdown.value = GameSettings.Difficulty;
            difficultyDropdown.onValueChanged.RemoveAllListeners();
            difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
        }

        if (soundToggle != null)
        {
            soundToggle.isOn = GameSettings.SoundEnabled;
            soundToggle.onValueChanged.RemoveAllListeners();
            soundToggle.onValueChanged.AddListener(OnSoundChanged);
        }

        AudioListener.volume = GameSettings.SoundEnabled ? 1f : 0f;
    }

    private void GenerateLevelButtons()
    {
        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("Bạn chưa kéo LevelData vào MainMenuManager.");
            return;
        }

        if (levelGridParent == null)
        {
            Debug.LogError("Bạn chưa kéo LevelGrid vào ô Level Grid Parent.");
            return;
        }

        if (levelButtonPrefab == null)
        {
            Debug.LogError("Bạn chưa kéo LevelButton prefab vào ô Level Button Prefab.");
            return;
        }

        foreach (Transform child in levelGridParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < levels.Length; i++)
        {
            int levelIndex = i;

            Button newButton = Instantiate(levelButtonPrefab, levelGridParent);
            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
            {
                buttonText.text = levels[i].levelName;
            }

            newButton.onClick.RemoveAllListeners();
            newButton.onClick.AddListener(() => PlayLevel(levelIndex));
        }
    }

    private void PlayLevel(int levelIndex)
    {
        GameSettings.SelectedLevelIndex = levelIndex;
        SceneManager.LoadScene("GameScene");
    }

    private void OnDifficultyChanged(int value)
    {
        GameSettings.Difficulty = value;
    }

    private void OnSoundChanged(bool isOn)
    {
        GameSettings.SoundEnabled = isOn;
        AudioListener.volume = isOn ? 1f : 0f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}