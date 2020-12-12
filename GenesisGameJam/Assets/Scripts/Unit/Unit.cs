using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
	[Header("Values"), Space]
	[SerializeField] float speed = 4.0f;

	[Header("Refs"), Space]
	[SerializeField] SpriteRenderer arrowSr;
	[SerializeField] Transform rendererParent;

	Vector2 clickPos = Vector2.zero;

	private void Awake() {
		arrowSr.enabled = false;
	}

	void Update() {
		if (arrowSr.enabled) {
			Vector3 v = TemplateGameManager.Instance.Camera.ScreenToWorldPoint(Input.mousePosition).SetZ(0.0f) - transform.position;
			float rawAngle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;

			arrowSr.transform.localEulerAngles = arrowSr.transform.localEulerAngles.SetZ(rawAngle);
			arrowSr.size = arrowSr.size.SetX(v.magnitude);
		}
		
		if(clickPos != Vector2.zero) {
			Vector2 moveVector = clickPos - (Vector2)transform.position;
			float moveVal = speed * Time.deltaTime;
			if (moveVector.magnitude < moveVal)
				moveVal = moveVector.magnitude;
			transform.position += (Vector3)moveVector.normalized * moveVal;
		}
	}

	void OnMouseDown() {
		arrowSr.enabled = true;
		GameManager.Instance.IsCanMoveCamereByClick = false;
		//clickPos = Vector2.zero;

	}

	void OnMouseUp() {
		arrowSr.enabled = false;
		GameManager.Instance.IsCanMoveCamereByClick = true;
		clickPos = TemplateGameManager.Instance.Camera.ScreenToWorldPoint(Input.mousePosition).SetZ(0.0f);

		Vector2 moveVector = clickPos - (Vector2)transform.position;

		if ((moveVector.x < 0 && rendererParent.rotation.y != -180)) {
			LeanTween.cancel(rendererParent.gameObject);
			LeanTween.rotateY(rendererParent.gameObject, -180, 0.2f);
		}
		else if (moveVector.x > 0 && rendererParent.rotation.y != 0) {
			LeanTween.cancel(rendererParent.gameObject);
			LeanTween.rotateY(rendererParent.gameObject, 0, 0.2f);
		}
	}
}
