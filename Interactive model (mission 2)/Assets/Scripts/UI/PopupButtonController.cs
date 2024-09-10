using UnityEngine;
using UnityEngine.UI;

public class PopupButtonController : MonoBehaviour
{
    [SerializeField] private Button _button;

    [SerializeField] private GameObject _popupObject;

    private void Awake()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (_popupObject.activeSelf)
        {
            _popupObject.SetActive(false);
        }
        else
        {
            _popupObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnButtonClicked);
    }
}
