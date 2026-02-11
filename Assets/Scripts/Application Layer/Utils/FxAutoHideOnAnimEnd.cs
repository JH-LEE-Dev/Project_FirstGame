using UnityEngine;

public class FxAutoHideOnAnimEnd : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;

    [Header("Options")]
    [SerializeField] private int layerIndex = 0;

    [SerializeField] private float failSafeSeconds = 3f;

    private float alive;
    private bool playing;

    public void PlayAt(Vector2 worldPos)
    {
        transform.position = worldPos;

        alive = 0f;
        playing = true;

        gameObject.SetActive(true);

        if (!animator)
        {
            Debug.LogError($"{name}: Animator not assigned.");
            return;
        }

        if (!animator.gameObject.activeInHierarchy)
            animator.gameObject.SetActive(true);

        animator.enabled = true;
        animator.speed = 1f;
        animator.Play(0, layerIndex, 0f);
        animator.Update(0f);
    }

    private void Update()
    {
        if (!playing) return;

        alive += Time.deltaTime;
        if (failSafeSeconds > 0f && alive >= failSafeSeconds)
        {
            Hide();
            return;
        }

        if (!animator) return;

        var st = animator.GetCurrentAnimatorStateInfo(layerIndex);

        if (st.normalizedTime >= 1f)
            Hide();
    }

    private void Hide()
    {
        playing = false;

        if (animator)
            animator.enabled = false;

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        playing = false;
        alive = 0f;
    }
}