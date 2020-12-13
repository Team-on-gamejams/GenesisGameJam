using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

public class UIArrow : MonoBehaviour {
	[System.NonSerialized] public Transform pointTo;

	[SerializeField] ContactFilter2D filterPointToPlayer = new ContactFilter2D();
	List<RaycastHit2D> hits = new List<RaycastHit2D>(16);

	[Header("Scale tween")] [Space]
	[SerializeField] Vector3 minScale = new Vector3(0.65f, 0.65f, 1.0f);
	[SerializeField] Vector3 maxScale = new Vector3(0.95f, 0.95f, 1.0f);
	[SerializeField] float scaleTime = 1.2f;
	float currScaleTime = 0.0f;
	bool isScaleUp = true;

	[SerializeField] RectTransform rect;
	[SerializeField] Image img;
	[SerializeField] TextMeshProUGUI timerField;
	[SerializeField] CanvasGroup cg;

	BoxCollider2D screenFrames;
	float middleToFrameDist;
	float frameToScreenDist;

	bool isShowed;
	float maxA;
	Color c;

	Vector3 dir;
	int hitted;

	System.Func<string> secondsLeft;

	public void Init(Transform pointTo, BoxCollider2D screenFrames, float middleToFrameDist, float frameToScreenDist, float scale, System.Func<string> secondsLeft) {
		this.pointTo = pointTo;
		this.screenFrames = screenFrames;
		this.middleToFrameDist = middleToFrameDist;
		this.frameToScreenDist = frameToScreenDist;
		this.secondsLeft = secondsLeft;

		if(secondsLeft != null)
			timerField.text = "";

		minScale *= scale;
		maxScale *= scale;

		isShowed = false;


		c = img.color;
		maxA = c.a;
		c.a = 0.0f;
		img.color = c;
	}

	private void OnDestroy() {
		LeanTween.cancel(img.gameObject, false);
	}

	private void Update() {
		if(!pointTo) {
			return;
		}

		bool isInCameraView = screenFrames.OverlapPoint(pointTo.position);

		if (!isShowed && !isInCameraView) {
			Show();
		}

		if (isShowed) {
			if (isInCameraView) {
				Hide();
			}
			else {
				dir = (pointTo.position - TemplateGameManager.Instance.Camera.transform.position).normalized;
				dir.z = 0.0f;
				hitted = Physics2D.Raycast(pointTo.position, -dir, filterPointToPlayer, hits, 100.0f);

				for(int i = 0; i < hitted; ++i) {
					if(
						hits[i].collider == screenFrames
					) {
						rect.position = TemplateGameManager.Instance.Camera.WorldToScreenPoint(hits[i].point);
						rect.rotation = Quaternion.LookRotation(Vector3.forward, dir);
						break;
					}
				}
			}

			if (isScaleUp) {
				currScaleTime += Time.deltaTime;
				if (currScaleTime >= scaleTime) {
					currScaleTime = scaleTime;
					isScaleUp = false;
				}
			}
			else {
				currScaleTime -= Time.deltaTime;
				if (currScaleTime <= 0) {
					currScaleTime = 0.0f;
					isScaleUp = true;
				}
			}
			transform.localScale = Vector3.Lerp(minScale, maxScale, currScaleTime / scaleTime);

			c = img.color;
			c.a = Mathf.Lerp(0.0f, maxA, (((Vector2)pointTo.position - screenFrames.ClosestPoint(pointTo.position)).magnitude) / frameToScreenDist);
			img.color = c;
			cg.alpha = c.a;

			if (secondsLeft != null)
				timerField.text = secondsLeft.Invoke();
		}
	}

	void Show() {
		if (isShowed)
			return;
		isShowed = true;
		LeanTween.cancel(img.gameObject, false);
		LeanTween.value(img.gameObject, img.color.a, maxA, 0.3f)
		.setOnUpdate((float a) => {
			c = img.color;
			c.a = a;
			img.color = c;
			cg.alpha = a;
		});

		transform.localScale = minScale;
		isScaleUp = false;
		currScaleTime = Random.Range(0.0f, scaleTime);
	}

	void Hide() {
		if (!isShowed)
			return;
		isShowed = false;
		LeanTween.cancel(img.gameObject, false);
		LeanTween.value(img.gameObject, img.color.a, 0.0f, 0.1f)
		.setOnUpdate((float a) => {
			c = img.color;
			c.a = a;
			img.color = c;
			cg.alpha = a;
		});
	}

	public void Cheat_SkipTime(float seconds) {

	}
}
