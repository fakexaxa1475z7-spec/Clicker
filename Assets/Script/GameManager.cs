using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] TMP_Text powerText;
    [SerializeField] TMP_Text incomeText;
    [SerializeField] TMP_Text multiplierText;
    [SerializeField] TMP_Text BaseText;
    [SerializeField] StoreUpgrade[] storeUpgrades;
    [SerializeField] ClickUpgrade[] clickUpgrades;
    [SerializeField] BaseUpgrade[] baseUpgrades;
    [SerializeField] int updatePerSecond = 5;

    [Header("Floating Text")]
    [SerializeField] GameObject floatingTextPrefab;
    [SerializeField] Canvas mainCanvas;

    [Header("Background Tier System")]
    [SerializeField] Image backgroundImage;
    [SerializeField] Image fadeImage;
    [SerializeField] Sprite[] backgroundTiers;
    [SerializeField] int levelPerTier = 5;
    [SerializeField] float fadeDuration = 0.5f;

    bool isFading = false;

    [Header("Value")]
    [HideInInspector] public float power = 0;
    [HideInInspector] public int rebirths = 0;
    int baseIncome = 1;
    float ClickMultiplier = 1f;
    float nextTime = 1f;
    float LastIncome = 0f;
    float RebirthMultiplier => 1 + (rebirths * 0.5f);
    float powerPerClick => baseIncome * (RebirthMultiplier + ClickMultiplier);

    [Header("Manager")]
    public RebirthUpgrade rebirthUpgrade;
    public OfflineIncomePopup offlinePopup;
    [SerializeField] QuitPopupUI quitPopup;

    void Start()
    {
        LoadGame();
        UpdateBackground();
        UpdateUI();
    }

    void Update()
    {
        if (Keyboard.current != null &&
        Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("ESC Pressed");

            if (quitPopup.IsOpen)
                quitPopup.Hide();
            else
                quitPopup.Show();
        }

        if (Time.timeScale == 0f)
            return;

        if (nextTime < Time.timeSinceLevelLoad)
        { 
            IdleIncome(1f);
            ClickIncome();
            BaseIncome();
            rebirthUpgrade.UpdateUI();
            nextTime = Time.timeSinceLevelLoad + (1f / updatePerSecond);
        }
    }

    void IdleIncome(float incomePerSecond)
    {
        float sum = 0;
        foreach (StoreUpgrade upgrade in storeUpgrades)
        {
            sum += upgrade.calculateIncomePerSecond();
            upgrade.UpdateUI();
        }
        LastIncome = sum;
        power += sum / updatePerSecond;
        UpdateUI();
    }

    void ClickIncome()
    {
        float sum = 0;
        foreach (ClickUpgrade upgrade in clickUpgrades)
        {
            sum += upgrade.calculateMultiplier();
            upgrade.UpdateUI();
        }
        ClickMultiplier = sum;
        UpdateUI();
    }

    void BaseIncome()
    {
        foreach (BaseUpgrade upgrade in baseUpgrades)
        {
            upgrade.UpdateUI();
        }
        UpdateUI();
    }

    public void BaseUpgradeIncome()
    {
        baseIncome += 1;
    }

    public void ClickAction()
    {
        float amount = powerPerClick;
        power += amount;

        ShowFloatingText("+" + Mathf.RoundToInt(amount));

        UpdateUI();
    }

    public bool PurchaseAction(int cost)
    {
        if (power >= cost)
        {
            power -= cost;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void UpdateUI()
    {
        powerText.text = Mathf.RoundToInt(power).ToString();
        incomeText.text = LastIncome.ToString("F1") + "/s";
        multiplierText.text = "x " + (RebirthMultiplier + ClickMultiplier).ToString("F1");
        BaseText.text = baseIncome.ToString();
    }

    public void ResetUpgrades()
    {
        foreach (StoreUpgrade upgrade in storeUpgrades)
        {
            upgrade.ResetUpgrade();
            UpdateUI();
        }
        foreach (ClickUpgrade upgrade in clickUpgrades)
        {
            upgrade.ResetUpgrade();
            UpdateUI();
        }
        foreach (BaseUpgrade upgrade in baseUpgrades)
        {
            upgrade.ResetUpgrade();
            baseIncome = 1; // reset ก่อน
            UpdateUI();
        }
    }

    public void SaveGame()
    {
        ClickerSaveData data = new ClickerSaveData();

        data.power = power;
        data.rebirths = rebirths;
        data.baseIncome = baseIncome;

        data.rebirthLevels = new int[1];
        data.rebirthLevels[0] = rebirthUpgrade.Level;

        data.storeLevels = new int[storeUpgrades.Length];
        for (int i = 0; i < storeUpgrades.Length; i++)
            data.storeLevels[i] = storeUpgrades[i].Level;

        data.clickLevels = new int[clickUpgrades.Length];
        for (int i = 0; i < clickUpgrades.Length; i++)
            data.clickLevels[i] = clickUpgrades[i].Level;

        data.baseLevels = new int[baseUpgrades.Length];
        for (int i = 0; i < baseUpgrades.Length; i++)
            data.baseLevels[i] = baseUpgrades[i].Level;

        data.lastExitTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        SaveSystem.Save(data);
    }

    public void LoadGame()
    {
        ClickerSaveData data = SaveSystem.Load();
        if (data == null) return;

        power = data.power;
        rebirths = data.rebirths;
        baseIncome = 1; // reset ก่อน
        rebirthUpgrade.SetLevel(data.rebirthLevels[0]);

        // Store
        for (int i = 0; i < storeUpgrades.Length && i < data.storeLevels.Length; i++)
            storeUpgrades[i].SetLevel(data.storeLevels[i]);

        // Click
        for (int i = 0; i < clickUpgrades.Length && i < data.clickLevels.Length; i++)
            clickUpgrades[i].SetLevel(data.clickLevels[i]);

        // Base
        for (int i = 0; i < baseUpgrades.Length && i < data.baseLevels.Length; i++)
        {
            baseUpgrades[i].SetLevel(data.baseLevels[i]);
            baseIncome += data.baseLevels[i]; // คำนวณใหม่
        }

        // Rebirth
        if (data.rebirthLevels != null && data.rebirthLevels.Length > 0)
        {
            rebirthUpgrade.SetLevel(data.rebirthLevels[0]);
        }
        ApplyOfflineIncome(data.lastExitTime);
        UpdateBackground();
        UpdateUI();
    }

    public void ExitGame()
    {
        SaveGame();

#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveGame();
    }

    double CalculateCurrentIncomePerSecond()
    {
        double sum = 0;

        foreach (StoreUpgrade upgrade in storeUpgrades)
            sum += upgrade.calculateIncomePerSecond();

        return sum;
    }

    void ApplyOfflineIncome(long lastExitTime)
    {
        long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long secondsAway = currentTime - lastExitTime;

        if (secondsAway <= 0)
            return;

        long maxSeconds = 28800; // 8 ชั่วโมง
        if (secondsAway > maxSeconds)
            secondsAway = maxSeconds;

        double incomePerSecond = CalculateCurrentIncomePerSecond();
        double offlineIncome = incomePerSecond * secondsAway;

        if (offlineIncome <= 0)
            return;

        power += (float)offlineIncome;

        offlinePopup.Show(offlineIncome); // 🔥 แสดง Popup
    }

    int GetTotalBaseLevel()
    {
        int total = 0;

        foreach (BaseUpgrade upgrade in baseUpgrades)
            total += upgrade.Level;

        return total;
    }

    public void UpdateBackground()
    {
        int totalLevel = GetTotalBaseLevel();
        int tierIndex = totalLevel / levelPerTier;

        if (tierIndex >= backgroundTiers.Length)
            tierIndex = backgroundTiers.Length - 1;

        if (backgroundImage.sprite != backgroundTiers[tierIndex] && !isFading)
        {
            StartCoroutine(FadeToNewBackground(backgroundTiers[tierIndex]));
        }
    }

    IEnumerator FadeToNewBackground(Sprite newSprite)
    {
        isFading = true;

        fadeImage.sprite = newSprite;
        Color color = fadeImage.color;
        color.a = 0;
        fadeImage.color = color;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = timer / fadeDuration;

            color.a = alpha;
            fadeImage.color = color;

            yield return null;
        }

        backgroundImage.sprite = newSprite;

        color.a = 0;
        fadeImage.color = color;

        isFading = false;
    }

    void ShowFloatingText(string message)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mainCanvas.transform as RectTransform,
            mousePos,
            mainCanvas.worldCamera,
            out Vector2 localPoint);

        // 🔥 สุ่มกระจายเล็กน้อย
        localPoint += new Vector2(
            UnityEngine.Random.Range(-30f, 30f),
            UnityEngine.Random.Range(-20f, 20f)
        );

        GameObject obj = Instantiate(floatingTextPrefab, mainCanvas.transform);
        obj.GetComponent<RectTransform>().localPosition = localPoint;

        obj.GetComponent<FloatingText>().Setup(message);
    }
}
