using UnityEngine;

public class QuitPopupUI : MonoBehaviour
{
    [SerializeField] GameObject root;
    [SerializeField] GameManager gameManager;

    public bool IsOpen => root.activeSelf;

    public void Show()
    {
        root.SetActive(true);
        Time.timeScale = 0f; // ËÂØ´à¡Á
    }

    public void Hide()
    {
        root.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ConfirmQuit()
    {
        Time.timeScale = 1f;
        gameManager.ExitGame();
    }
}