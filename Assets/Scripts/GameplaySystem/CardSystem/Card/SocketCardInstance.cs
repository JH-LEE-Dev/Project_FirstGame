using UnityEngine.EventSystems;
using UnityEngine;

public class SocketCardInstance : CardInstance
{
    public int socketIndex { get; private set; }
    private BulletSocketSystem bulletSocketSystem;

    public void Bind(int _socketIndex, BulletSocketSystem _bulletSocketSystem)
    {
        socketIndex = _socketIndex;
        bulletSocketSystem = _bulletSocketSystem;
    }

    // For Input
    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bulletSocketSystem?.UnEquipBulletCard(socketIndex);
    }
}
