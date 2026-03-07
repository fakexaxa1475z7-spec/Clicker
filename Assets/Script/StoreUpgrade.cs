using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreUpgrade : MonoBehaviour
{
    [Header("Components")]
    public TMP_Text priceText;
    public TMP_Text incomeInfoText;
    public Button button;
    public Image icon;
    public TMP_Text name;
    public GameObject cat;

    [Header("Generator Values")]
    public string upgradeName;
    public int startPrice = 15;
    public float priceMultiplier = 1.15f;
    public float powerPerUpgrade = 0.1f;

    [Header("Manager")]
    public GameManager gameManager;

    public int Level { get; private set; } = 0;

    void Start()
    {
        UpdateUI();
    }

    public void Upgrade()
    {
        int price = CalculatedPrice();
        bool purchased = gameManager.PurchaseAction(price);
        if (purchased)
        {
            Level++;
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        priceText.text = CalculatedPrice().ToString();
        incomeInfoText.text = calculateIncomePerSecond().ToString("F1") + "/s";

        bool canAfford = gameManager.power >= CalculatedPrice();
        button.interactable = canAfford;

        bool isPurchased = Level > 0;
        icon.color = isPurchased ? Color.white : Color.black;
        name.text = isPurchased ? upgradeName : "Locked";
            cat.SetActive(isPurchased);
    }

    public void ResetUpgrade()
    {
        Level = 0;
        UpdateUI();
    }

    public void SetLevel(int level)
    {
        Level = level;
        UpdateUI();
    }

    int CalculatedPrice()
    {
        return Mathf.RoundToInt(startPrice * Mathf.Pow(priceMultiplier, Level));
    }

    public float calculateIncomePerSecond()
    {
        return powerPerUpgrade * Level;
    }
}