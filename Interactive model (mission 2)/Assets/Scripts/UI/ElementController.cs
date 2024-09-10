using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElementController : MonoBehaviour
{
    public Toggle selectionToggle;

    public Toggle focusToggle;

    public TextMeshProUGUI observableNameText;

    public Outline outline;

    public Button colorSetButton;

    public Toggle visibilityToggle;

    private GameObject _observable;

    public GameObject GetObservable()
    {
        return _observable;
    }

    public void SetObservable(GameObject observable)
    {
        _observable = observable;

        observableNameText.text = _observable.name;
    }
}