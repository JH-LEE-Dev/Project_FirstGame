using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
public class VFX_CardUseEffect : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private VisualEffect vfx;

    [Header("Timing")]
    [SerializeField] private float totalDuration = 0.6f;

    private Coroutine routine;
    private Action<VFX_CardUseEffect> releaseToPool;
    private Action onComplete;

    [Header("Default VFX Params")]
    private float widthX = 0.96f;
    private float widthY = 1.44f;
    private float spreadPower = 3.5f;
    private float spreadDur = 0.2f;
    private float collectDur = 0.3f;
    private float glowIntensity = 4.5f;
    private Color glowColor = new Color(1f, 1f, 0f, 1f); // 255,255,0

    private const string P_WidthX = "WidthX";
    private const string P_WidthY = "WidthY";
    private const string P_Center = "Center";
    private const string P_SpreadPower = "SpreadPower";
    private const string P_SpreadDur = "SpreadDur";
    private const string P_CollectDur = "CollectDur";
    private const string P_GlowIntensity = "GlowIntensity";
    private const string P_GlowColor = "GlowColor";

    private void Awake()
    {
        if (!vfx) vfx = GetComponentInChildren<VisualEffect>(true);
    }


    // 풀 시스템이 SetReleaseHandler로 주입해줌.
    public void SetReleaseHandler(Action<VFX_CardUseEffect> releaseHandler)
    {
        releaseToPool = releaseHandler;
    }

    // 이펙트 재생. 끝나면 onComplete 호출 후 풀로 반환.
    public void Play(Vector3 worldPos, float initialLocalScale, Action onCompleteCallback = null)
    {
        onComplete = onCompleteCallback;

        transform.position = worldPos;
        transform.localScale *= initialLocalScale;

        gameObject.SetActive(true);

        // 재생 중인 코루틴/이펙트 정리
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }

        if (vfx)
        {
            // 같은 인스턴스를 재사용할 때 잔상/상태 초기화
            vfx.Stop();

            ApplyDefaultParams(worldPos);


            vfx.Reinit();
            vfx.Play();
        }

        routine = StartCoroutine(Co_PlayAndRelease());
    }

    private void ApplyDefaultParams(Vector3 centerWorld)
    {
        var center = transform.InverseTransformPoint(centerWorld);
        //var center = centerWorld;

        vfx.SetFloat(P_WidthX, widthX);
        vfx.SetFloat(P_WidthY, widthY);
        vfx.SetVector3(P_Center, center);

        vfx.SetFloat(P_SpreadPower, spreadPower);
        vfx.SetFloat(P_SpreadDur, spreadDur);
        vfx.SetFloat(P_CollectDur, collectDur);

        vfx.SetFloat(P_GlowIntensity, glowIntensity);
        vfx.SetVector4(P_GlowColor, (Vector4)glowColor); // VisualEffect는 Vector4로도 잘 받음
    }


    public void ForceStopAndRelease()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }

        if (vfx)
        {
            vfx.Stop();
            vfx.Reinit();
        }

        Release();
    }

    private IEnumerator Co_PlayAndRelease()
    {
        yield return new WaitForSeconds(totalDuration);
        Release();
    }

    private void Release()
    {
        routine = null;

        var cb = onComplete;
        onComplete = null;
        cb?.Invoke();

        releaseToPool?.Invoke(this);
    }
}
