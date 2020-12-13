using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;
using yaSingleton;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "GameManager", menuName = "Singletons/GameManager")]
public class GameManager : Singleton<GameManager> {
	public int minSecondsToShowResourcePopup = 60;

	[ReadOnly] public bool IsCanMoveCamereByClick = false;
	[ReadOnly] public SaveSpawner saveSpawner;
	[ReadOnly] public Player player;
	[ReadOnly] public EnemiesSpawner enemiesSpawner;
	[ReadOnly] public MapGenerator mapGenerator;
	[ReadOnly] public ArrowsNavigation arrows;

	protected override void Initialize() {
		Debug.Log("GameManager.Initialize()");
		base.Initialize();

		StartCoroutine(DelayedSetup());

		IEnumerator DelayedSetup() {
			yield return null;
			yield return null;
		}
	}

	protected override void Deinitialize() {
		base.Deinitialize();
	}
}
