using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class TMPTextFxController : MonoBehaviour
{
    public enum FxType { 
        RB,     // 무지개.    강화류
        WB,     // 암흑 느낌. 소멸, 강화불가
        RY,     // 노랑 빨강. 중첩

        YB,     // 전기: 노랑/파랑
        BW,     // 물: 파랑/흰색
        R,      // 화염: 빨강/다크레드
        G,      // 독: 그린/다크그린
    }

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

    private readonly Color32 GREENBLUE = new Color32(0, 172, 151, 255);
    private readonly Color32 DARKBLUE = new Color32(65, 0, 172, 255);
    private readonly Color32 RED = new Color32(255, 60, 60, 255);
    private readonly Color32 YELLOW = new Color32(255, 230, 80, 255);


    // Electric (Yellow <-> DarkYellow)
    private readonly Color32 ELECTRIC_YELLOW = new Color32(255, 231, 77, 255);
    private readonly Color32 ELECTRIC_DARK_YELLOW = new Color32(150, 140, 25, 255);

    // Water (Blue <-> White)
    private readonly Color32 WATER_BLUE = new Color32(72, 170, 255, 255);
    private readonly Color32 WATER_WHITE = new Color32(245, 250, 255, 255);

    // Fire (Red <-> Dark Red)
    private readonly Color32 FIRE_RED = new Color32(255, 80, 65, 255);
    private readonly Color32 FIRE_DARK_RED = new Color32(150, 25, 25, 255);

    // Poison (Bright Green <-> Dark Green)
    private readonly Color32 POISON_GREEN = new Color32(130, 255, 90, 255);
    private readonly Color32 POISON_DARK_GREEN = new Color32(20, 120, 55, 255);


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

        // 폰트 크기 설정
        taggedText = ApplyLeadingFontSizeHeader(taggedText);

        // 커스텀 태그 파싱
        ParseTagsAndSetText(taggedText);
        tmp.ForceMeshUpdate();
    }

    public void ClearFx()
    {
        ranges.Clear();
    }

    // Font Size
    private string ApplyLeadingFontSizeHeader(string src)
    {
        if (string.IsNullOrEmpty(src))
            return src;

        if (src[0] != '<')
            return src;

        int close = src.IndexOf('>', 1);
        if (close < 0)
            return src;

        string numStr = src.Substring(1, close - 1);
        if (!int.TryParse(numStr, out int size))
            return src;

        tmp.fontSize = size;
        return src.Substring(close + 1);
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

            int yb = src.IndexOf("<yb>", i, System.StringComparison.Ordinal);
            int bw = src.IndexOf("<bw>", i, System.StringComparison.Ordinal);
            int r = src.IndexOf("<r>", i, System.StringComparison.Ordinal);
            int g = src.IndexOf("<g>", i, System.StringComparison.Ordinal);

            int open = MinPositive(rb, wb, ry, yb, bw, r, g); 
            
            
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
            else if (open == ry) { type = FxType.RY; openTag = "<ry>"; closeTag = "</ry>"; }
            else if (open == yb) { type = FxType.YB; openTag = "<yb>"; closeTag = "</yb>"; }
            else if (open == bw) { type = FxType.BW; openTag = "<bw>"; closeTag = "</bw>"; }
            else if (open == r) { type = FxType.R; openTag = "<r>"; closeTag = "</r>"; }
            else { type = FxType.G; openTag = "<g>"; closeTag = "</g>"; }

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

    private static int MinPositive(params int[] values)
    {
        int m = int.MaxValue;
        for (int i = 0; i < values.Length; i++)
        {
            int v = values[i];
            if (v >= 0 && v < m) m = v;
        }
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

        // 0~1 파형 (부드러운 왕복)
        float a = 0.5f + 0.5f * Mathf.Sin(phase * Mathf.PI * 2f);

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
                    return Lerp32(GREENBLUE, DARKBLUE, a);
                }
            case FxType.RY:
                {
                    return Lerp32(RED, YELLOW, a);
                }

            case FxType.YB: // 전기
                {
                    return Lerp32(ELECTRIC_YELLOW, ELECTRIC_DARK_YELLOW, a);
                }
            case FxType.BW: // 물
                {
                    return Lerp32(WATER_BLUE, WATER_WHITE, a);
                }
            case FxType.R: // 화염
                {
                    return Lerp32(FIRE_RED, FIRE_DARK_RED, a);
                }
            case FxType.G: // 독
            default:
                {
                    return Lerp32(POISON_DARK_GREEN, POISON_GREEN, a);
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