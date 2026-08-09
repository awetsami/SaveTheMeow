using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panelleri")]
    public GameObject winPanel;
    public GameObject losePanel;
    public TextMeshProUGUI countdownText;

    [Header("Sistemler")]
    public DrawManager drawManager;

    [Header("Kedi Takip Sistemi")]
    public int totalCatsInLevel = 0;
    public int freedCatsCount = 0;

    [HideInInspector] public bool isGameOver = false;
    private Coroutine countdownCoroutine;
    private bool isCounting = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // YENİ: Oyun başladığında sahnedeki tüm kedileri bul ve say
        totalCatsInLevel = FindObjectsByType<CatLogic>(FindObjectsSortMode.None).Length;
        freedCatsCount = 0;
    }

    // --- YENİ KEDİ YOKLAMA SİSTEMİ ---

    public void CatFreed()
    {
        freedCatsCount++;

        // Sadece tüm kediler kurtulduysa sayacı başlat
        if (freedCatsCount >= totalCatsInLevel)
        {
            StartFreedomCountdown();
        }
    }

    public void CatTrapped()
    {
        freedCatsCount--;

        // Eğer bir kedi bile tekrar hapsolursa (veya yoluna taş çizilirse) sayacı iptal et
        CancelFreedomCountdown();
    }

    // ---------------------------------

    public void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("KAYBETTİN!");

        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);

        countdownText.gameObject.SetActive(false);
        losePanel.SetActive(true);

        if (drawManager != null) drawManager.enabled = false;
    }

    // Not: Artık CatLogic doğrudan bu fonksiyonları çağırmayacak, CatFreed'i çağıracak
    public void StartFreedomCountdown()
    {
        if (isGameOver || isCounting) return;
        countdownCoroutine = StartCoroutine(CountdownRoutine());
    }

    public void CancelFreedomCountdown()
    {
        if (isGameOver || !isCounting) return;

        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);
        isCounting = false;
        countdownText.gameObject.SetActive(false);
        Debug.Log("Sayım iptal edildi! Kedilerden biri hala hapis.");
    }

    IEnumerator CountdownRoutine()
    {
        isCounting = true;
        countdownText.gameObject.SetActive(true);

        countdownText.text = "3";
        yield return new WaitForSeconds(1f);
        if (isGameOver) yield break;

        countdownText.text = "2";
        yield return new WaitForSeconds(1f);
        if (isGameOver) yield break;

        countdownText.text = "1";
        yield return new WaitForSeconds(1f);
        if (isGameOver) yield break;

        countdownText.gameObject.SetActive(false);
        TriggerWin();
    }

    private void TriggerWin()
    {
        isGameOver = true;
        Debug.Log("KAZANDIN!");

        winPanel.SetActive(true);
        if (drawManager != null) drawManager.enabled = false;

        if (EconomyManager.Instance != null) EconomyManager.Instance.CalculateLevelProfit();
    }

    // --- BUTON FONKSİYONLARI VE KAYIT SİSTEMİ ---

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        int reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);
        if (nextSceneIndex > reachedLevel)
        {
            PlayerPrefs.SetInt("ReachedLevel", nextSceneIndex);
            PlayerPrefs.Save();
        }

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextSceneIndex);
        else
            MainMenu();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}