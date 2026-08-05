using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menü Panelleri")]
    public GameObject levelMapPanel;
    public GameObject settingsPanel;

    [Header("Harita Butonları")]
    public Button[] levelButtons;

    void Start()
    {
        // Başlangıçta yan panelleri gizle
        if (levelMapPanel != null) levelMapPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    // --- BÖLÜM HARİTASI (SCROLL MAP) ---
    public void OpenLevelMap()
    {
        levelMapPanel.SetActive(true);
        int reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            levelButtons[i].interactable = (levelIndex <= reachedLevel);
        }
    }

    public void CloseLevelMap()
    {
        levelMapPanel.SetActive(false);
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    // --- AYARLAR ---
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    // --- OYUNU SIFIRLAMA ---
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("ReachedLevel");
        CloseLevelMap();
        OpenLevelMap();
    }
}