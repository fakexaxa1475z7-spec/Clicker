using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 80f;
    public float lifetime = 0.8f;

    TMP_Text text;
    CanvasGroup canvasGroup;
    float timer;

    void Awake()
    {
        text = GetComponent<TMP_Text>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Setup(string message)
    {
        text.text = message;

        // 🔥 scale pop effect
        transform.localScale = Vector3.one * 0.8f;
    }

    void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        timer += Time.deltaTime;

        // scale เด้งก่อนหาย
        float scale = 1f + (timer * 0.5f);
        transform.localScale = Vector3.one * scale;

        canvasGroup.alpha = 1 - (timer / lifetime);

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}