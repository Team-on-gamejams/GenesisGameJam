using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Health : MonoBehaviour {
	[Header("Values"), Space]
	public GeneralType type = GeneralType.None;
	public int maxHP = 100;
	public int currHP;

	[Header("Audio"), Space]
	[SerializeField] AudioClip onDie;

	[Header("UI")]
	[Space]
	[SerializeField] Canvas canvas = null;
	[SerializeField] Slider barFirst = null;
	[SerializeField] Slider barSecond = null;
	[SerializeField] TextMeshProUGUI hpTextField = null;
	[SerializeField] float firstBarTime = 0.2f;
	[SerializeField] float secondBarTime = 1.0f;

	private void Awake() {
		currHP = maxHP;

		barFirst.minValue = 0.0f;
		barFirst.maxValue = maxHP;
		barFirst.value = maxHP;

		barSecond.minValue = 0.0f;
		barSecond.maxValue = maxHP;
		barSecond.value = maxHP;

		if (hpTextField != null)
			hpTextField.text = $"{Mathf.RoundToInt(currHP)}/{Mathf.RoundToInt(maxHP)}";

		canvas.gameObject.SetActive(false);
	}

	public void SetCurrHealth(int val) {
		barSecond.value = barFirst.value = currHP = val;
		if (hpTextField != null)
			hpTextField.text = $"{Mathf.RoundToInt(currHP)}/{Mathf.RoundToInt(maxHP)}";

		if (!canvas.gameObject.activeSelf) {
			canvas.gameObject.SetActive(true);
		}
	}

	public void GetDamage(int damage) {
		currHP -= damage;

		if (currHP <= 0) {
			if (onDie)
				AudioManager.Instance.Play(onDie);
			Destroy(transform.parent.gameObject);
			return;
		}


		UpdateHpBar();
	}

	void UpdateHpBar() {
		if (!canvas.gameObject.activeSelf) {
			canvas.gameObject.SetActive(true);
		}

		LeanTween.cancel(barFirst.gameObject, false);
		LeanTween.value(barFirst.gameObject, barFirst.value, currHP, firstBarTime)
		.setOnUpdate((float hp) => {
			barFirst.value = hp;
		});

		LeanTween.cancel(barSecond.gameObject, false);
		LeanTween.value(barSecond.gameObject, barSecond.value, currHP, secondBarTime)
		.setEase(LeanTweenType.easeInQuad)
		.setOnUpdate((float hp) => {
			barSecond.value = hp;
		})
		.setOnComplete(() => {
			if(currHP == maxHP)
				canvas.gameObject.SetActive(false);
		});

		if (hpTextField != null)
			hpTextField.text = $"{Mathf.RoundToInt(currHP)}/{Mathf.RoundToInt(maxHP)}";
	}

	public bool IsEnemy() {
		switch (type) {
			case GeneralType.PlayerBuilding:
			case GeneralType.PlayerAdditionalTree:
			case GeneralType.PlayerUnitWorker:
			case GeneralType.PlayerUnitDefender:
				return false;

			case GeneralType.EnemyBuilding:
			case GeneralType.EnemyUnit:
				return true;
		}

		return false;
	}

	public bool IsPlayer() {
		return !IsEnemy();
	}

	public bool IsUnit() {
		switch (type) {
			case GeneralType.PlayerAdditionalTree:
			case GeneralType.PlayerBuilding:
			case GeneralType.EnemyBuilding:
				return false;

			case GeneralType.PlayerUnitWorker:
			case GeneralType.PlayerUnitDefender:
			case GeneralType.EnemyUnit:
				return true;
		}

		return false;
	}

	public bool IsBuilding() {
		return !IsUnit();
	}
}
