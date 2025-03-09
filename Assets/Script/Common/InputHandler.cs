using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    public Camera Camera
    {
        get
        {
            if(_camera == null )
                _camera = Camera.main;
            return _camera;
        }
    }

    private HeroSlot _originSlot; // 선택된 슬롯
    private HeroSlot _targetSlot; // 이동 대상 슬롯

    private Vector3 _mouseDownPos;
    private float _dragThreshold = 5.0f;
    private bool _isDragging = false;

    public event Action<HeroSlot> OnSelectSlot; // 캐릭터가 선택되었을때, 캐릭터의 범위 표시
    public event Action<HeroSlot> OnClickSlot; // 캐릭터를 선택 후 드래그하지않고 땠을 때 판매, 조합 표시
    public event Action<HeroSlot> OnDragStart;
    public event Action<HeroSlot, HeroSlot> OnDragingSlot; // 캐릭터를 선택 후 드래그 할 때 각 위치 마커 표시
    public event Action<HeroSlot, HeroSlot> OnEndDragSlot; // 드래그가 끝나면 해당 각 위치의 영웅 자리 교환
    public event Action OnClickEmpty;

    private void Update()
    {
        OnMouseButtonDown();

        OnStartDrag();

        OnMouseButtonUp();
    }

    private void OnMouseButtonDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI())
                return;
            Vector2 rayPos = Camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);

            if (hit.collider == null || !hit.collider.TryGetComponent(out HeroSlot slot))
            {
                _originSlot = null;
                return;
            }

            _originSlot = slot;
            OnSelectSlot?.Invoke(slot);
            _mouseDownPos = Input.mousePosition;
            _isDragging = false;
        }
    }

    private void OnStartDrag()
    {
        if (Input.GetMouseButton(0))
        {
            if (IsPointerOverUI())
                return;
            if (_originSlot == null)
                return;

            if (Vector3.Distance(_mouseDownPos, Input.mousePosition) < _dragThreshold)
                return;

            OnDragStart?.Invoke(_originSlot);

            Vector2 rayPos = Camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);
            if (hit.collider == null || !hit.collider.TryGetComponent(out HeroSlot slot))
                return;

            if (slot == _originSlot)
                return;

            _isDragging = true;
            _targetSlot = slot;
            OnDragingSlot?.Invoke(_originSlot, _targetSlot);
        }
    }

    private void OnMouseButtonUp()
    {
        if(Input.GetMouseButtonUp(0))
        {
            if (IsPointerOverUI())
                return;
            if (_isDragging)
            {
                _isDragging = false;
                OnEndDragSlot?.Invoke(_originSlot, _targetSlot);
                OnClickEmpty?.Invoke();
                _originSlot = null;
                _targetSlot = null;
                return;
            }

            Vector2 rayPos = Camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);

            if (hit.collider == null || !hit.collider.TryGetComponent(out HeroSlot slot))
            {
                OnClickEmpty?.Invoke();
                _originSlot = null;
                return;
            }

            if (slot != _originSlot)
            {
                OnClickEmpty?.Invoke();
                _originSlot = null;
                return;
            }
            
            OnClickSlot?.Invoke(_originSlot);

        }
    }
    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

}
