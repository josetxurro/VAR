using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartButton : MonoBehaviour
{
    public void OnButtonPressed()
    {
        Debug.Log("🟢 UI BUTTON PRESSED!");
    }
    
    public void RestartGame()
    {
        Debug.Log("🔄 Game Restarted");

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetScore();

        if (TargetSpawner.Instance != null)
            TargetSpawner.Instance.ResetTargets();
    }
}
