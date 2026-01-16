using System.Collections.Generic;
using UnityEngine;


public class BulletSocketSystem : MonoBehaviour
{
    [Header("Refs")]
    // 소켓 메인 피봇
    [SerializeField] private Transform pivot;
    // 소켓 프리팹
    [SerializeField] private GameObject slotPrefab;
    // 카드 프리팹
    [SerializeField] private GameObject CardPrefab;

    [Header("Capacity")]
    // 최대 5개 까지
    [SerializeField, Range(1, 5)] private int maxSockets = 5;

    [Header("Line Layout")]
    [SerializeField] private float spacing = 0.1f;

    public int Count { get; private set; } = 0;

    private readonly List<SocketVisual> sockets = new();
    private readonly List<SocketCardInstance> cards = new();


    public void Init(int _slotCount)
    {
        if (pivot == null) { enabled = false; return; }

        BuildSlotsIfNeeded();
        BuildCardIntanceIfneeded();

        SetCount(_slotCount);
    }

    // 풀링 함수
    private void BuildSlotsIfNeeded()
    {
        sockets.Clear();
        if (slotPrefab == null) return;

        for (int i = 0; i < maxSockets; i++)
        {
            GameObject go = Instantiate(slotPrefab, pivot);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.SetActive(false);

            var visual = go.GetComponent<SocketVisual>();
            if (visual == null) continue;
            visual.SetOverlapCount(0);
            sockets.Add(visual);
        }

        for (int i = 0; i < maxSockets; i++)
        {
            GameObject go = Instantiate(CardPrefab, sockets[i].GetSocketVisualTransform());
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.SetActive(false);

            var card = go.GetComponent<SocketCardInstance>();
            if (card == null) continue;
            cards.Add(card);

        }
    }

    private void BuildCardIntanceIfneeded()
    {

    }

    //////////////////////////////////////////////// 위치 잡기

    // pivot 안에서 로컬 위치 시키기
    public void SetCount(int count)
    {
        int prevCount = Count;
        Count = Mathf.Clamp(count, 0, maxSockets);

        for (int i = 0; i < sockets.Count; i++)
        {
            bool shouldBeActive = i < Count;
            bool wasActive = sockets[i].gameObject.activeSelf;

            sockets[i].gameObject.SetActive(shouldBeActive);

            if (shouldBeActive && !wasActive)
            {
                Vector3 pos = GetLocalSlotPosition(i);
                sockets[i].SetTargetLocalPosition(pos, snap: true);
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
            sockets[i].SetTargetLocalPosition(target);
        }
    }

    // 단일 위치 잡아주기
    public Vector3 GetLocalSlotPosition(int index)
    {
        if (Count <= 0)
            return new Vector3(0f, 0f, 0f);

        index = Mathf.Clamp(index, 0, Count - 1);

        float center = (Count - 1) * 0.5f;
        float x = (index - center) * spacing;

        return new Vector3(x, 0f, 0f);
    }


    //////////////////////////////////////////////// 소켓 위치 뱉기

    public Transform GetSocketTransform(int index)
    {
        if (Count <= 0)
            return null;

        index = Mathf.Clamp(index, 0, Count - 1);

        return sockets[index].GetSocketVisualTransform();
    }


    //////////////////////////////////////////////// 카드 장착 및 해제

    public void EquipBulletCard(int _index, CardDataInstance _data = null)
    {
        CardData data = _data?.GetCardData();

        SocketVisual socketVisual = sockets[_index];
        int count = socketVisual.GetOverlapCount();
        count++;
        socketVisual.SetOverlapCount(count);

        SocketCardInstance targetCardInstance = cards[_index];
        targetCardInstance.gameObject.SetActive(true);
        targetCardInstance.ApplyData(_data);

        // socketVisual -> 연출. 
    }

    public void UnEquipBulletCard(int _index)
    {
        SocketVisual socketVisual = sockets[_index];

        int count = socketVisual.GetOverlapCount();
        count--;
        socketVisual.SetOverlapCount(count);

        if (count <= 0)
        {
            SocketCardInstance targetCardInstance = cards[_index];
            targetCardInstance.gameObject.SetActive(false);
            targetCardInstance.Clear();
        }

        // socketVisual -> 연출. 
    }
}
