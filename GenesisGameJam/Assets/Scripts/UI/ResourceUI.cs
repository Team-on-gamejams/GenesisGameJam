using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceUI : MonoBehaviour {
	[Header("Data"), Space]
	[SerializeField] ResourceType type;

	[Header("Refs"), Space]
	[SerializeField] TextMeshProUGUI textField;

	Coroutine coroutine;
	int oldValue;
	int currValue;

	private void Awake() {
		GameManager.Instance.player.onResourceChange += OnValueUpdated;
	}

	private void OnDestroy() {
		GameManager.Instance.player.onResourceChange -= OnValueUpdated;
	}

	void OnValueUpdated(ResourceType type, int newValue) {
		if(this.type == type) {
			currValue = newValue;

			if (coroutine != null)
				StopCoroutine(coroutine);
			coroutine = StartCoroutine(UpdateValueRoutine());
		}
	}

	IEnumerator UpdateValueRoutine() {
		while (oldValue != currValue) {
			int difference = currValue - oldValue;
			int absDifference = Math.Abs(difference);

			int fastAddValue = 10 + UnityEngine.Random.Range(0, 6);

			if(difference > 0) {
				if (absDifference > fastAddValue)
					oldValue += fastAddValue;
				else if (absDifference > 10)
					oldValue += 10;
				else
					++oldValue;
			}
			else {
				if (absDifference > fastAddValue)
					oldValue -= fastAddValue;
				else if (absDifference > 10)
					oldValue -= 10;
				else
					--oldValue;
			}

			textField.text = oldValue.ToString();
			yield return null;
		}
	}
}
