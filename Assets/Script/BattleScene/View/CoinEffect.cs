using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinEffect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private Image _coinImage;

    public TextMeshProUGUI CoinText => _coinText;
    public Image CoinImage => _coinImage;   

    public void SetText(int value)
    {
        _coinText.SetText($"+{value}");
    }



}
