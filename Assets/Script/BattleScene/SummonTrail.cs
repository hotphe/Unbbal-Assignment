using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

public class SummonTrail : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _self;
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private float _speed;
    [SerializeField] private float _height;
    private Vector3 _startPos;
    private Vector3 _targetPos;
    private float _delta = 0f;
    public void Init(EGrade grade,Vector3 startPosition, Vector3 targetPosition)
    {
        _startPos = startPosition;
        _targetPos = targetPosition;
        switch(grade)
        {
            case EGrade.Normal:
                _self.color = Color.white;
                _trailRenderer.startColor = Color.white;
                _trailRenderer.endColor = Color.white;
                break;
            case EGrade.Rare:
                _self.color = Color.blue;
                _trailRenderer.startColor = Color.blue;
                _trailRenderer.endColor = Color.blue;
                break;
            case EGrade.Epic:
                _self.color = Color.magenta;
                _trailRenderer.startColor = Color.magenta;
                _trailRenderer.endColor = Color.magenta;
                break;
            case EGrade.Mythic:
                _self.color = Color.yellow;
                _trailRenderer.startColor = Color.yellow;
                _trailRenderer.endColor = Color.yellow;
                break;
        }
    }

    public void Play(Action onComplete)
    {
        Move(onComplete).AttachExternalCancellation(cancellationToken:destroyCancellationToken).Forget();
    }

    public async UniTask Move(Action onComplete)
    {
        while(_delta < 1.0f)
        {
            _delta += Time.deltaTime * _speed;
            _delta = Mathf.Clamp01(_delta);

            Vector3 midPoint = (_startPos + _targetPos) / 2 + Vector3.up * _height;
            Vector3 newPosition = CalculateQuadraticBezierPoint(_delta, _startPos, midPoint, _targetPos);
            transform.position = newPosition;
            await UniTask.NextFrame();
        }
        onComplete?.Invoke();
        Destroy(gameObject);
    }


    // https://youtu.be/Xwj8_z9OrFw?si=x1LzaOykKLDx9fF3
    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        return (u * u * p0) + (2 * u * t * p1) + (t * t * p2);
    }
}
