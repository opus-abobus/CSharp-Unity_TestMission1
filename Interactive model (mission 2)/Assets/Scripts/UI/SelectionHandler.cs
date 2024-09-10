using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionHandler : MonoBehaviour
{
    [SerializeField] private Toggle _allElementsSelectionToggle;

    private ElementController[] _elementsControllers;

    private List<ElementController> _selectedElements = new List<ElementController>();

    public List<ElementController> GetSelectedElements()
    {
        return _selectedElements;
    }

    public void Init(ElementController[] controllers)
    {
        _elementsControllers = controllers;

        foreach (var element in _elementsControllers)
        {
            element.selectionToggle.onValueChanged.AddListener((value) => 
            {
                if (value)
                {
                    _selectedElements.Add(element);
                }
                else
                {
                    if (_selectedElements.Count == _elementsControllers.Length)
                    {
                        _allElementsSelectionToggle.isOn = false;
                    }

                    _selectedElements.Remove(element);
                }
            });
        }

        _allElementsSelectionToggle.onValueChanged.AddListener((value) => 
        {
            _selectedElements.Clear();

            if (value)
            {
                _selectedElements.AddRange(_elementsControllers);
            }

            foreach (var element in _elementsControllers)
            {
                element.selectionToggle.isOn = value;
            }
        });
    }

    private void OnDestroy()
    {
        foreach (var element in _elementsControllers)
        {
            element.selectionToggle.onValueChanged.RemoveAllListeners();
        }

        _allElementsSelectionToggle.onValueChanged.RemoveAllListeners();
    }
}
