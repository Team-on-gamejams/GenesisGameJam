using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTreeButtons : MonoBehaviour {
	[Header("Prefabs"), Space]
	[SerializeField] GameObject workerWispPrefab;
	[SerializeField] GameObject defenedWispPrefab;

	[Header("Points"), Space]
	[SerializeField] Transform centerPoint;
	[SerializeField] Transform workerSpawnPoint;
	[SerializeField] Transform defenderSpawnPoint;

	public void SpawnWorkerWisp() {
		GameObject go = Instantiate(workerWispPrefab, workerSpawnPoint.position, Quaternion.identity, null);
	}

	public void SpawnDefenderWisp() {
		GameObject go = Instantiate(defenedWispPrefab, defenderSpawnPoint.position, Quaternion.identity, null);

	}

	public void Upgrade() {
		Debug.LogWarning("Upgrade. [Not realised]");
	}
}