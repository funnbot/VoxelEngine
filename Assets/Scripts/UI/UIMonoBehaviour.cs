using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIMonoBehaviour : MonoBehaviour, IBeginDragHandler, IEndDragHandler {
    public bool noRaycastOnDrag;
    [System.NonSerialized]
    new public RectTransform transform;
    private Graphic graphic;

    protected bool raycastTarget {
        get => graphic.raycastTarget;
        set => graphic.raycastTarget = value;
    }

    void Awake() {
        transform = GetComponent<RectTransform>();
        graphic = GetComponent<Graphic>();
        AwakeImpl();
    }

    public void Disable() {
        gameObject.SetActive(false);
    }

    public void Enable() {
        gameObject.SetActive(true);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (noRaycastOnDrag) raycastTarget = false;
        OnBeginDrag();
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (noRaycastOnDrag) raycastTarget = true;
        OnEndDrag();
    }

    protected virtual void AwakeImpl() { }

    public virtual void OnBeginDrag() { }
    public virtual void OnEndDrag() { }
}