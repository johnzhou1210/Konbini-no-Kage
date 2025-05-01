using TMPro;
using UnityEngine;

public class TextJitter : MonoBehaviour
{
    private TMP_Text text;
    private TMP_TextInfo textInfo;

    private Vector2[] jitterOffsets;
    private float timer;
    public float updateInterval = 0.1f;
    public float jitterAmount = 16f;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void LateUpdate()
    {
        text.ForceMeshUpdate();
        textInfo = text.textInfo;

        if (jitterOffsets == null || jitterOffsets.Length != textInfo.characterCount)
            jitterOffsets = new Vector2[textInfo.characterCount];

        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            for (int i = 0; i < jitterOffsets.Length; i++)
            {
                jitterOffsets[i] = Random.insideUnitCircle * jitterAmount;
            }
        }

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int index = textInfo.characterInfo[i].vertexIndex;
            Vector3[] verts = textInfo.meshInfo[textInfo.characterInfo[i].materialReferenceIndex].vertices;

            Vector2 offset = jitterOffsets[i];
            for (int j = 0; j < 4; j++)
            {
                verts[index + j] += new Vector3(offset.x, offset.y, 0f);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
            text.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }

}