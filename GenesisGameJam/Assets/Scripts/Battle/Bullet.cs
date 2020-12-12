using System;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	[NonSerialized] public Health target;

	[NonSerialized] public int damage = 10;
	[NonSerialized] public int speed = 10;

	void Update() {
		if (target) {
			Vector2 moveVector = (Vector2)(target.transform.position - transform.position);
			float moveVal = speed * Time.deltaTime;
			if (moveVector.magnitude < moveVal) {
				OnHit();
				target = null;
			}
			else {
				transform.position += (Vector3)moveVector.normalized * moveVal;
			}
		}
	}

	void OnHit() {
		target.GetDamage(damage);
		Destroy(gameObject);
	}
}
