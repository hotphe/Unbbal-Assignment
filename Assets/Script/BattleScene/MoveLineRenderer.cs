using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLineRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer.positionCount = 2;
    }


    public void SetPosition(Vector3 startPos, Vector3 endPos)
    {
        _lineRenderer.SetPosition(0, startPos);
        _lineRenderer.SetPosition(1, endPos);
    }

}
