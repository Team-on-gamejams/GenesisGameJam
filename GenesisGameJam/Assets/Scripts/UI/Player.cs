using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


	private void Awake() {
		GameManager.Instance.player = this;
	}

	private void OnDestroy() {
		GameManager.Instance.player = null;
	}
}
