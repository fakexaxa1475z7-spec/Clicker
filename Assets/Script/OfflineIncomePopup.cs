using UnityEngine;
using TMPro;

public class OfflineIncomePopup : MonoBehaviour
{
    public TMP_Text incomeText;
    public GameObject root;

    public void Show(double amount)
    {
        root.SetActive(true);
        incomeText.text =
            Mathf.RoundToInt((float)amount).ToString("N0");
    }

    public void Close()
    {
        root.SetActive(false);
    }
}