using UnityEngine;

[CreateAssetMenu(fileName = "New Level Data", menuName = "Memory Match/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public int levelIndex = 1;
    public string levelName = "Level 1";

    [Header("Board Size")]
    [Min(2)] public int rows = 2;
    [Min(2)] public int columns = 2;

    [Header("Gameplay")]
    public float timeLimit = 60f;
    public int baseScorePerMatch = 100;
    public float hintCooldown = 10f;

    public int TotalCards => rows * columns;
    public int TotalPairs => TotalCards / 2;

    private void OnValidate()
    {
        if ((rows * columns) % 2 != 0)
        {
            Debug.LogWarning($"{levelName}: Tổng số thẻ phải là số chẵn để ghép đôi được.");
        }
    }
}