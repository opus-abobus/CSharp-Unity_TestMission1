using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModifySelectionHandler : MonoBehaviour
{
    private SelectionHandler _selectionHandler;

    [SerializeField] private Color _visibilityToggleOnColor;
    [SerializeField] private Color _visibilityToggleOffColor;

    [SerializeField] private Material _opaqueMat;
    [SerializeField] private Material _transparentMat;

    [SerializeField] private ColorPanelHandler _colorPanelHandler;

    [SerializeField] private Toggle _selectionVisibilityToggle;

    [SerializeField] private Transform _opacityButtonsRoot;

    private Dictionary<Button, float> _opacityButtons = new Dictionary<Button, float>{};

    public void Init(ElementController[] controllers, SelectionHandler selectionHandler)
    {
        _selectionHandler = selectionHandler;

        InitOpacityButtons();

        foreach (var element in controllers)
        {
            element.colorSetButton.onClick.AddListener(() =>
            {
                _colorPanelHandler.LoadPanel(element.GetObservable().GetComponent<Renderer>().material);

                OnColorChangeButton(element);
            });

            _selectionVisibilityToggle.image.color = _visibilityToggleOnColor;
            _selectionVisibilityToggle.onValueChanged.AddListener((value) =>
            {
                SetVisibilityForSelection(value, _selectionHandler.GetSelectedElements(), false);
            });

            SetVisibilityToggle(element);
        }

        _colorPanelHandler.Init();
        _colorPanelHandler.ColorChanged += OnColorChanged;
    }

    private ElementController _editColorElement;
    private void OnColorChangeButton(ElementController element)
    {
        _editColorElement = element;
    }

    private void OnColorChanged(Color color)
    {
        var mat = _editColorElement.GetObservable().GetComponent<Renderer>().material;
        float alpha = mat.color.a;

        mat.color = new Color(color.r, color.g, color.b, alpha);
    }

    private void InitOpacityButtons()
    {
        _opacityButtons = new Dictionary<Button, float>(_opacityButtonsRoot.childCount);
        Button button;

        for (int i = 0; i < _opacityButtonsRoot.childCount; i++)
        {
            button = _opacityButtonsRoot.GetChild(i).GetComponent<Button>();
            _opacityButtons.Add(button, button.image.color.a);
        }

        foreach (var key in _opacityButtons.Keys)
        {
            key.onClick.AddListener(() =>
            {
                var selection = _selectionHandler.GetSelectedElements();
                foreach (var element in selection)
                {
                    SetTransparency(element.GetObservable().GetComponent<Renderer>(), key.image.color.a);
                }
            });
        }
    }

    private void SetVisibilityForSelection(bool isVisible, List<ElementController> elements, bool setInverse = false)
    {
        if (elements.Count == 0)
        {
            _selectionVisibilityToggle.SetIsOnWithoutNotify(isVisible);
            return;
        }

        if (!setInverse)
        {
            foreach (var element in elements)
            {
                SetVisibility(element.GetObservable(), isVisible);

                element.visibilityToggle.isOn = isVisible;
            }
        }
        else
        {
            bool isActive;
            foreach (var element in elements)
            {
                isActive = element.GetObservable().activeSelf;
                SetVisibility(element.GetObservable(), !isActive);

                element.visibilityToggle.isOn = !isActive;
            }
        }

        if (isVisible)
        {
            _selectionVisibilityToggle.image.color = _visibilityToggleOnColor;
        }
        else
        {
            _selectionVisibilityToggle.image.color = _visibilityToggleOffColor;
        }
    }

    private void SetTransparency(Renderer renderer, float opacity)
    {
        Color color = renderer.material.color;

        if (opacity > 0.9f)
        {
            renderer.material = _opaqueMat;
            renderer.material.color = color;
        }
        else
        {
            renderer.material = _transparentMat;

            color.a = opacity;
            renderer.material.color = color;
        }
    }

    private void SetVisibility(GameObject gameObject, bool visibility)
    {
        gameObject.SetActive(visibility);
    }

    private void SetVisibilityToggle(ElementController element)
    {
        element.visibilityToggle.image.color = _visibilityToggleOnColor;

        element.visibilityToggle.onValueChanged.AddListener((value) =>
        {
            element.GetObservable().SetActive(value);

            if (value)
            {
                element.visibilityToggle.image.color = _visibilityToggleOnColor;
            }
            else
            {
                element.visibilityToggle.image.color = _visibilityToggleOffColor;
            }
        });
    }
}
