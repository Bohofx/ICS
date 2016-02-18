using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class WidgetFloatInspector : MonoBehaviour
{
	[SerializeField]
	Slider _slider;

	[SerializeField]
	InputField _inputField;

	[System.Serializable]
	public class FloatChangedEvent : UnityEvent<float>
	{ }

	public FloatChangedEvent onValueChanged = new FloatChangedEvent();

	void Awake()
	{
		_slider.onValueChanged.AddListener(OnSliderChanged);
		_inputField.onValueChanged.AddListener(OnInputFieldChanged);
	}

	void OnInputFieldChanged(string inValue)
	{
		float result = 0f;
		if(System.Single.TryParse(inValue, out result))
		{
			_slider.value = result;
			onValueChanged.Invoke(result);
		}
	}

	void OnSliderChanged(float inValue)
	{
		_inputField.text = inValue.ToString();
		onValueChanged.Invoke(inValue);
	}
}
