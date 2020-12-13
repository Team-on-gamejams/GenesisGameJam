using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSpawner : MonoBehaviour {
	[SerializeField] GeneralToPrefab[] prefabs;

	private void Awake() {
		GameManager.Instance.saveSpawner = this;
	}


	public void Spawn(UnitSaveData unit) {
		GameObject prefab = null;
		foreach (var p in prefabs) {
			if(p.type == unit.type) {
				prefab = p.prefab;
				break;
			}
		}

		GameObject go = Instantiate(prefab, unit.position, Quaternion.identity);
		Unit u = go.GetComponent<Unit>();
		if (u != null)
			u.health.SetCurrHealth(unit.health);

		EnemyAI ai = go.GetComponent<EnemyAI>();
		if (ai != null)
			ai.health.SetCurrHealth(unit.health);
	}


	public void Spawn(BuildingSaveData building) {
		GameObject prefab = null;
		foreach (var p in prefabs) {
			if (p.type == building.type) {
				prefab = p.prefab;
				break;
			}
		}

		Building b = Instantiate(prefab, building.position, Quaternion.identity).GetComponent<Building>();
		b.health.SetCurrHealth(building.health);
		b.resourceCreator.lastCollectTicks = building.lastCollect;
		b.resourceCreator.timeSinceLastCollectUnity = (float)new System.TimeSpan(System.DateTime.Now.Ticks - building.lastCollect).TotalSeconds;
	}

	[System.Serializable]
	class GeneralToPrefab {
		public GeneralType type;
		public GameObject prefab;
	}
}
