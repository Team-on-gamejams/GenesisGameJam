using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class ResourceUI : MonoBehaviour {
	[Header("Data"), Space]
	[SerializeField] ResourceType type;

	[Header("Audio"), Space]
	[SerializeField] AudioClip counterSound;
	AudioSource counteras;

	[Header("Refs"), Space]
	[SerializeField] TextMeshProUGUI textField;
	[SerializeField] Image image;

	Coroutine coroutine;
	int oldValue;
	int currValue;

	private void Awake() {
		GameManager.Instance.player.onResourceChange += OnValueUpdated;
	}

	private void OnDestroy() {
		GameManager.Instance.player.onResourceChange -= OnValueUpdated;
	}

	public void DropWithFlyingParticles(int delta, Vector3 worldPos) {
		int pieaces = delta / 10 + (delta % 10 != 0 ? 1 : 0);
		while (pieaces != 0) {
			--pieaces;

			GameObject meatgo = new GameObject("Flying resource");
			meatgo.transform.SetParent(image.canvas.transform);
			Image img = meatgo.AddComponent<Image>();
			img.sprite = image.sprite;
			Color c = img.color;
			c.a = 0.0f;
			img.color = c;
			img.SetNativeSize();

			meatgo.transform.position = TemplateGameManager.Instance.Camera.WorldToScreenPoint(worldPos + (Vector3)Random.insideUnitCircle);
			meatgo.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360f));

			LeanTween.value(0, 1, 0.5f)
				.setOnUpdate((float a) => {
					c = img.color;
					c.a = a;
					img.color = c;
				})
				.setOnComplete(() => {
					Vector3 startPos = meatgo.transform.position;
					Vector3 startAngle = meatgo.transform.localEulerAngles;
					float dist = (image.transform.position - startPos).magnitude;
					int endedTweens = 0;
					bool isLastLoop = pieaces == 0;

					LeanTween.value(0, 1, dist / Screen.height * 0.8f)
					.setDelay(0.1f * pieaces)
					.setOnUpdate((float t) => {
						meatgo.transform.position = Vector3.Lerp(startPos, image.transform.position, t);
						meatgo.transform.localEulerAngles = Vector3.Lerp(startAngle, Vector3.zero, t);
					})
					.setOnComplete(() => {
						OnFlyEnd();
					});


					LeanTween.value(1, 0, 0.2f)
					.setDelay(0.1f * pieaces + dist / Screen.height * 0.64f)
					.setOnStart(() => {
						if (counteras == null) {
							counteras = AudioManager.Instance.Play(counterSound, channel: AudioManager.AudioChannel.Sound);
						}
					})
					.setOnUpdate((float t) => {
						c = img.color;
						c.a = t;
						img.color = c;
						meatgo.transform.localScale = Vector3.one * t;
					})
					.setEase(LeanTweenType.easeInCubic)
					.setOnComplete(() => {
						OnFlyEnd();
					});

					void OnFlyEnd() {
						++endedTweens;
						if (endedTweens != 2)
							return;

						if (delta >= 10) {
							GameManager.Instance.player[type] += 10;
							delta -= 10;
						}
						else {
							GameManager.Instance.player[type] += delta % 10;
							delta = 0;
						}

						if (counteras != null && isLastLoop) {
							counteras.Stop();
							counteras = null;
						}

						Destroy(meatgo, Random.Range(0.1f, 1.0f));
					}
				});
		}
	}


	void OnValueUpdated(ResourceType type, int newValue) {
		if(this.type == type) {
			currValue = newValue;

			if (coroutine != null)
				StopCoroutine(coroutine);
			coroutine = StartCoroutine(UpdateValueRoutine());
		}
	}

	IEnumerator UpdateValueRoutine() {
		while (oldValue != currValue) {
			int difference = currValue - oldValue;
			int absDifference = Math.Abs(difference);

			int fastAddValue = 10 + UnityEngine.Random.Range(0, 6);

			if(difference > 0) {
				//if (absDifference > fastAddValue)
				//	oldValue += fastAddValue;
				//else if (absDifference > 10)
				//	oldValue += 10;
				//else
					++oldValue;
			}
			else {
				//if (absDifference > fastAddValue)
				//	oldValue -= fastAddValue;
				//else if (absDifference > 10)
				//	oldValue -= 10;
				//else
					--oldValue;
			}

			textField.text = oldValue.ToString();
			yield return null;
		}
	}
}
