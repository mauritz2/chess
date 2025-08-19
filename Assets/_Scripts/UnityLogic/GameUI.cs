using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI gameOverText;

    public void SetGameOverText(string endGameMEssage)
    {
        gameOverText.text = endGameMEssage;
    }
}
