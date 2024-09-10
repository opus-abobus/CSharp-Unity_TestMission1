using UnityEngine;

public class ObservablesController : MonoBehaviour
{
    [SerializeField] private Transform _elementsUIRoot;

    [SerializeField] private GameObject _observableRoot;

    [SerializeField] private GameObject _elementPrefab;

    [field: SerializeField] public FocalObjectHandler FocalObjectHandler { get; private set; }
    [field: SerializeField] public SelectionHandler SelectionHandler { get; private set; }

    [SerializeField] private ModifySelectionHandler _modifySelectionHandler;

    private ElementController[] _observableElements;

    private GameObject[] GetObjects()
    {
        GameObject[] result = new GameObject[_observableRoot.transform.childCount];
        for (int i = 0; i < _observableRoot.transform.childCount; i++)
        {
            result[i] = _observableRoot.transform.GetChild(i).gameObject;
        }

        return result;
    }

    public void Init()
    {
        BuildList();

        FocalObjectHandler.Init(_observableElements);
        SelectionHandler.Init(_observableElements);
        _modifySelectionHandler.Init(_observableElements, SelectionHandler);
    }

    private void BuildList()
    {
        var observables = GetObjects();
        _observableElements = new ElementController[observables.Length];

        for (int i = 0; i < observables.Length; i++)
        {
            _observableElements[i] = Instantiate(_elementPrefab, _elementsUIRoot).GetComponent<ElementController>();
            _observableElements[i].SetObservable(observables[i]);
        }
    }

    public ElementController[] GetElements()
    {
        return _observableElements;
    }
}
