using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BulletLineSystem : MonoBehaviour
{
    [Header("Refs")]
    private UIView_Unit_World uIView_Unit_World;
    private Transform characterTransform;
    private InputManager inputManager;

    [Header("Dot Setup")]
    [SerializeField] private BulletLineDot dotPrefab;
    [SerializeField] private Sprite dotSprite;
    [SerializeField, Range(8, 200)] private int dotCount = 50;

    [Header("Line Params")]
    private float offset = 0.6f;    // 캐릭터와 떨어뜨리는 거리
    private float length = 17f;     // 점선 총 길이
    private float flowSpeed = 4f; // 흐르는 속도

    [Header("Optional")]
    [SerializeField] private bool isAiming;

    [Header("Fade")]
    private float aimFadeDuration = 0.25f;
    private float aimAlpha01 = 0f;
    private Tween aimFadeTween;

    [Header("Distance Alpha")]
    [SerializeField, Range(0.1f, 6f)]
    private float tailFalloffPow = 2.8f;

    [Header("Visibility Gate")]
    private float gateFadeDuration = 0.35f;
    private float hideBelowYOffset = -2.2f; // 캐릭터보다 이만큼 아래면 숨김
    private float gateAlpha01 = 1f;
    private float gateAlphaTarget = 1f;
    private Tween gateTween;

    private bool gateHidden;


    private readonly List<BulletLineDot> dots = new();
    private Camera mainCam;

    private Vector2 mousePos;

    private float finalmulalpha = 0.6f;

    private void Awake()
    {
    }

    private void OnDestroy()
    {
        ReleaseEvent();
    }

    private float GetAimAlpha()
    {
        return aimAlpha01;
    }

    private void SetAimAlpha(float v)
    {
        aimAlpha01 = v;
    }

    private float GetGateAlpha()
    {
        return gateAlpha01;
    }
    private void SetGateAlpha(float v)
    {
        gateAlpha01 = v;
    }

    public void Init(UIView_Unit_World owner, Transform ct, InputManager im)
    {
        if (owner) uIView_Unit_World = owner;
        if (ct) characterTransform = ct;
        if (im) inputManager = im;
        if (!mainCam) mainCam = Camera.main;

        EnsurePool();
        SetVisible(false);
        BindEvent();
    }

    private void BindEvent()
    {
        inputManager.inputReader.PointerPositionEvent -= SetMousePos;
        inputManager.inputReader.PointerPositionEvent += SetMousePos;

    }

    private void ReleaseEvent()
    {
        inputManager.inputReader.PointerPositionEvent -= SetMousePos;
    }

    private void EnsurePool()
    {
        if (!dotPrefab) return;

        while (dots.Count < dotCount)
        {
            var d = Instantiate(dotPrefab, transform);
            d.Init(dotSprite);
            d.SetVisible(false);
            dots.Add(d);
        }
    }

    public void SetAiming(bool aiming)
    {
        isAiming = aiming;

        aimFadeTween?.Kill();

        float target = aiming ? 1f : 0f;

        aimFadeTween = DOTween.To(
            GetAimAlpha,
            SetAimAlpha,
            target,
            aimFadeDuration
        )
        .SetEase(Ease.OutCubic)
        .SetUpdate(true)
        .SetLink(gameObject, LinkBehaviour.KillOnDisable);

        if (aiming)
            SetVisible(true);
    }

    private void SetVisible(bool v)
    {
        for (int i = 0; i < dots.Count; i++)
            dots[i].SetVisible(v);
    }

    public void SetMousePos(Vector2 move)
    {
        mousePos = move;
    }


    private void Update()
    {
        if (!characterTransform) return;
        if (!mainCam) return;

        if (!isAiming && aimAlpha01 <= 0.001f)
        {
            SetVisible(false);
            return;
        }

        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(
            new Vector3(mousePos.x, mousePos.y, 0f)
        );
        mouseWorld.z = characterTransform.position.z;

        Vector3 origin = characterTransform.position;
        Vector3 dir = mouseWorld - origin;
        dir.z = 0f;



        UpdateGate(mouseWorld, origin);


        float sqr = dir.sqrMagnitude;
        if (sqr < 0.0001f) return;

        dir /= Mathf.Sqrt(sqr);

        if (dots.Count < dotCount) EnsurePool();

        float spacing = length / dotCount;
        float phase = Mathf.Repeat(Time.time * flowSpeed, 1f);
        Vector3 start = origin + dir * offset;

        for (int i = 0; i < dots.Count; i++)
        {
            if (i >= dotCount)
            {
                dots[i].SetVisible(false);
                continue;
            }

            float dist = (i + phase) * spacing;
            dots[i].transform.position = start + dir * dist;

            float t = (float)i / (dotCount - 1);

            float tailAlpha = Mathf.Pow(1f - t, tailFalloffPow);

            float finalAlpha = aimAlpha01 * gateAlpha01 * tailAlpha * finalmulalpha;

            dots[i].SetVisible(true);
            dots[i].SetAlpha(finalAlpha);
        }
    }

    private void UpdateGate(Vector3 mouseWorld, Vector3 origin)
    {
        if (!gateHidden)
        {
            if (mouseWorld.y < origin.y - hideBelowYOffset)
                SetGateTarget(0f);
        }
        else
        {
            if (mouseWorld.y > origin.y - hideBelowYOffset)
                SetGateTarget(1f);
        }
    }
    private void SetGateTarget(float newTarget)
    {
        if (Mathf.Approximately(gateAlphaTarget, newTarget)) return;

        gateAlphaTarget = newTarget;
        gateHidden = (newTarget < 0.5f);

        gateTween?.Kill();
        Ease ease = (gateAlphaTarget > gateAlpha01) ? Ease.InOutCubic : Ease.OutCubic;

        gateTween = DOTween.To(GetGateAlpha, SetGateAlpha, gateAlphaTarget, gateFadeDuration)
            .SetEase(ease)
            .SetUpdate(true)
            .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }
}
