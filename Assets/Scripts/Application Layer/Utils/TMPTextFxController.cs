using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class TMPTextFxController : MonoBehaviour
{
    public enum FxType { RB, WB, RY }

    [SerializeField] private TMP_Text tmp;

    [Header("Anim")]
    [SerializeField] private bool useUnscaledTime = true;
    [SerializeField] private float speed = 1.2f;          // 전체 속도
    [SerializeField] private float charPhase = 0.15f;     // 그라데이션 폭
    [SerializeField] private float updateInterval = 0f;   // 0이면 매프레임, 0.03이면 30fps

    private struct RangeFx
    {
        public FxType type;
        public int start;
        public int length;
    }

    private readonly List<RangeFx> ranges = new();
    private Coroutine routine;

    private static readonly Color32 BLACK = new Color32(0, 0, 0, 255);
    private static readonly Color32 WHITE = new Color32(255, 255, 255, 255);
    private static readonly Color32 RED = new Color32(255, 60, 60, 255);
    private static readonly Color32 YELLOW = new Color32(255, 230, 80, 255);

    private void Awake()
    {
        if (!tmp) tmp = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        StartFxLoop();
    }

    private void OnDisable()
    {
        StopFxLoop();
    }

    private void StartFxLoop()
    {
        StopFxLoop();
        routine = StartCoroutine(Co_FxLoop());
    }

    private void StopFxLoop()
    {
        if (routine != null) StopCoroutine(routine);
        routine = null;
    }

    public void SetTaggedText(string taggedText)
    {
        if (!tmp) return;

        ParseTagsAndSetText(taggedText);
        tmp.ForceMeshUpdate();
    }

    public void ClearFx()
    {
        ranges.Clear();
    }


    // Parsing
    private void ParseTagsAndSetText(string src)
    {
        ranges.Clear();
        if (string.IsNullOrEmpty(src))
        {
            tmp.text = "";
            return;
        }

        // 지원 태그 <rb> </rb>, <wb> </wb>, <ry> </ry>
        var sb = new StringBuilder(src.Length);

        int i = 0;
        while (i < src.Length)
        {
            int rb = src.IndexOf("<rb>", i, System.StringComparison.Ordinal);
            int wb = src.IndexOf("<wb>", i, System.StringComparison.Ordinal);
            int ry = src.IndexOf("<ry>", i, System.StringComparison.Ordinal);

            int open = MinPositive(rb, wb, ry);
            if (open < 0)
            {
                sb.Append(src, i, src.Length - i);
                break;
            }

            sb.Append(src, i, open - i);

            FxType type;
            string openTag, closeTag;

            if (open == rb) { type = FxType.RB; openTag = "<rb>"; closeTag = "</rb>"; }
            else if (open == wb) { type = FxType.WB; openTag = "<wb>"; closeTag = "</wb>"; }
            else { type = FxType.RY; openTag = "<ry>"; closeTag = "</ry>"; }

            int contentStart = open + openTag.Length;
            int close = src.IndexOf(closeTag, contentStart, System.StringComparison.Ordinal);
            if (close < 0)
            {
                sb.Append(src, open, src.Length - open);
                break;
            }

            int len = close - contentStart;
            int startInOutput = sb.Length;

            sb.Append(src, contentStart, len);

            ranges.Add(new RangeFx { type = type, start = startInOutput, length = len });

            i = close + closeTag.Length;
        }

        tmp.text = sb.ToString();
    }

    private static int MinPositive(int a, int b, int c)
    {
        int m = int.MaxValue;
        if (a >= 0 && a < m) m = a;
        if (b >= 0 && b < m) m = b;
        if (c >= 0 && c < m) m = c;
        return (m == int.MaxValue) ? -1 : m;
    }


    private IEnumerator Co_FxLoop()
    {
        while (true)
        {
            if (tmp != null && ranges.Count > 0)
            {
                ApplyFxToRanges();
            }

            if (updateInterval <= 0f) yield return null;
            else yield return new WaitForSecondsRealtime(updateInterval);
        }
    }

    private void ApplyFxToRanges()
    {
        tmp.ForceMeshUpdate();
        var textInfo = tmp.textInfo;

        float t = useUnscaledTime ? Time.unscaledTime : Time.time;

        foreach (var r in ranges)
        {
            for (int k = 0; k < r.length; k++)
            {
                int charIndex = r.start + k;
                if (charIndex < 0 || charIndex >= textInfo.characterCount) continue;

                var ci = textInfo.characterInfo[charIndex];
                if (!ci.isVisible) continue;

                int meshIndex = ci.materialReferenceIndex;
                int v = ci.vertexIndex;

                Color32 col = EvaluateColor(r.type, t, k);

                var colors = textInfo.meshInfo[meshIndex].colors32;
                colors[v + 0] = col;
                colors[v + 1] = col;
                colors[v + 2] = col;
                colors[v + 3] = col;
            }
        }

        // 반영
        for (int m = 0; m < textInfo.meshInfo.Length; m++)
        {
            textInfo.meshInfo[m].mesh.colors32 = textInfo.meshInfo[m].colors32;
            tmp.UpdateGeometry(textInfo.meshInfo[m].mesh, m);
        }
    }

    private Color32 EvaluateColor(FxType type, float time, int charIndexInRange)
    {
        float phase = (time * speed) + (charIndexInRange * charPhase);

        switch (type)
        {
            case FxType.RB:
                {
                    float hue = Mathf.Repeat(phase, 1f);
                    Color c = Color.HSVToRGB(hue, 1f, 1f);
                    return (Color32)c;
                }
            case FxType.WB:
                {
                    float a = 0.5f + 0.5f * Mathf.Sin(phase * Mathf.PI * 2f);
                    return Lerp32(BLACK, WHITE, a);
                }
            case FxType.RY:
            default:
                {
                    float a = 0.5f + 0.5f * Mathf.Sin(phase * Mathf.PI * 2f);
                    return Lerp32(RED, YELLOW, a);
                }
        }
    }

    private static Color32 Lerp32(Color32 a, Color32 b, float t)
    {
        t = Mathf.Clamp01(t);
        return new Color32(
            (byte)Mathf.RoundToInt(Mathf.Lerp(a.r, b.r, t)),
            (byte)Mathf.RoundToInt(Mathf.Lerp(a.g, b.g, t)),
            (byte)Mathf.RoundToInt(Mathf.Lerp(a.b, b.b, t)),
            (byte)Mathf.RoundToInt(Mathf.Lerp(a.a, b.a, t))
        );
    }
}