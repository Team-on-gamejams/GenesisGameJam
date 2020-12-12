using System;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	[NonSerialized] public Health target;
	Vector3 lastTargetPos;

	[NonSerialized] public int damage = 10;
	[NonSerialized] public int speed = 10;

	void Update() {
		if (target) {
			lastTargetPos = target.transform.position;
		}

		Vector2 moveVector = (Vector2)(lastTargetPos - transform.position);
		float moveVal = speed * Time.deltaTime;
		if (moveVector.magnitude < moveVal) {
			OnHit();
			target = null;
		}
		else {
			transform.position += (Vector3)moveVector.normalized * moveVal;
		}
	}

	void OnHit() {
		if(target)
			target.GetDamage(damage);
		Destroy(gameObject);
	}
}
