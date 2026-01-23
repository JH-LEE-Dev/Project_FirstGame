using UnityEngine;
using TMPro;


[RequireComponent(typeof(TextMeshProUGUI))]
public class UIText_SelectTextWave : MonoBehaviour
{
    [SerializeField] private float amplitude = 6f;   // 픽셀
    [SerializeField] private float frequency = 2f;   // 속도
    [SerializeField] private float charOffset = 0.2f;

    private TextMeshProUGUI tmp;
    private Vector3[][] originalVertices;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        tmp.ForceMeshUpdate();
        CacheOriginal();
    }

    private void CacheOriginal()
    {
        var textInfo = tmp.textInfo;
        originalVertices = new Vector3[textInfo.meshInfo.Length][];
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            originalVertices[i] = textInfo.meshInfo[i].vertices.Clone() as Vector3[];
        }
    }

    private void LateUpdate()
    {
        if (tmp == null) return;

        tmp.ForceMeshUpdate();
        var textInfo = tmp.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] verts = textInfo.meshInfo[materialIndex].vertices;

            float t = Time.unscaledTime * frequency + i * charOffset;
            float y = Mathf.Sin(t) * amplitude;

            // 4개의 버텍스를 같이 이동 (한 글자)
            verts[vertexIndex + 0] = originalVertices[materialIndex][vertexIndex + 0] + new Vector3(0, y, 0);
            verts[vertexIndex + 1] = originalVertices[materialIndex][vertexIndex + 1] + new Vector3(0, y, 0);
            verts[vertexIndex + 2] = originalVertices[materialIndex][vertexIndex + 2] + new Vector3(0, y, 0);
            verts[vertexIndex + 3] = originalVertices[materialIndex][vertexIndex + 3] + new Vector3(0, y, 0);
        }

        // 메시 갱신
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            tmp.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    public void Rebuild()
    {
        tmp.ForceMeshUpdate();
        CacheOriginal();
    }
}
