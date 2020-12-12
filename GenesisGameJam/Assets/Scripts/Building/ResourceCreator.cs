using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ResourceCreator : MonoBehaviour {
	[Header("Generators")]
	[SerializeField] ResourceGeneratorField[] generators;

	[Header("Refs"), Space]
	[SerializeField] CanvasGroup popupcg;
	[SerializeField] ScaleUpDown popupScaler;
	[SerializeField] Collider2D collider;
	ShowRadialMenuOnClick selectionHandler;

	const float showTime = 0.2f;
	const float minMinutesToCollect = 1.0f;
	const float minSecondsToCollect = minMinutesToCollect * 60.0f;

	long lastCollectTicks;
	float timeSinceLastCollectUnity = 0;

	bool isPopupShowed = false;

	void Awake() {
		selectionHandler = GetComponentInParent<ShowRadialMenuOnClick>();

		lastCollectTicks = DateTime.Now.Ticks;

		popupcg.alpha = 0.0f;
		popupcg.gameObject.transform.localScale = Vector3.zero;
		popupScaler.enabled = false;
		collider.enabled = false;
	}

	void Update() {
		if (!isPopupShowed && (selectionHandler == null || !selectionHandler.isShowed)) {
			if ((timeSinceLastCollectUnity += Time.deltaTime) > minSecondsToCollect) {
				ShowPopup();
			}
		}
		else if (isPopupShowed) {
			UpdatePopupValues();
		}
	}

	void OnMouseUpAsButton() {
		if (!isPopupShowed)
			return;

		long passedTicks = DateTime.Now.Ticks - lastCollectTicks;
		double minutesPassed = new TimeSpan(passedTicks).TotalMinutes;

		foreach (var gen in generators) {
			int created = Mathf.Clamp(Mathf.FloorToInt((float)(gen.perMinute * minutesPassed)), 0, gen.max);
			GameManager.Instance.player.CollectResource(gen.type, created, gen.anchor.position);
		}

		lastCollectTicks = DateTime.Now.Ticks;
		timeSinceLastCollectUnity = 0;
		HidePopup();
	}

	void ShowPopup() {
		if (isPopupShowed)
			return;
		isPopupShowed = true;

		if(selectionHandler)
			selectionHandler.isCanOpenMenu = false;

		collider.enabled = true;

		LeanTween.cancel(gameObject, false);

		LeanTween.alphaCanvas(popupcg, 1.0f, showTime * 0.75f);
		LeanTween.scale(popupcg.gameObject, Vector3.one, showTime)
		.setEase(LeanTweenType.easeOutBack)
		.setOnComplete(() => {
			popupScaler.enabled = true;
		});

		UpdatePopupValues();
	}

	void HidePopup() {
		if (!isPopupShowed)
			return;
		isPopupShowed = false;

		if (selectionHandler)
			selectionHandler.isCanOpenMenu = true;

		popupScaler.enabled = false;
		collider.enabled = false;

		LeanTween.alphaCanvas(popupcg, 0.0f, showTime * 0.75f);
		LeanTween.scale(popupcg.gameObject, Vector3.zero, showTime)
		.setEase(LeanTweenType.easeInBack);
	}

	void UpdatePopupValues() {
		long passedTicks = DateTime.Now.Ticks - lastCollectTicks;
		double minutesPassed = new TimeSpan(passedTicks).TotalMinutes;

		foreach (var gen in generators) {
			int created = Mathf.Clamp(Mathf.FloorToInt((float)(gen.perMinute * minutesPassed)), 0, gen.max);
			gen.textField.text = $"{created}/{gen.max}";
		}
	}

	[Serializable]
	struct ResourceGeneratorField {
		[Header("Production")]
		public ResourceType type;
		public float perMinute;
		public int max;

		[Header("Anhor drop")]
		public Transform anchor;

		[Header("Popup refs")]
		public TextMeshProUGUI textField;
	}
}
