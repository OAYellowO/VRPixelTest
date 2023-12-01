using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Reflection;

public class ColorController : MonoBehaviour
{
    [Serializable]
    public class PresetColor
    {
        public Button btn;
        public Color color;
    }

    public enum EColorType
    {
        R,
        G,
        B,
    }

    [Serializable]
    public class CustomSlider
    {
        public EColorType colorType;
        public Slider slider;
        public TextMeshProUGUI valueText;
    }

    [SerializeField] private Camera m_Camera = null;

    [Space(6)]
    [SerializeField] private GameObject m_ShowObj = null;
    [SerializeField] private GameObject m_RightRay = null;
    [SerializeField] private List<PresetColor> m_PresetColors = null;
    [SerializeField] private List<CustomSlider> m_CustomSliders = null;

    [Space(6)]
    [SerializeField] private InputActionReference m_IAR_ShowHideUI = null;
    [SerializeField] private InputActionReference m_IAR_SwitchColorL = null;
    [SerializeField] private InputActionReference m_IAR_SwitchColorR = null;
    [SerializeField] private float m_SwitchThreshold = 0.5f;

    private int _curColorIndex = 0;
    private bool _canSwitchLeft = true;
    private bool _canSwitchRight = true;

    private void Start()
    {
        for (int i = 0; i < m_PresetColors.Count; ++i)
        {
            int index = i;
            PresetColor presetColor = m_PresetColors[index];
            presetColor.btn.onClick.AddListener( () => { OnPresetColorBtnClicked(index); } );
        }

        for (int i = 0; i < m_CustomSliders.Count; ++i)
        {
            int index = i;
            CustomSlider customSlider = m_CustomSliders[index];
            customSlider.slider.onValueChanged.AddListener( (float value) => { OnCustomSliderValueChanged(index, value); });
        }

        if (m_IAR_ShowHideUI != null && m_IAR_ShowHideUI.action != null)
        {
            m_IAR_ShowHideUI.action.performed += OnThumbstickClicked;
        }
    }

    private void Update()
    {
        if (m_IAR_SwitchColorL != null && m_IAR_SwitchColorL.action != null)
        {
            Vector2 value = m_IAR_SwitchColorL.action.ReadValue<Vector2>();
            if (_canSwitchLeft)
            {
                if (value.x > m_SwitchThreshold)
                {
                    NextColor();
                    _canSwitchLeft = false;
                }
                else if (value.x < -m_SwitchThreshold)
                {
                    PreviousColor();
                    _canSwitchLeft = false;
                }
            }
            else
            {
                if (value.x > -m_SwitchThreshold && value.x < m_SwitchThreshold)
                {
                    _canSwitchLeft = true;
                }
            }
        }

        if (m_IAR_SwitchColorR != null && m_IAR_SwitchColorR.action != null)
        {
            Vector2 value = m_IAR_SwitchColorR.action.ReadValue<Vector2>();
            if (_canSwitchRight)
            {
                if (value.x > m_SwitchThreshold)
                {
                    NextColor();
                    _canSwitchRight = false;
                }
                else if (value.x < -m_SwitchThreshold)
                {
                    PreviousColor();
                    _canSwitchRight = false;
                }
            }
            else
            {
                if (value.x > -m_SwitchThreshold && value.x < m_SwitchThreshold)
                {
                    _canSwitchRight = true;
                }
            }
        }
    }

    private void OnPresetColorBtnClicked(int index)
    {
        SetColor(m_PresetColors[index].color);
        _curColorIndex = index;
    }

    private void OnCustomSliderValueChanged(int index, float value)
    {
        m_CustomSliders[index].valueText.text = (value * 255).ToString("F0");
        SetColor(GetCustomColor());
    }

    private Color GetCustomColor()
    {
        Color color = Color.white;

        foreach (CustomSlider cs in m_CustomSliders)
        {
            switch (cs.colorType)
            {
                case EColorType.R:
                    color.r = cs.slider.value;
                    break;
                case EColorType.G:
                    color.g = cs.slider.value;
                    break;
                case EColorType.B:
                    color.b = cs.slider.value;
                    break;
            }
        }

        return color;
    }

    private void NextColor()
    {
        _curColorIndex = (_curColorIndex + 1) % m_PresetColors.Count;
        SetColor(m_PresetColors[_curColorIndex].color);
    }

    private void PreviousColor()
    {
        _curColorIndex = _curColorIndex - 1;
        if (_curColorIndex < 0) _curColorIndex += m_PresetColors.Count;
        SetColor(m_PresetColors[_curColorIndex].color);
    }

    private void OnThumbstickClicked(InputAction.CallbackContext context)
    {
        bool targetState = !m_ShowObj.activeSelf;
        m_ShowObj.SetActive(targetState);
        m_RightRay.SetActive(targetState);
    }

    private void SetColor(Color color)
    {
        m_Camera.backgroundColor = color;
    }
}
