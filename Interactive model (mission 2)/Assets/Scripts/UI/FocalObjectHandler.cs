using System;
using UnityEngine;

public class FocalObjectHandler : MonoBehaviour
{
    [HideInInspector] public GameObject focalObject;

    public Action<GameObject> focalObjectChanged;

    private ElementController _previousFocalElement;

    private Action<bool, ElementController> FocusToggle;

    public void Init(ElementController[] observableElements)
    {
        FocusToggle += OnFocusToggle;

        foreach (var element in observableElements)
        {
            element.focusToggle.onValueChanged.AddListener((value) =>
            {
                FocusToggle?.Invoke(value, element);
            });
        }
    }

    private void OnFocusToggle(bool value, ElementController element)
    {
        if (_previousFocalElement == element)
        {
            element.outline.enabled = !element.outline.enabled;

            if (focalObject != null)
            {
                focalObject = null;
            }
            else
            {
                focalObject = element.GetObservable();
            }
        }
        else
        {
            element.outline.enabled = true;
            if (_previousFocalElement != null)
            {
                _previousFocalElement.outline.enabled = false;
            }

            focalObject = element.GetObservable();
        }

        focalObjectChanged?.Invoke(focalObject);

        _previousFocalElement = element;
    }
}
