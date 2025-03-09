using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PCS.UI
{
    public class ScreenSizeChangeListener : MonoBehaviour
    {
        private void OnRectTransformDimensionsChange()
        {
            UniTask.Create(async () =>
            {
                await UniTask.DelayFrame(2);
                ScreenSizeChangeController.Notify();
            }).Forget();
        }
    }
}
