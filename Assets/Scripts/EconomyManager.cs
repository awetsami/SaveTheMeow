using UnityEngine;
using TMPro;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;

    [Header("Çeviriler - Kasa Arayüzü")]
    public string kasaTR = "Kasa: ";
    public string kasaEN = "Bank: ";

    [Header("Çeviriler - Bölüm Sonu Özeti")]
    public string gelirTR = "Bölüm Ödülü: ";
    public string gelirEN = "Level Reward: ";

    public string giderTR = "Maliyet: ";
    public string giderEN = "Total Cost: ";

    public string karTR = "KAZANÇ: ";
    public string karEN = "PROFIT: ";

    public string zararTR = "ZARAR: ";
    public string zararEN = "LOSS: ";

    [Header("Çeviriler - Uyarı Mesajı")]
    public string uyariTR = "(Daha ucuz yollar bulmalısın!)";
    public string uyariEN = "(You should find cheaper ways!)";

    [Header("Arayüz (UI) Bağlantıları")]
    public TextMeshProUGUI totalMoneyText;
    public TextMeshProUGUI currentCostText;
    public TextMeshProUGUI profitText;

    [HideInInspector] public int totalMoney;

    [HideInInspector] public float currentLevelCost = 0f;

    [Header("Ekonomi Ayarları")]
    public int levelReward = 100;

    [Tooltip("Oyuna ilk defa başlayan oyuncuya verilecek başlangıç parası")]
    public int startingMoney = 200;

    public float[] penCosts = { 50f, 25f, 0f, 400f, 200f };

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // DÜZELTME: Artık 1000 yerine Inspector'dan girdiğin startingMoney (Örn: 200) değerini okuyor
        totalMoney = PlayerPrefs.GetInt("TotalMoney", startingMoney);
        UpdateUI();
    }

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

        // --- YENİ ÇEVİRİ SİSTEMİ ---
        if (profitText != null)
        {
            bool isTR = (LanguageManager.CurrentLanguage == "TR");

            string secilenGelir = isTR ? gelirTR : gelirEN;
            string secilenGider = isTR ? giderTR : giderEN;
            string secilenUyari = isTR ? uyariTR : uyariEN;

            if (profit >= 0)
            {
                string secilenKar = isTR ? karTR : karEN;
                profitText.text = secilenGelir + levelReward + "\n" +
                                  secilenGider + "-" + costInt + "\n" +
                                  secilenKar + "+" + profit + " 🪙";
            }
            else
            {
                string secilenZarar = isTR ? zararTR : zararEN;
                profitText.text = secilenGelir + levelReward + "\n" +
                                  secilenGider + "-" + costInt + "\n" +
                                  secilenZarar + profit + " 🪙\n" + secilenUyari;
            }
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        bool isTR = (LanguageManager.CurrentLanguage == "TR");

        // 1. Kasa Yazısı Çevirisi
        string prefix = isTR ? kasaTR : kasaEN;
        if (totalMoneyText != null)
        {
            totalMoneyText.text = prefix + totalMoney.ToString();
        }

        // 2. Anlık Harcama (currentCostText) Çevirisi
        if (currentCostText != null)
        {
            string costPrefix = isTR ? giderTR : giderEN;
            currentCostText.text = costPrefix + Mathf.RoundToInt(currentLevelCost).ToString();
        }
    }
}