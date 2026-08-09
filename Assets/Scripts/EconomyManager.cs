using UnityEngine;
using TMPro;
using System;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;

    [Header("Ekonomi Ayarları")]
    public int levelReward = 500;        // Bölümü geçince verilecek standart ödül
    public float costPerMeter = 50f;     // 1 birimlik (metre) çizginin maliyeti

    [Header("Arayüz (UI) Bağlantıları")]
    public TextMeshProUGUI totalMoneyText; // Sağ üstteki toplam para yazısı
    public TextMeshProUGUI currentCostText;  // O anki harcamayı gösteren yazı

    // Oyun sonu ekranında (Win Panel) gösterilecek kâr/zarar yazısı
    public TextMeshProUGUI profitText;

    [HideInInspector] public int totalMoney;
    [HideInInspector] public float currentLevelCost = 0f;

    void Awake()
    {
        // Singleton mantığı: Her yerden EconomyManager.Instance diyerek ulaşabilmemiz için
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Oyuncu oyuna ilk defa giriyorsa cebine 1000 Altın koyalım, girmiyorsa kayıtlı parasını çekelim
        totalMoney = PlayerPrefs.GetInt("TotalMoney", 1000);
        UpdateUI();
    }

    // Oyuncu çizgi çektikçe DrawManager burayı çağıracak
    public void AddCost(float lineLength)
    {
        float cost = lineLength * costPerMeter;
        currentLevelCost += cost;
        UpdateUI();
    }

    // Oyuncu silgiyle çizgiyi silerse parasının %100'ü (veya ileride istersen %50'si) iade edilecek
    public void RefundCost(float lineLength)
    {
        float refund = lineLength * costPerMeter;
        currentLevelCost -= refund;

        if (currentLevelCost < 0) currentLevelCost = 0; // Hata payını önlemek için
        UpdateUI();
    }

    // Kedi kurtulduğunda GameManager burayı çağıracak
    public void CalculateLevelProfit()
    {
        int costInt = Mathf.RoundToInt(currentLevelCost);
        int profit = levelReward - costInt;

        totalMoney += profit;

        // İleride eksi bakiyeye düşmeyi engellemek veya borç sistemi yapmak için temel kontrol
        if (totalMoney < 0) totalMoney = 0;

        // Yeni parayı telefona kaydet
        PlayerPrefs.SetInt("TotalMoney", totalMoney);
        PlayerPrefs.Save();

        // Kazanma ekranındaki (Win Panel) yazıyı güncelle
        if (profitText != null)
        {
            if (profit >= 0)
                profitText.text = "Ödül: " + levelReward + "\nMaliyet: -" + costInt + "\nKAZANÇ: +" + profit + " 🪙";
            else
                profitText.text = "Ödül: " + levelReward + "\nMaliyet: -" + costInt + "\nZARAR: " + profit + " 🪙\n(Daha kısa yollar bulmalısın!)";
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (totalMoneyText != null) totalMoneyText.text = "Kasa: " + totalMoney;
        if (currentCostText != null) currentCostText.text = "Harcanan: " + Mathf.RoundToInt(currentLevelCost);
    }
}