using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ScaleUpDown : MonoBehaviour {
	[Header("Params")]
	[SerializeField] float minScale = 0.75f;
	[SerializeField] float maxScale = 1.0f;
	[SerializeField] float maxTime = 0.5f;

	Light2D light;
	float minL;
	float maxL;

	float currTime;
	bool isIncreaseScale = false;

	private void Awake() {
		light = GetComponent<Light2D>();
		if (light != null && light.lightType != Light2D.LightType.Point)
			light = null;

		if(light == null) {
			foreach (var l in GetComponentsInChildren<Light2D>(true)) {
				if (l.lightType == Light2D.LightType.Point) {
					light = l;
					break;
				}
			}
		}

		minL = 0.75f;//minScale - (maxScale - 1);
		maxL = 1.0f;//minScale;
	}

	void OnEnable() {
		if (light != null) {
			currTime = Mathf.InverseLerp(minScale, maxScale, gameObject.transform.localScale.x);
		}
		else {
			currTime = Random.Range(0, currTime);
			isIncreaseScale = Random.Range(0, 2) == 1;
		}

		gameObject.transform.localScale = Vector3.one * Mathf.SmoothStep(minScale, maxScale, currTime / maxTime);
		if (light)
			light.intensity = Mathf.SmoothStep(minL, maxL, currTime / maxTime);
	}

	void Update() {
		if (isIncreaseScale) {
			currTime += Time.deltaTime;
			if (currTime >= maxTime) {
				currTime = maxTime;
				isIncreaseScale = false;
			}
		}
		else {
			currTime -= Time.deltaTime;
			if (currTime <= 0) {
				currTime = 0;
				isIncreaseScale = true;
			}
		}

		gameObject.transform.localScale = Vector3.one * Mathf.SmoothStep(minScale, maxScale, currTime / maxTime);

		if (light) 
			light.intensity = Mathf.SmoothStep(minL, maxL, currTime / maxTime);
	}
}
