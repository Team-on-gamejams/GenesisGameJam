using System.Collections;
using System.Collections.Generic;
using Nrjwolf.Tools.AttachAttributes;
using UnityEngine;

public class ShowRadialMenuOnClick : MonoBehaviour {
	[Header("Refs"), Space]
	[SerializeField, GetComponentInChildren] CircleSelectionHandler selectionHandler;
	
	private void OnMouseDown() {
		selectionHandler.transform.position = TemplateGameManager.Instance.Camera.ScreenToWorldPoint(Input.mousePosition).SetZ(selectionHandler.transform.position.z);

		selectionHandler.Show();
	}

	private void OnMouseUp() {
		selectionHandler.Hide();
	}
}
