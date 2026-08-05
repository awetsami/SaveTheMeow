using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizeUI : MonoBehaviour
{
    [Header("Çeviriler")]
    [TextArea(2, 3)] public string trText; // Türkçe metin
    [TextArea(2, 3)] public string enText; // İngilizce metin

    private TextMeshProUGUI textComp;

    void Start()
    {
        textComp = GetComponent<TextMeshProUGUI>();
        UpdateText(); // Başlangıçta doğru dili yaz
        LanguageManager.OnLanguageChanged += UpdateText; // Dil değiştiğinde beni haberdar et
    }

    void OnDestroy()
    {
        // Obje silinirken aboneliği iptal et ki hata vermesin
        LanguageManager.OnLanguageChanged -= UpdateText;
    }

    public void UpdateText()
    {
        if (textComp == null) return;

        if (LanguageManager.CurrentLanguage == "TR")
        {
            textComp.text = trText;
        }
        else if (LanguageManager.CurrentLanguage == "EN")
        {
            textComp.text = enText;
        }
    }
}