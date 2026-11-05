using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameHud : MonoBehaviour
{
    public Sprite coinSprite;

    Text scoreValue;
    Text coinValue;
    Text livesValue;
    Text worldValue;
    Text timeValue;
    LevelTimer timer;
    int shownScore = -1;
    float pulse;

    void Awake()
    {
        Build();
    }

    void Update()
    {
        Refresh();

        if (pulse > 0f && scoreValue != null) {
            pulse -= Time.deltaTime * 3f;
            float scale = 1f + Mathf.Max(pulse, 0f) * 0.18f;
            scoreValue.rectTransform.localScale = new Vector3(scale, scale, 1f);
        }
    }

    void Refresh()
    {
        GameManager game = GameManager.Instance;
        if (!timer) timer = FindFirstObjectByType<LevelTimer>();
        if (timeValue) timeValue.text = timer ? Mathf.CeilToInt(timer.TiempoRestante).ToString("000") : "---";
        int currentScore = ScoreManager.Instance != null ? ScoreManager.Instance.score : 0;

        if (scoreValue != null) {
            scoreValue.text = currentScore.ToString("D6");
            if (currentScore != shownScore) {
                shownScore = currentScore;
                pulse = 1f;
            }
        }

        if (coinValue != null) {
            coinValue.text = "x" + (game != null ? game.coins.ToString("00") : "00");
        }

        if (livesValue != null) {
            livesValue.text = "x" + (game != null ? game.lives.ToString() : "0");
        }

        if (worldValue != null) {
            worldValue.text = game != null ? game.world + "-" + game.stage : "1-1";
        }
    }

    void Build()
    {
        if (FindAnyObjectByType<EventSystem>() == null) {
            GameObject events = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            events.transform.SetParent(transform, false);
        }

        GameObject canvasObject = new GameObject("HudCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(960f, 540f);
        scaler.matchWidthOrHeight = 0.5f;

        RectTransform bar = CreatePanel(canvasObject.transform, "Bar", new Color(0.04f, 0.06f, 0.14f, 0.88f));
        bar.anchorMin = new Vector2(0f, 1f);
        bar.anchorMax = new Vector2(1f, 1f);
        bar.pivot = new Vector2(0.5f, 1f);
        bar.sizeDelta = new Vector2(0f, 64f);
        bar.anchoredPosition = Vector2.zero;

        RectTransform line = CreatePanel(bar, "GoldLine", new Color(0.96f, 0.78f, 0.18f, 1f));
        line.anchorMin = new Vector2(0f, 0f);
        line.anchorMax = new Vector2(1f, 0f);
        line.pivot = new Vector2(0.5f, 0f);
        line.sizeDelta = new Vector2(0f, 3f);
        line.anchoredPosition = Vector2.zero;

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        Color gold = new Color(1f, 0.86f, 0.28f);

        CreateLabel(bar, "MARIO", font, 16, gold, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(22f, -6f), new Vector2(160f, 18f));
        scoreValue = CreateLabel(bar, "000000", font, 24, Color.white, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(22f, -30f), new Vector2(180f, 28f));

        RectTransform coinIcon = CreatePanel(bar, "CoinIcon", Color.white);
        coinIcon.anchorMin = new Vector2(0.34f, 0.5f);
        coinIcon.anchorMax = new Vector2(0.34f, 0.5f);
        coinIcon.sizeDelta = new Vector2(22f, 22f);
        coinIcon.anchoredPosition = new Vector2(0f, -4f);
        Image coinImage = coinIcon.GetComponent<Image>();
        coinImage.sprite = coinSprite;
        coinImage.preserveAspect = true;
        if (coinSprite == null) {
            coinImage.color = gold;
        }

        CreateLabel(bar, "MONEDAS", font, 14, gold, new Vector2(0.34f, 1f), new Vector2(0.34f, 1f), new Vector2(28f, -6f), new Vector2(140f, 16f));
        coinValue = CreateLabel(bar, "x00", font, 24, Color.white, new Vector2(0.34f, 1f), new Vector2(0.34f, 1f), new Vector2(28f, -30f), new Vector2(120f, 28f));

        CreateLabel(bar, "VIDAS", font, 14, gold, new Vector2(0.58f, 1f), new Vector2(0.58f, 1f), new Vector2(0f, -6f), new Vector2(120f, 16f));
        livesValue = CreateLabel(bar, "x3", font, 24, Color.white, new Vector2(0.58f, 1f), new Vector2(0.58f, 1f), new Vector2(0f, -30f), new Vector2(120f, 28f));

        CreateLabel(bar, "TIEMPO", font, 14, gold, new Vector2(0.73f, 1f), new Vector2(0.73f, 1f), new Vector2(0f, -6f), new Vector2(95f, 16f));
        timeValue = CreateLabel(bar, "400", font, 24, Color.white, new Vector2(0.73f, 1f), new Vector2(0.73f, 1f), new Vector2(0f, -30f), new Vector2(95f, 28f));

        CreateLabel(bar, "MUNDO", font, 14, gold, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-150f, -6f), new Vector2(120f, 16f));
        worldValue = CreateLabel(bar, "1-1", font, 24, Color.white, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-150f, -30f), new Vector2(120f, 28f));
    }

    static RectTransform CreatePanel(Transform parent, string name, Color color)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);
        panel.GetComponent<Image>().color = color;
        return panel.GetComponent<RectTransform>();
    }

    static Text CreateLabel(Transform parent, string content, Font font, int size, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 sizeDelta)
    {
        GameObject label = new GameObject(content, typeof(RectTransform), typeof(Text));
        label.transform.SetParent(parent, false);
        RectTransform rect = label.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = sizeDelta;

        Text text = label.GetComponent<Text>();
        text.font = font;
        text.text = content;
        text.fontSize = size;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleLeft;
        text.color = color;
        text.raycastTarget = false;
        return text;
    }
}
