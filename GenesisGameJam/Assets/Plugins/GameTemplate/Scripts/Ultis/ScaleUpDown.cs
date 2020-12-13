using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ScaleUpDown : MonoBehaviour {
	[Header("Params")]
	[SerializeField] float minScale = 0.75f;
	[SerializeField] float maxScale = 1.0f;
	[SerializeField] float maxTime = 0.5f;
	[SerializeField] bool startFromRandom = true;

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

		minL = 0.4f;//minScale - (maxScale - 1);
		maxL = 1.0f;//minScale;

		if (startFromRandom) {
			currTime = Random.Range(0, maxTime);
			isIncreaseScale = Random.Range(0, 2) == 1;
		}
	}

	void OnEnable() {
		if (light == null) {
			currTime = Mathf.InverseLerp(minScale, maxScale, gameObject.transform.localScale.x);
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
