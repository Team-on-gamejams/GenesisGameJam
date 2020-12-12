using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesSpawner : MonoBehaviour {
	[Header("Values"), Space]
	[SerializeField] Vector2 enemiesPerTree = new Vector2(1, 2);
	[SerializeField] float secondsBetweenAttacks = 600;
	[SerializeField] float secondsToShowWarning = 60;
	long lastAttackTicks;

	[Header("Refs"), Space]
	[SerializeField] SpriteRenderer mapBorder;
	[SerializeField] GameObject enemyPrefab;
	[SerializeField] GameObject attackWarningPrefab;
	GameObject attackWarning;

	[NaughtyAttributes.ReadOnly] [SerializeField] List<EnemyAI> aliveEnemies;
	bool isAttackWarningShowed = false;

	private void Awake() {
		aliveEnemies = new List<EnemyAI>(16);

		GameManager.Instance.enemiesSpawner = this;

		lastAttackTicks = DateTime.Now.Ticks;
	}

	private void Update() {
		long passedTicks = DateTime.Now.Ticks - lastAttackTicks;
		double secondsPassed = new TimeSpan(passedTicks).TotalSeconds;

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
			new Vector3(UnityEngine.Random.Range(mapBorder.bounds.min.x, mapBorder.bounds.max.x),
						UnityEngine.Random.Range(mapBorder.bounds.min.y, mapBorder.bounds.max.y)), 
			Quaternion.identity,
			transform
		);
	}

	public void HideAttackWarning() {
		if (!isAttackWarningShowed)
			return;
		isAttackWarningShowed = false;

		Destroy(attackWarning);
	}

	public void Chear_SkipSeconds(float seconds) {
		lastAttackTicks -= Mathf.RoundToInt(seconds * TimeSpan.TicksPerSecond);
	}
}
