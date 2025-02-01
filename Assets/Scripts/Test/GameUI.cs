using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text scoreText;
    public GameObject warningPanel;

    void Update()
    {
        timeText.text = $"Time: {Time.timeSinceLevelLoad:0.0}s";
    }

    public void ShowWarning(string message)
    {
        warningPanel.SetActive(true);
        warningPanel.GetComponentInChildren<TMP_Text>().text = message;
        Invoke(nameof(HideWarning), 2f);
    }

    void HideWarning()
    {
        warningPanel.SetActive(false);
    }
}
