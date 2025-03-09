using UnityEngine;
using UnityEngine.UI;
using PCS.Common;
using UnityEngine.UIElements;

namespace PCS.UI
{
    public class UIAdjuster : MonoBehaviour
    {
        [SerializeField] private bool _useLocalScreenSize = false;
        [Condition(nameof(_useLocalScreenSize), true)][SerializeField] private Vector2 _referenceScreenSize = new Vector2(1080, 1920);
        
        [SerializeField] private bool _useLetterBox = false;
        private Camera _camera;
        public Camera MainCamera
        {
            get
            {
                if(_camera == null)
                    _camera = Camera.main;
                return _camera;
            }
        }

        [Fold("LetterBox")]
        [SerializeField] private CanvasScaler _letterBoxCanavasScaler;
        [SerializeField] private RectTransform _letterBox;
        [SerializeField] private RectTransform _letterBoxMaskSizer;
        [SerializeField] private RectTransform _letterBoxMask;
        [EndFold]

        [Fold("UIRoot")]
        [SerializeField] private RectTransform _rootCanvas;
        [SerializeField] private CanvasScaler _rootCanvasScaler;
        [SerializeField] private AspectRatioFitter _backgroundRatioFitter;
        [SerializeField] private RectTransform _holderPanel;
        [SerializeField] private AspectRatioFitter _holderAspectRatioFitter;

        [SerializeField] private RectTransform _actualAreaPanel;

        [SerializeField] private RectTransform _topPanel;
        [SerializeField] private RectTransform _centerPanel;
        [SerializeField] private RectTransform _bottomPanel;
        [SerializeField] private RectTransform _leftPanel;
        [SerializeField] private RectTransform _rightPanel;
        [SerializeField] private RectTransform _mainPanel;
        [EndFold]

        [Fold("Constants")]
        [SerializeField] private bool _useConstantTop;
        [Condition(nameof(_useConstantTop), true)][SerializeField] private float _topHeight;
        [Condition(nameof(_useConstantTop), true)][SerializeField] private int _topPriority;

        [SerializeField] private bool _useConstantBottom;
        [Condition(nameof(_useConstantBottom), true)][SerializeField] private float _bottomHeight;
        [Condition(nameof(_useConstantBottom), true)][SerializeField] private int _bottomPriority;

        [SerializeField] private bool _useConstantLeft;
        [Condition(nameof(_useConstantLeft), true)][SerializeField] private float _leftWidth;
        [Condition(nameof(_useConstantLeft), true)][SerializeField] private int _leftPriority;

        [SerializeField] private bool _useConstantRight;
        [Condition(nameof(_useConstantRight), true)][SerializeField] private float _rightWidth;
        [Condition(nameof(_useConstantRight), true)][SerializeField] private int _rightPriority;
        [EndFold]

        private float _referenceRatio => _referenceScreenSize.x / _referenceScreenSize.y;

        private void Start()
        {
            ScreenSizeChangeController.OnChanged += Apply;
            Apply(new Vector2(Screen.width, Screen.height));
        }

        private void OnDestroy()
        {
            ScreenSizeChangeController.OnChanged -= Apply;
        }

