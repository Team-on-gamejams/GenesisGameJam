using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowsNavigation : MonoBehaviour {
	[SerializeField] UIArrow arrowPrefab;
	[SerializeField] float framesSize = 0.9f;

	BoxCollider2D screenFrames;
	float middleToFrameDist;
	float frameToScreenDist;

	List<UIArrow> arrows = new List<UIArrow>(4);

	private void Awake() {
		GameManager.Instance.arrows = this;

		screenFrames = TemplateGameManager.Instance.Camera.gameObject.AddComponent<BoxCollider2D>();
		screenFrames.isTrigger = true;
		screenFrames.gameObject.layer = LayerMask.NameToLayer("Water");
		screenFrames.transform.position = Vector3.zero;
		screenFrames.size = TemplateGameManager.Instance.Camera.ViewportToWorldPoint(Vector3.one * framesSize) - TemplateGameManager.Instance.Camera.ViewportToWorldPoint(Vector3.zero);

		Vector2 dist = (TemplateGameManager.Instance.Camera.ViewportToWorldPoint(Vector3.one * (1.0f - framesSize) / 2) - TemplateGameManager.Instance.Camera.ViewportToWorldPoint(Vector3.zero));
		frameToScreenDist = (dist.x + dist.y) / 2;

		dist = (TemplateGameManager.Instance.Camera.ViewportToWorldPoint(Vector3.one * framesSize) - TemplateGameManager.Instance.Camera.ViewportToWorldPoint(Vector3.zero));
		middleToFrameDist = Mathf.Min(dist.x, dist.y);
	}

	public void AddArrow(Transform pointTo, float scale, System.Func<string> secondsLeft) {
		UIArrow arrow = Instantiate(arrowPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<UIArrow>();
		arrow.Init(pointTo, screenFrames, middleToFrameDist, frameToScreenDist, scale, secondsLeft);
		arrows.Add(arrow);
	}

	public void RemoveArrow(Transform pointTo) {
		foreach (var arrow in arrows) {
			if(arrow.pointTo == pointTo) {
				Destroy(arrow.gameObject);
				arrows.Remove(arrow);
				break;
			}
		}
	}

	public void Cheat_SkipTime(float seconds) {
		foreach (var arrow in arrows) {
			arrow.Cheat_SkipTime(seconds);
		}
	}
}
