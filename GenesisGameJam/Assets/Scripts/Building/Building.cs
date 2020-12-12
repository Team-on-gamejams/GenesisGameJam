using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {
	[NaughtyAttributes.ReadOnly] public Health health;
	[NaughtyAttributes.ReadOnly] public RangeAttacker rangeAttacker;
	[NaughtyAttributes.ReadOnly] public ResourceCreator resourceCreator;
	[NaughtyAttributes.ReadOnly] public ShowRadialMenuOnClick radialMenu;

	private void Awake() {
		health = GetComponentInChildren<Health>();
		rangeAttacker = GetComponentInChildren<RangeAttacker>();
		resourceCreator = GetComponentInChildren<ResourceCreator>();
		radialMenu = GetComponent<ShowRadialMenuOnClick>();
	}

	private void Start() {
		if (health.IsPlayer()) {
			GameManager.Instance.player.AddBuilding(this);
		}
	}

	private void OnDestroy() {
		if (health.IsPlayer()) {
			GameManager.Instance.player.RemoveBuilding(this);
		}
	}
}
