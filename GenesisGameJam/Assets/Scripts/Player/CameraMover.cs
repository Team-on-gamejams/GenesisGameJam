using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CameraMover : MonoBehaviour {
	[SerializeField] CanvasScaler scaler;
	[SerializeField] SpriteRenderer border;
	[Space]
	[SerializeField] float keyboardMapSensitivity = 1;
	[SerializeField] float mouseMapSensitivity = 1;


	Vector2 lastMoveValueWASD;
	Vector2 lastMoveValueDrag;
	bool isMouseDown = false;

	void LateUpdate() {
		if (isMouseDown && GameManager.Instance.IsCanMoveCamereByClick) {
			Vector2 mouseDelta = Mouse.current.delta.ReadValue();
			Vector2 delta =
				TemplateGameManager.Instance.Camera.ScreenToWorldPoint(Vector3.zero).SetZ(0.0f) -
				TemplateGameManager.Instance.Camera.ScreenToWorldPoint(mouseDelta).SetZ(0.0f);
			Debug.Log($"{delta} {mouseDelta} {Mouse.current.position.ReadValue()}");
			transform.position += (Vector3)delta;
		}
		else {
			transform.position += (Vector3)lastMoveValueWASD * keyboardMapSensitivity * Time.deltaTime;
		}

		transform.position = new Vector3(Mathf.Clamp(transform.position.x, border.bounds.min.x, border.bounds.max.x), 
			Mathf.Clamp(transform.position.y, border.bounds.min.y, border.bounds.max.y));
}

	public void OnMouseDrag(InputAction.CallbackContext context) {
		if (isMouseDown && context.performed) {
			
		}
	}

	public void OnWASDMove(InputAction.CallbackContext context) {
		lastMoveValueWASD = context.ReadValue<Vector2>();
	}

	public void OnMouseDown(InputAction.CallbackContext context) {
		isMouseDown = context.ReadValueAsButton();
		lastMoveValueDrag = Vector2.zero;
	}
}