        public void Apply(Vector2 deviceResolution)
        {
            if (!_useLocalScreenSize)
                _referenceScreenSize = deviceResolution;

            RescaleCamera();

            _rootCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            _rootCanvasScaler.referenceResolution = _referenceScreenSize;

            _backgroundRatioFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            _backgroundRatioFitter.aspectRatio = _referenceRatio;

            _letterBoxCanavasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            _letterBoxCanavasScaler.referenceResolution = _referenceScreenSize;

            Vector2 minAnchor = Screen.safeArea.min;
            Vector2 maxAnchor = Screen.safeArea.max;

            minAnchor.x /= Screen.width;
            minAnchor.y /= Screen.height;
            maxAnchor.x /= Screen.width;
            maxAnchor.y /= Screen.height;
            
            _holderPanel.anchorMin = minAnchor;
            _holderPanel.anchorMax = maxAnchor;
            _letterBoxMaskSizer.anchorMin = minAnchor;
            _letterBoxMaskSizer.anchorMax = maxAnchor;

            _mainPanel.offsetMin = Vector2.zero;
            _mainPanel.offsetMax = Vector2.zero;
            _mainPanel.anchorMin = Vector2.zero;
            _mainPanel.anchorMax = Vector2.one;
            _mainPanel.localPosition = Vector3.zero;

            
            if (_useLetterBox)
            {
                if(Mathf.Approximately(_rootCanvas.rect.size.x, deviceResolution.x))
                    _holderAspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
                else
                    _holderAspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;

                _holderAspectRatioFitter.aspectRatio = deviceResolution.x/(float)deviceResolution.y;
                _holderPanel.sizeDelta = Vector2.zero;

                _letterBox.gameObject.SetActive(true);
                _letterBoxMask.sizeDelta = _referenceScreenSize;

                _actualAreaPanel.anchorMin = new Vector2(0.5f, 0.5f);
                _actualAreaPanel.anchorMax = new Vector2(0.5f, 0.5f);
                _actualAreaPanel.anchoredPosition = Vector3.zero;
                _actualAreaPanel.sizeDelta = _referenceScreenSize;
            }
            else
            {
                _letterBox.gameObject.SetActive(false);
                _holderAspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.None;
                _holderPanel.offsetMin = Vector2.zero;
                _holderPanel.offsetMax = Vector2.zero;

                _actualAreaPanel.offsetMin = Vector2.zero;
                _actualAreaPanel.offsetMax = Vector2.zero;
                _actualAreaPanel.anchorMin = Vector2.zero;
                _actualAreaPanel.anchorMax = Vector2.one;
            }
            

            Vector2 midOffsetMin = new Vector2(0, 0);
            Vector2 midOffsetMax = new Vector2(0, 0);

            float topOffset;
            float bottomOffset;
            float leftOffset;
            float rightOffset;

            if (_useConstantTop)
            {
                leftOffset = 0;
                rightOffset = 0;
                if(_leftPriority > _topPriority)
                    leftOffset = _leftWidth;
                if(_rightPriority > _topPriority)
                    rightOffset = _rightWidth;
                _topPanel.anchorMin = new Vector2(0, 1);
                _topPanel.anchorMax = new Vector2(1, 1);
                _topPanel.offsetMin = new Vector2(leftOffset, -_topHeight);
                _topPanel.offsetMax = new Vector2(-rightOffset, 0);
                midOffsetMax.y = -_topHeight;
            }

            if(_useConstantBottom)
            { 
                leftOffset = 0;
                rightOffset = 0;
                if (_leftPriority > _bottomPriority)
                    leftOffset = _leftWidth;
                if (_rightPriority > _bottomPriority)
                    rightOffset = _rightWidth;
                _bottomPanel.anchorMin = new Vector2(0, 0);
                _bottomPanel.anchorMax = new Vector2(1, 0);
                _bottomPanel.offsetMin = new Vector2(leftOffset, 0);
                _bottomPanel.offsetMax = new Vector2(-rightOffset,_bottomHeight);
                midOffsetMin.y = _bottomHeight;
            }

            if(_useConstantLeft)
            {
                topOffset = 0;
                bottomOffset = 0;
                if (_topPriority >= _leftPriority)
                    topOffset = _topHeight;
                if (_bottomPriority >= _leftPriority)
                    bottomOffset = _bottomHeight;
                _leftPanel.anchorMin = new Vector2(0, 0);
                _leftPanel.anchorMax = new Vector2(0, 1);
                _leftPanel.offsetMin = new Vector2(0, bottomOffset);
                _leftPanel.offsetMax = new Vector2(_leftWidth, -topOffset);
                midOffsetMin.x = _leftWidth;
            }

            if(_useConstantRight) 
            {
                topOffset = 0;
                bottomOffset = 0;
                if (_topPriority >= _rightPriority)
                    topOffset = _topHeight;
                if (_bottomPriority >= _rightPriority)
                    bottomOffset = _bottomHeight;
                _rightPanel.anchorMin = new Vector2(1, 0);
                _rightPanel.anchorMax = new Vector2(1, 1);
                _rightPanel.offsetMin = new Vector2(-_rightWidth, bottomOffset);
                _rightPanel.offsetMax = new Vector2(0, -topOffset);
                midOffsetMax.x = -_rightWidth;
            }

            _centerPanel.anchorMin = new Vector2(0, 0);
            _centerPanel.anchorMax = new Vector2(1, 1);
            _centerPanel.offsetMin = midOffsetMin;
            _centerPanel.offsetMax = midOffsetMax;
        }

        private void RescaleCamera()
        {
            if(!_useLetterBox)
            {
                MainCamera.rect = new Rect(0, 0, 1, 1);
                return;
            }

            float windowaspect = (float)Screen.width / (float)Screen.height;
            float scaleheight = windowaspect / _referenceRatio;

            if (scaleheight < 1.0f)
            {
                Rect rect = MainCamera.rect;

                rect.width = 1.0f;
                rect.height = scaleheight;
                rect.x = 0;
                rect.y = (1.0f - scaleheight) / 2.0f;

                MainCamera.rect = rect;
            }
            else
            {
                float scalewidth = 1.0f / scaleheight;

                Rect rect = MainCamera.rect;

                rect.width = scalewidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scalewidth) / 2.0f;
                rect.y = 0;

                MainCamera.rect = rect;
            }

        }

    }
}