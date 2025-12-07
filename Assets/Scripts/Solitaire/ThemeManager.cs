using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }

    [Header("Theme Colors")]
    public Color[] backgroundColors = new Color[]
    {
        new Color(0.5f, 0.3f, 0.7f, 1f),
        new Color(0.2f, 0.5f, 0.3f, 1f),
        new Color(0.2f, 0.3f, 0.6f, 1f),
        new Color(0.6f, 0.2f, 0.2f, 1f),
        new Color(0.3f, 0.3f, 0.3f, 1f),
    };

    [Header("UI References")]
    public Button[] colorButtons;

    private Camera mainCamera;
    private const string THEME_KEY = "BackgroundColorIndex";

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
        mainCamera = Camera.main;

        int savedIndex = PlayerPrefs.GetInt(THEME_KEY, 0);
        ApplyTheme(savedIndex);

        SetupButtons();
    }

    private void SetupButtons()
    {
        if (colorButtons == null) return;

        for (int i = 0; i < colorButtons.Length; i++)
        {
            if (colorButtons[i] != null)
            {
                int index = i;
                colorButtons[i].onClick.AddListener(() => SetTheme(index));

                if (i < backgroundColors.Length)
                {
                    var buttonImage = colorButtons[i].GetComponent<Image>();
                    if (buttonImage != null)
                    {
                        buttonImage.color = backgroundColors[i];
                    }
                }
            }
        }
    }

    public void SetTheme(int colorIndex)
    {
        if (colorIndex < 0 || colorIndex >= backgroundColors.Length) return;

        ApplyTheme(colorIndex);

        PlayerPrefs.SetInt(THEME_KEY, colorIndex);
        PlayerPrefs.Save();
    }

    private void ApplyTheme(int colorIndex)
    {
        if (mainCamera != null && colorIndex >= 0 && colorIndex < backgroundColors.Length)
        {
            mainCamera.backgroundColor = backgroundColors[colorIndex];
        }
    }
}
