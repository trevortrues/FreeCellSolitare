using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { MainMenu, Playing, Paused, Won }
    public GameState currentState = GameState.MainMenu;

    private Solitaire solitaire;
    private UIManager uiManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        solitaire = FindAnyObjectByType<Solitaire>();
        uiManager = FindAnyObjectByType<UIManager>();

        if (uiManager != null)
        {
            uiManager.ShowMainMenu();
        }
    }

    public void StartNewGame()
    {
        currentState = GameState.Playing;

        if (solitaire != null)
        {
            solitaire.ResetGame();
        }

        if (uiManager != null)
        {
            uiManager.ShowGameUI();
        }
    }

    public void RestartGame()
    {
        if (solitaire != null)
        {
            solitaire.ResetGame();
        }

        currentState = GameState.Playing;

        if (uiManager != null)
        {
            uiManager.UpdateMoveCounter(0);
        }
    }

    public void ReturnToMenu()
    {
        currentState = GameState.MainMenu;

        if (uiManager != null)
        {
            uiManager.ShowMainMenu();
        }
    }

    public void OnGameWon()
    {
        currentState = GameState.Won;

        if (uiManager != null)
        {
            uiManager.ShowWinPanel(solitaire != null ? solitaire.moveCount : 0);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public bool IsPlaying()
    {
        return currentState == GameState.Playing;
    }

    [ContextMenu("Test Win Screen")]
    public void TestWinScreen()
    {
        OnGameWon();
    }
}
