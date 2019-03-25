using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UIButton : UIMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {
    public delegate void Click();
    public event Click OnClick;

    public Image TargetGraphic;
    public TMP_Text TargetText;
    [Space(10)]
    public Sprite HighlightSprite;
    public Color HighlightColor;
    public Color HighlightTextColor;
    public Vector2 HighlightTextShift;
    [Space]
    public Sprite PressedSprite;
    public Color PressedColor;
    public Color PressedTextColor;
    public Vector2 PressedTextShift;
    [Space]
    public Sprite DisabledSprite;
    public Color DisabledColor;
    public Color DisabledTextColor;
    public Vector2 DisabledTextShift;

    private Sprite sprite;
    private Color color;
    private Color textColor;
    private Vector2 textShift;

    private bool _interactable = true;
    public bool interactable {
        get => _interactable;
        set {
            if (_interactable = value) Default();
            else Disable();
        }
    }

    public void SetText(string text) {
        TargetText.text = text;
    }

    void Start() {
        sprite = TargetGraphic.sprite;
        color = TargetGraphic.color;
        textColor = TargetText.color;
        textShift = TargetText.rectTransform.localPosition;
    }

    void Reset() {
        TargetGraphic = GetComponent<Image>() ?? GetComponentInChildren<Image>();
        TargetText = GetComponent<TMP_Text>() ?? GetComponentInChildren<TMP_Text>();

        if (TargetGraphic != null) {
            HighlightSprite = PressedSprite = DisabledSprite = TargetGraphic.sprite;
            HighlightColor = PressedColor = DisabledColor = TargetGraphic.color;
        }
        if (TargetText != null) {
            HighlightTextColor = PressedTextColor = DisabledTextColor = TargetText.color;
            HighlightTextShift = PressedTextShift = DisabledTextShift = TargetText.rectTransform.localPosition;
        }
    }

    void Highlight() {
        if (HighlightSprite != null) TargetGraphic.sprite = HighlightSprite;
        if (HighlightColor != null) TargetGraphic.color = HighlightColor;
        if (HighlightTextColor != null) TargetText.color = HighlightTextColor;
        if (HighlightTextShift != null) TargetText.rectTransform.localPosition = HighlightTextShift;
    }

    void Pressed() {
        if (PressedSprite != null) TargetGraphic.sprite = PressedSprite;
        if (PressedColor != null) TargetGraphic.color = PressedColor;
        if (PressedTextColor != null) TargetText.color = PressedTextColor;
        if (PressedTextShift != null) TargetText.rectTransform.localPosition = PressedTextShift;
    }

    void Disable() {
        if (DisabledSprite != null) TargetGraphic.sprite = DisabledSprite;
        if (DisabledColor != null) TargetGraphic.color = DisabledColor;
        if (DisabledTextColor != null) TargetText.color = DisabledTextColor;
        if (DisabledTextShift != null) TargetText.rectTransform.localPosition = DisabledTextShift;
    }

    void Default() {
        if (sprite != null) TargetGraphic.sprite = sprite;
        if (color != null) TargetGraphic.color = color;
        if (textColor != null) TargetText.color = textColor;
        if (textShift != null) TargetText.rectTransform.localPosition = textShift;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (_interactable) Highlight();
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (_interactable) Default();
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (_interactable) Pressed();
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (_interactable) Highlight();
    }

    public void OnPointerClick(PointerEventData eventData) {
        OnClick?.Invoke();
    }
}