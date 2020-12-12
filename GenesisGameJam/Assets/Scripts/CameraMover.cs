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

	float aspect;
	float mouseSens;

	private void Start() {
		float screenWidth = Screen.width;
		float screenHeight = Screen.height;

		float calcWidth = screenWidth * (screenHeight / scaler.referenceResolution.y);
		float calcHeight = screenHeight * (screenWidth / scaler.referenceResolution.x);

		if (calcWidth >= screenWidth) {
			aspect = screenHeight / scaler.referenceResolution.y;
		}
		else {
			aspect = screenWidth / scaler.referenceResolution.x;
		}

		mouseSens = mouseMapSensitivity / aspect;
	}

	void LateUpdate() {
		if (isMouseDown && GameManager.Instance.IsCanMoveCamereByClick) {
			transform.position += (Vector3)lastMoveValueDrag * mouseMapSensitivity / aspect;
		}
		else {
			transform.position += (Vector3)lastMoveValueWASD * keyboardMapSensitivity * Time.deltaTime;
		}

		transform.position = new Vector3(Mathf.Clamp(transform.position.x, border.bounds.min.x, border.bounds.max.x), 
			Mathf.Clamp(transform.position.y, border.bounds.min.y, border.bounds.max.y));
}

	public void OnMouseDrag(InputAction.CallbackContext context) {
		if (isMouseDown)
			lastMoveValueDrag = context.ReadValue<Vector2>();
	}

	public void OnWASDMove(InputAction.CallbackContext context) {
		lastMoveValueWASD = context.ReadValue<Vector2>();
	}

	public void OnMouseDown(InputAction.CallbackContext context) {
		isMouseDown = context.ReadValueAsButton();
		lastMoveValueDrag = Vector2.zero;
	}
}
