using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    private void OnEnable()
    {
        GameManager.Instance.OnScoreChanged += UpdateScoreUI;
        UpdateScoreUI(GameManager.Instance.Score);
    }

    private void OnDisable()
    {
        GameManager.Instance.OnScoreChanged -= UpdateScoreUI;
    }

    private void UpdateScoreUI(int newScore)
    {
        scoreText.text = newScore.ToString();
    }
}
