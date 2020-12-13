using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData {
	const string playerPrefsKey = "whosyourdaddy";
	const string passPhrase = "there is no spoon";

	public long lastAttackTime;
	public Vector3 lastAttackPos;

	public int playerTime;
	public int playerSun;
	public int playerWater;

	public UnitSaveData[] units;
	public BuildingSaveData[] buildings;
	public UnitSaveData[] enemies;

	public bool IsExist() {
		return PlayerPrefs.HasKey(playerPrefsKey);
	}

	public SaveData Read() {
		string json = PlayerPrefs.GetString(playerPrefsKey);


		return JsonUtility.FromJson<SaveData>(StringCipher.Decrypt(json, passPhrase));
	}

	public void Write() {
		string json = JsonUtility.ToJson(this, true);

		PlayerPrefs.SetString(playerPrefsKey, StringCipher.Encrypt(json, passPhrase));
		PlayerPrefs.Save();
	}
}

[Serializable]
public class UnitSaveData {
	public GeneralType type;
	public Vector3 position;
	public int health;

	public UnitSaveData() {
	}

	public UnitSaveData(GeneralType type, Vector3 position, int health) {
		this.type = type;
		this.position = position;
		this.health = health;
	}
}

[Serializable]
public class BuildingSaveData : UnitSaveData {
	public long lastCollect;

	public BuildingSaveData() {
	}

	public BuildingSaveData(GeneralType type, Vector3 position, int health, long lastCollect) : base(type, position, health) {
		this.lastCollect = lastCollect;
	}
}
