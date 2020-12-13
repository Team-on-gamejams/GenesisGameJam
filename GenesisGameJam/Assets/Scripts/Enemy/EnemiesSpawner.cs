using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesSpawner : MonoBehaviour {
	[Header("Values"), Space]
	[SerializeField] Vector2 enemiesPerTree = new Vector2(1, 2);
	[SerializeField] float secondsBetweenAttacks = 600;
	[SerializeField] float secondsToShowWarning = 60;
	[NonSerialized] public long lastAttackTicks = 0;
	public Vector3 AttackPos{
		get{
			if (!attackWarning) {
				return Vector3.zero;
			}
			return attackWarning.transform.position;
		}
		set {
			if (value == Vector3.zero)
				return;
			if (!attackWarning) {
				long passedTicks = DateTime.Now.Ticks - lastAttackTicks;
				secondsPassed = new TimeSpan(passedTicks).TotalSeconds;
				if (!isAttackWarningShowed && secondsPassed >= secondsToShowWarning) {
					ShowAttackWarning();
				}
			}

			if (attackWarning)
				attackWarning.transform.position = value;
		}
	}

	[Header("Refs"), Space]
	[SerializeField] SpriteRenderer mapBorder;
	[SerializeField] GameObject enemyPrefab;
	[SerializeField] GameObject attackWarningPrefab;
	GameObject attackWarning;

	[NaughtyAttributes.ReadOnly] [SerializeField] List<EnemyAI> aliveEnemies;
	bool isAttackWarningShowed = false;
	double secondsPassed;

	private void Awake() {
		aliveEnemies = new List<EnemyAI>(16);

		GameManager.Instance.enemiesSpawner = this;

		if(lastAttackTicks == 0)
			lastAttackTicks = DateTime.Now.Ticks;
	}

	private void Update() {
		long passedTicks = DateTime.Now.Ticks - lastAttackTicks;
		secondsPassed = new TimeSpan(passedTicks).TotalSeconds;

		if(secondsPassed >= secondsBetweenAttacks) {
			lastAttackTicks = DateTime.Now.Ticks;

			int playerBuildings = GameManager.Instance.player.GetBuildingCount();
			int neededEnemies = Mathf.RoundToInt(UnityEngine.Random.Range(enemiesPerTree.x * playerBuildings, enemiesPerTree.y * playerBuildings));
			Health target = GameManager.Instance.player.GetNearestTargetForEnemy(attackWarning.transform.position);

			while (neededEnemies-- != 0) {
				GameObject enemygo = Instantiate(enemyPrefab, attackWarning.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * 2, Quaternion.identity);
				EnemyAI enemy = enemygo.GetComponent<EnemyAI>();
				enemy.SetTarget(target);
			}

			HideAttackWarning();
		}
		else if (!isAttackWarningShowed && secondsPassed >= secondsToShowWarning) {
			ShowAttackWarning();
		}
	}

	

	public void AddUnit(EnemyAI unit) {
		aliveEnemies.Add(unit);
	}

	public void RemoveUnit(EnemyAI unit) {
		aliveEnemies.Remove(unit);
	}

	public void ShowAttackWarning() {
		if (isAttackWarningShowed)
			return;
		isAttackWarningShowed = true;

		attackWarning = Instantiate(
			attackWarningPrefab,
			GetRandomPos(), 
			Quaternion.identity,
			transform
		);

		GameManager.Instance.arrows.AddArrow(attackWarning.transform, 1.0f, ()=> {
			float timeLeft = (float)(secondsBetweenAttacks - secondsPassed);
			if (timeLeft < 0)
				timeLeft = 0;
			return ((int)(timeLeft) / 60).ToString() + "." + (Mathf.RoundToInt(timeLeft) % 60).ToString("00");
		});
	}

	public void HideAttackWarning() {
		if (!isAttackWarningShowed)
			return;
		isAttackWarningShowed = false;

		GameManager.Instance.arrows.RemoveArrow(attackWarning.transform);
		Destroy(attackWarning);
	}

	public void LoadUnits(UnitSaveData[] units) {
		foreach (var u in units) {
			GameManager.Instance.saveSpawner.Spawn(u);
		}
	}

	public UnitSaveData[] SaveUnits() {
		List<UnitSaveData> units = new List<UnitSaveData>();

		foreach (EnemyAI ai in FindObjectsOfType<EnemyAI>()) {
			units.Add(new UnitSaveData(ai.health.type, ai.transform.position, ai.health.currHP));
		}

		return units.ToArray();
	}

	Vector2 GetRandomPos() {
		Vector2 vector;

		do {
			vector = new Vector2(UnityEngine.Random.Range(mapBorder.bounds.min.x, mapBorder.bounds.max.x),
							   UnityEngine.Random.Range(mapBorder.bounds.min.y, mapBorder.bounds.max.y));
		} while (Mathf.Abs(vector.x) <= 10 && Mathf.Abs(vector.y) <= 10);

		return vector;
	}

	public void Chear_SkipSeconds(float seconds) {
		lastAttackTicks -= Mathf.RoundToInt(seconds * TimeSpan.TicksPerSecond);
	}
}
