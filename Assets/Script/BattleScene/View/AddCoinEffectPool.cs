using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class AddCoinEffectPool : MonoBehaviour
{
    [SerializeField] private CoinEffect _coinEffect;
    [SerializeField] private RectTransform _target;
    [SerializeField] private float _duration;
    [SerializeField] private float _moveUpDistance;

    IObjectPool<CoinEffect> _pool;
    // Start is called before the first frame update
    void Start()
    {
        _pool = new ObjectPool<CoinEffect>(
            CreateDamagePrefab,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            true
            );
    }

    public void Show(int value)
    {
        var prefab = _pool.Get();

        prefab.SetText(value);
        prefab.transform.localScale = Vector3.one;
        prefab.transform.position = _target.position;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(prefab.transform.DOPunchScale(Vector3.one * 0.4f, 0.2f, 5, 0.5f));
        sequence.Append(prefab.transform.DOLocalMoveY(_moveUpDistance, _duration).SetRelative());
        sequence.Join(prefab.CoinImage.DOFade(0, _duration));
        sequence.Join(prefab.CoinText.DOFade(0, _duration));
        sequence.OnComplete(() =>
        {
            _pool.Release(prefab);
        });
    }

    private CoinEffect CreateDamagePrefab()
    {
        var prefab = Instantiate(_coinEffect, transform);
        prefab.gameObject.SetActive(false);
        return prefab;
    }

    private void OnTakeFromPool(CoinEffect coinEffct)
    {
        coinEffct.gameObject.SetActive(true);
    }

    private void OnReturnedToPool(CoinEffect coinEffect)
    {
        coinEffect.gameObject.SetActive(false);
        coinEffect.CoinImage.color = Color.white;
        coinEffect.CoinText.color = Color.white;
    }

    private void OnDestroyPoolObject(CoinEffect coinEffect)
    {
        Destroy(coinEffect.gameObject);
    }



}
