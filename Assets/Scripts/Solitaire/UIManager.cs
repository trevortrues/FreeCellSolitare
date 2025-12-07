using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameUIPanel;
    public GameObject rulesPanel;
    public GameObject themePanel;
    public GameObject winPanel;

    [Header("Game UI Elements")]
    public TextMeshProUGUI moveCounterText;
    public Button undoButton;
    public Button redoButton;

    [Header("Win Panel Elements")]
    public TextMeshProUGUI finalMoveCountText;

    private Solitaire solitaire;
    private MoveHistory moveHistory;

    void Start()
    {
        solitaire = FindAnyObjectByType<Solitaire>();
        moveHistory = FindAnyObjectByType<MoveHistory>();

        HideAllPanels();
        ShowMainMenu();
    }

    void Update()
    {
        UpdateButtonStates();
    }

    private void HideAllPanels()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameUIPanel != null) gameUIPanel.SetActive(false);
        if (rulesPanel != null) rulesPanel.SetActive(false);
        if (themePanel != null) themePanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }

    public void ShowMainMenu()
    {
        HideAllPanels();
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);

        SetGameObjectsVisible(false);
    }

    public void ShowGameUI()
    {
        HideAllPanels();
        if (gameUIPanel != null) gameUIPanel.SetActive(true);

        SetGameObjectsVisible(true);
        UpdateMoveCounter(0);
    }

    public void ShowRulesPanel()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (rulesPanel != null) rulesPanel.SetActive(true);
    }

    public void HideRulesPanel()
    {
        if (rulesPanel != null) rulesPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void ShowThemePanel()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (themePanel != null) themePanel.SetActive(true);
    }

    public void HideThemePanel()
    {
        if (themePanel != null) themePanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void ShowWinPanel(int finalMoveCount)
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            if (finalMoveCountText != null)
            {
                finalMoveCountText.text = "Moves: " + finalMoveCount;
            }
        }
    }

    public void HideWinPanel()
    {
        if (winPanel != null) winPanel.SetActive(false);
    }

    public void UpdateMoveCounter(int moves)
    {
        if (moveCounterText != null)
        {
            moveCounterText.text = "Moves: " + moves;
        }
    }

    private void UpdateButtonStates()
    {
        if (moveHistory != null)
        {
            if (undoButton != null) undoButton.interactable = moveHistory.CanUndo();
            if (redoButton != null) redoButton.interactable = moveHistory.CanRedo();
        }
    }

    private void SetGameObjectsVisible(bool visible)
    {
        if (solitaire == null) return;

        foreach (GameObject pos in solitaire.tableauPositions)
        {
            if (pos != null) pos.SetActive(visible);
        }
        foreach (GameObject pos in solitaire.foundationPositions)
        {
            if (pos != null) pos.SetActive(visible);
        }
        foreach (GameObject pos in solitaire.freeCellPositions)
        {
            if (pos != null) pos.SetActive(visible);
        }
    }

    public void OnStartGameClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewGame();
        }
    }

    public void OnRestartClicked()
    {
        HideWinPanel();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }

    public void OnMenuClicked()
    {
        HideWinPanel();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMenu();
        }
    }

    public void OnUndoClicked()
    {
        if (solitaire != null)
        {
            solitaire.UndoLastMove();
        }
    }

    public void OnRedoClicked()
    {
        if (solitaire != null)
        {
            solitaire.RedoMove();
        }
    }

    public void OnQuitClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
    }
}
