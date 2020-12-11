using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nrjwolf.Tools.AttachAttributes;

[RequireComponent(typeof(RMF_RadialMenu))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(ScaleUpDown))]
public class CircleSelectionHandler : MonoBehaviour {
	[Header("Timings"), Space]
	[SerializeField] float showTime = 0.2f;

	[Header("Refs"), Space]
	[SerializeField, GetComponent] CanvasGroup cg;
	[SerializeField, GetComponent] RMF_RadialMenu menu;
	[SerializeField, GetComponent] ScaleUpDown scaler;

	private void Awake() {
		transform.localScale = Vector3.zero;
		cg.interactable = cg.blocksRaycasts = false;
		menu.enabled = false;
		menu.isCanSelect = false;
		scaler.enabled = false;
	}

	public void Show() {
		LeanTween.cancel(gameObject, false);

		LeanTween.alphaCanvas(cg, 1.0f, showTime * 0.75f);
		LeanTween.scale(gameObject, Vector3.one, showTime)
			.setEase(LeanTweenType.easeOutBack)
			.setOnComplete(()=> { 
				scaler.enabled = true;
			});

		LeanTween.delayedCall(showTime / 2, () => {
			menu.isCanSelect = true;
		});

		cg.interactable = cg.blocksRaycasts = true;
		menu.enabled = true;
		GameManager.Instance.IsCanMoveCamereByClick = false;
	}

	public void Hide() {
		LeanTween.cancel(gameObject, false);
		
		scaler.enabled = false;

		LeanTween.alphaCanvas(cg, 0.0f, showTime);
		LeanTween.scale(gameObject, Vector3.zero, showTime * 0.75f)
			.setEase(LeanTweenType.easeInBack);

		StartCoroutine(InvokeNextFrame());

		IEnumerator InvokeNextFrame() {
			yield return null;
			cg.interactable = cg.blocksRaycasts = false;
			menu.isCanSelect = false;
			menu.enabled = false;
		}

		GameManager.Instance.IsCanMoveCamereByClick = true;
	}

	public void OnSelect(int id) {
		Debug.Log($"{id}");
	}
}
