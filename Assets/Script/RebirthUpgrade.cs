using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RebirthUpgrade : MonoBehaviour
{
    [Header("Components")]
    public TMP_Text rebirthCostText;
    public TMP_Text rebirthCountText;
    public Button button;

    [Header("Value")]
    int rebirthCost = 1000;
    public float priceMultiplier = 1.15f;

    [Header("Manager")]
    public GameManager gameManager;

    public int Level { get; private set; } = 0;

    void Start()
    {
        UpdateUI();
    }

    public void SetLevel(int level)
    {
        Level = level;
        UpdateUI();
    }

    public void Rebirth()
    {
        if (gameManager.power >= rebirthCost)
        {
            gameManager.power = 0;
            gameManager.rebirths++;
            Level++;
            gameManager.ResetUpgrades();
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        rebirthCostText.text = CalculatedPrice().ToString();
        rebirthCountText.text = gameManager.rebirths.ToString();

        bool canAfford = gameManager.power >= CalculatedPrice();
        button.interactable = canAfford;
    }

    int CalculatedPrice()
    {
        int price = Mathf.RoundToInt(rebirthCost * Mathf.Pow(priceMultiplier, Level));
        return price;
    }
}
