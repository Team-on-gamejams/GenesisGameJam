using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nrjwolf.Tools.AttachAttributes;

public class ShowRadialMenuOnClick : MonoBehaviour {
	public bool isShowed = false;
	public bool isCanOpenMenu = true;

	[Header("Refs"), Space]
	[SerializeField, GetComponentInChildren] CircleSelectionHandler selectionHandler;
	
	private void OnMouseDown() {
		if (!isCanOpenMenu)
			return;

		isShowed = true;
		selectionHandler.transform.position = TemplateGameManager.Instance.Camera.ScreenToWorldPoint(Input.mousePosition).SetZ(selectionHandler.transform.position.z);
		selectionHandler.Show();
	}

	private void OnMouseUp() {
		if (!isCanOpenMenu)
			return;

		isShowed = false;
		selectionHandler.Hide();
	}
}
