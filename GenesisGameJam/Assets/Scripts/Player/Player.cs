using System;
using System.Collections.Generic;
using UnityEngine;

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

	private void Awake() {
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
		GameManager.Instance.player = null;
		GameManager.Instance.IsCanMoveCamereByClick = false;
	}

	public void CollectResource(ResourceType type, int delta, Vector3 dropWorldPos) {
		resourceUIs[(int)type].DropWithFlyingParticles(delta, dropWorldPos);
	}

	[NaughtyAttributes.Button]
	public void Add() {
		this[ResourceType.Time] += 5;
		this[ResourceType.Sunlight] += 55;
		this[ResourceType.Water] += 500;
	}

	[NaughtyAttributes.Button]
	public void Add2() {
		CollectResource(ResourceType.Sunlight, 55, transform.position);
	}
}
