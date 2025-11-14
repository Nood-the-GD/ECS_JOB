using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private TextMeshProUGUI _enemyCountText;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateEnemyCount(int count)
    {
        _enemyCountText.text = $"Enemy count: {count}";
    }
}