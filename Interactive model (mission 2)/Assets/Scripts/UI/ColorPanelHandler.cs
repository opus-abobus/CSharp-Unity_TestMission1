using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorPanelHandler : MonoBehaviour
{
    [SerializeField] private Slider _RSlider;
    [SerializeField] private Slider _GSlider;
    [SerializeField] private Slider _BSlider;

    [SerializeField] private TMP_InputField _RValueText;
    [SerializeField] private TMP_InputField _GValueText;
    [SerializeField] private TMP_InputField _BValueText;

    [SerializeField] private GameObject _panel;

    public event Action<Color> ColorChanged;

    public void Init()
    {
        _RSlider.onValueChanged.AddListener((value) =>
        {
            _RValueText.SetTextWithoutNotify(Mathf.RoundToInt(value * 255).ToString());

            ColorChanged?.Invoke(new Color(value, _GSlider.value, _BSlider.value));
        });

        _GSlider.onValueChanged.AddListener((value) =>
        {
            _GValueText.SetTextWithoutNotify(Mathf.RoundToInt(value * 255).ToString());

            ColorChanged?.Invoke(new Color(_RSlider.value, value, _BSlider.value));
        });

        _BSlider.onValueChanged.AddListener((value) =>
        {
            _BValueText.SetTextWithoutNotify(Mathf.RoundToInt(value * 255).ToString());

            ColorChanged?.Invoke(new Color(_RSlider.value, _GSlider.value, value));
        });


        _RValueText.onValueChanged.AddListener((value) =>
        {
            _RSlider.value = int.Parse(value) / 255.0f;

            ColorChanged?.Invoke(new Color(_RSlider.value, _GSlider.value, _BSlider.value));
        });

        _GValueText.onValueChanged.AddListener((value) =>
        {
            _GSlider.value = int.Parse(value) / 255.0f;

            ColorChanged?.Invoke(new Color(_RSlider.value, _GSlider.value, _BSlider.value));
        });

        _BValueText.onValueChanged.AddListener((value) =>
        {
            _BSlider.value = int.Parse(value) / 255.0f;

            ColorChanged?.Invoke(new Color(_RSlider.value, _GSlider.value, _BSlider.value));
        });
    }

    public void LoadPanel(Material material)
    {
        _RSlider.SetValueWithoutNotify(material.color.r);
        _GSlider.SetValueWithoutNotify(material.color.g);
        _BSlider.SetValueWithoutNotify(material.color.b);

        _RValueText.SetTextWithoutNotify(Mathf.RoundToInt(_RSlider.value * 255).ToString());
        _GValueText.SetTextWithoutNotify(Mathf.RoundToInt(_GSlider.value * 255).ToString());
        _BValueText.SetTextWithoutNotify(Mathf.RoundToInt(_BSlider.value * 255).ToString());

        _panel.SetActive(true);
    }
}