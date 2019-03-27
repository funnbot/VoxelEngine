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

    protected void SetText(Text comp, string text) {
        if (text == null || text == "") {
            comp.text = "";
        } else {
            comp.text = text;
        }
    }

    protected void SetText(TMPro.TMP_Text comp, string text) {
        if (text == null || text == "") {
            comp.SetText("");
        } else {
            comp.SetText(text);
        }
    }

    protected void SetImage(Image comp, Sprite sprite) {
        if (sprite == null) {
            comp.gameObject.SetActive(false);
        } else {
            comp.sprite = sprite;
            comp.gameObject.SetActive(true);
        }
    }

    protected void SetTexture(SpriteRenderer comp, Texture2D tex) {
        if (tex == null) {
            comp.gameObject.SetActive(false);
        } else {
            var newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), comp.sprite.pivot);
            comp.sprite = newSprite;
            comp.gameObject.SetActive(true);
        }
    }

    protected virtual void AwakeImpl() { }

    void Awake() {
        transform = GetComponent<RectTransform>();
        graphic = GetComponent<Graphic>();
        AwakeImpl();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (noRaycastOnDrag) raycastTarget = false;
        OnDragBegin();
    }

    public virtual void OnDragBegin() { }

    public void OnEndDrag(PointerEventData eventData) {
        if (noRaycastOnDrag) raycastTarget = true;
        OnDragEnd();
    }

    public virtual void OnDragEnd() { }
}