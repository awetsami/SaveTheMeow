using UnityEngine;
using TMPro;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;

    [Header("Ekonomi Ayarları")]
    public int levelReward = 500;        // Bölümü geçince verilecek ödül

    [Tooltip("Sırasıyla: 0=Ayna, 1=Taş, 2=Silgi, 3=SiyahCam, 4=Kristal")]
    public float[] penCosts = { 50f, 25f, 0f, 400f, 200f }; // Metre başına fiyatlar

    [Header("Arayüz (UI) Bağlantıları")]
    public TextMeshProUGUI totalMoneyText;
    public TextMeshProUGUI currentCostText;
    public TextMeshProUGUI profitText;

    [HideInInspector] public int totalMoney;
    [HideInInspector] public float currentLevelCost = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        totalMoney = PlayerPrefs.GetInt("TotalMoney", 1000);
        UpdateUI();
    }

    // YENİ: Hangi kalemin metresi ne kadar? (DrawManager buraya soracak)
    public float GetLineCost(float length, int penType)
    {
        if (penType < 0 || penType >= penCosts.Length) return 0f;
        return length * penCosts[penType];
    }

    public void AddCost(float cost)
    {
        currentLevelCost += cost;
        UpdateUI();
    }

    public void RefundCost(float cost)
    {
        currentLevelCost -= cost;
        if (currentLevelCost < 0) currentLevelCost = 0;
        UpdateUI();
    }

    public void CalculateLevelProfit()
    {
        int costInt = Mathf.RoundToInt(currentLevelCost);
        int profit = levelReward - costInt;

        totalMoney += profit;

        if (totalMoney < 0) totalMoney = 0;

        PlayerPrefs.SetInt("TotalMoney", totalMoney);
        PlayerPrefs.Save();

        if (profitText != null)
        {
            if (profit >= 0)
                profitText.text = "Ödül: " + levelReward + "\nMaliyet: -" + costInt + "\nKAZANÇ: +" + profit + " 🪙";
            else
                profitText.text = "Ödül: " + levelReward + "\nMaliyet: -" + costInt + "\nZARAR: " + profit + " 🪙\n(Daha ucuz yollar bulmalısın!)";
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (totalMoneyText != null) totalMoneyText.text = "Kasa: " + totalMoney;
        if (currentCostText != null) currentCostText.text = "Harcanan: " + Mathf.RoundToInt(currentLevelCost);
    }
}