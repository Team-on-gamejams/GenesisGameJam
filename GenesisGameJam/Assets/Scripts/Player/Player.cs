using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
	public int this[ResourceType type] {
		get {
			return resources[(int)type];
		}
		set {
			resources[(int)type] = value;
			onResourceChange?.Invoke(type, value);
		}
	}
	[NonSerialized] public int[] resources;
	public Action<ResourceType, int> onResourceChange;

	[Header("Refs"), Space]
	[SerializeReference] ResourceUI[] resourceUIs;

	[Header("Start data"), Space]
	[SerializeField] int startTime = 10;
	[SerializeField] int startSunlight = 100;
	[SerializeField] int startWater = 100;

	[Header("Debug data"), Space]
	[NaughtyAttributes.ReadOnly] [SerializeField] List<Building> buildings;
	[NaughtyAttributes.ReadOnly] [SerializeField] List<Unit> units;

	private void Awake() {
		buildings = new List<Building>(16);
		units = new List<Unit>(16);
		resources = new int[(int)ResourceType.LastResource];

		GameManager.Instance.player = this;
	}

	private void Start() {
		this[ResourceType.Time] = startTime;
		this[ResourceType.Sunlight] = startSunlight;
		this[ResourceType.Water] = startWater;

		GameManager.Instance.IsCanMoveCamereByClick = true;
	}

	private void OnDestroy() {
		//GameManager.Instance.player = null;
		GameManager.Instance.IsCanMoveCamereByClick = false;
	}

	public void CollectResource(ResourceType type, int delta, Vector3 dropWorldPos) {
		resourceUIs[(int)type].DropWithFlyingParticles(delta, dropWorldPos);
	}

	public void AddUnit(Unit unit) {
		units.Add(unit);
	}

	public void RemoveUnit(Unit unit) {
		units.Remove(unit);
	}

	public void AddBuilding(Building b) {
		buildings.Add(b);
	}

	public void RemoveBuilding(Building b) {
		buildings.Remove(b);
	}

	public int GetBuildingCount() => buildings.Count;

	public Health GetNearestTargetForEnemy(Vector3 position) {
		int nearestUnit = -1;
		int nearestBuilding = -1;
		float nearestUnitDist = -1;
		float nearestBuildingDist = -1;

		float thisDist;

		for (int i = 0; i < buildings.Count; ++i) {
			thisDist = (buildings[i].transform.position - position).magnitude;
			if (nearestBuilding == -1 || thisDist < nearestBuildingDist) {
				nearestBuilding = i;
				nearestBuildingDist = thisDist;
			}
		}

		for (int i = 0; i < units.Count; ++i) {
			thisDist = (units[i].transform.position - position).magnitude;
			if (nearestUnit == -1 || thisDist < nearestUnitDist) {
				nearestUnit = i;
				nearestUnitDist = thisDist;
			}
		}

		if(nearestBuilding != -1 && nearestUnit != -1) {
			if(nearestUnitDist < nearestBuildingDist) 
				return units[nearestUnit].health;
			return buildings[nearestBuilding].health;
		}

		if (nearestBuilding != -1)
			return buildings[nearestBuilding].health;

		if (nearestUnit != -1)
			return units[nearestUnit].health;

		return null;
	}

	public void Cheat_SkipMinute() {
		float skippedSeconds = 30.0f;

		foreach (var b in buildings) {
			if (b.resourceCreator) {
				b.resourceCreator.Chear_SkipSeconds(skippedSeconds);
			}
		}

		GameManager.Instance.enemiesSpawner.Chear_SkipSeconds(skippedSeconds);
	}

	public void Cheat_RestartGameClear() {
		Scene scene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(scene.name);
	}
}
