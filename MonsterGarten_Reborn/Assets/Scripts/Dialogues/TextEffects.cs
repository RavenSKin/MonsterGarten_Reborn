using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class TextEffects : MonoBehaviour
{
    public bool CanShake;
    public float magnitude = 0.1f;
    public TextMeshProUGUI _text;
    Mesh _textMesh;
    Vector3[] _vertices;
    public Vector3[] _startVertices;
    public Vector3 StartPosition;
    public int firstVertice;
    public int lastVertice;
    private void Update()
    {
        if (CanShake)
        {
            MoveLetters();
        }
        else
        {
            if (_startVertices != null)
            {

                //_textMesh.vertices = _startVertices;
                //_text.canvasRenderer.SetMesh(_textMesh);
            }

        }

    }
    public void MoveLetters()
    {
        Debug.Log("LES LETTRES BOUGENT");
        float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
        float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;
        _text.ForceMeshUpdate();
        _textMesh = _text.mesh;
        _vertices = _textMesh.vertices;
        for (int i = firstVertice; i <= lastVertice; i++)// le mot
        {       // j = vertice , i = compte unitaire , 

            for (int j = firstVertice; j < lastVertice; j++)
            {

                _vertices[j] = new Vector3(_vertices[j].x + x, _vertices[j].y + y);

            }
        }

        _textMesh.vertices = _vertices;
        _text.canvasRenderer.SetMesh(_textMesh);

    }


}
