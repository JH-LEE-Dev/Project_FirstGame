
using System.Collections.Generic;
using UnityEngine;

public class BulletSocketSystem : MonoBehaviour
{
    [Header("Refs")]
    private Transform characterTransform;
    // 소켓 메인 피봇
    [SerializeField] private Transform pivot;
    // 소켓 프리팹
    [SerializeField] private GameObject slotPrefab;        

    [Header("Capacity")]
    // 최대 5개 까지
    [SerializeField, Range(1, 5)] private int maxSlots = 5;


    [Header("Line Layout")]
    [SerializeField] private float spacing = 0.1f;
    [SerializeField] private float yOffset = 0.0f;

    public int Count { get; private set; } = 0;

    private readonly List<SocketVisual> slots = new();

    public void Init(Transform _characterTransform, int _slotCount)
    {
        if (pivot == null)
        {
            enabled = false;
            return;
        }

        BuildSlotsIfNeeded();
        characterTransform = _characterTransform;
        SetCount(_slotCount);
    }

    private void BuildSlotsIfNeeded()
    {
        slots.Clear();
        if (slotPrefab == null) return;

        for (int i = 0; i < maxSlots; i++)
        {
            GameObject go = Instantiate(slotPrefab, pivot);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.SetActive(false);

            var visual = go.GetComponent<SocketVisual>();
            if (visual == null) continue;
            slots.Add(visual);
        }

        Debug.Log(slots.Count);
    }

    public void SetCount(int count)
    {
        int prevCount = Count;
        Count = Mathf.Clamp(count, 0, maxSlots);

        for (int i = 0; i < slots.Count; i++)
        {
            bool shouldBeActive = i < Count;
            bool wasActive = slots[i].gameObject.activeSelf;

            slots[i].gameObject.SetActive(shouldBeActive);

            if (shouldBeActive && !wasActive)
            {
                Vector3 pos = GetLocalSlotPosition(i);
                slots[i].SetTargetLocalPosition(pos, snap: true);
            }
        }

        RelayoutSlots();
    }

    // 모든 피봇의 위치를 잡아주기
    private void RelayoutSlots()
    {
        for (int i = 0; i < Count; i++)
        {
            Vector3 target = GetLocalSlotPosition(i);
            slots[i].SetTargetLocalPosition(target);
        }
    }

    // 단일 위치 잡아주기
    public Vector3 GetLocalSlotPosition(int index)
    {
        if (Count <= 0)
            return new Vector3(0f, yOffset, 0f);

        index = Mathf.Clamp(index, 0, Count - 1);

        float center = (Count - 1) * 0.5f;
        float x = (index - center) * spacing;

        return new Vector3(x, yOffset, 0f);
    }

    // 끼워넣기
    public void Attach(CardInstance card, int index)
    {
        if (card == null) return;
        if (index < 0 || index >= Count) return;

        Transform slotTr = slots[index].transform;
        Transform cardTr = card.transform;

        cardTr.SetParent(slotTr, false);
        cardTr.localPosition = Vector3.zero;
        cardTr.localRotation = Quaternion.identity;
        cardTr.localScale = Vector3.one;
    }
}
