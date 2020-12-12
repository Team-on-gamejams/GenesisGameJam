using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class RangeAttacker : MonoBehaviour {
	[Header("Values"), Space]
	[SerializeField] bool isTargetEnemy = false;
	[Space]
	[SerializeField] float range = 4.0f;
	[SerializeField] float attackTime = 1.0f;
	[Space]
	[SerializeField] int bulletDamage = 10;
	[SerializeField] int bulletSpeed = 10;

	
	float currAttackTime = 0.0f;

	[Header("Refs"), Space]
	[SerializeField] CircleCollider2D collider;
	[SerializeField] GameObject bulletPrefab;

	List<Health> inRange = new List<Health>(4);
	int nearestId = 0;

	private void Awake() {
		collider.radius = range;
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		Health h = collision.GetComponent<Health>();
		if(h.IsEnemy() == isTargetEnemy) {
			inRange.Add(h);
		}
	}

	private void OnTriggerExit2D(Collider2D collision) {
		Health h = collision.GetComponent<Health>();
		if (inRange.Contains(h)) {
			inRange.Remove(h);
		}
	}

	private void Update() {
		if(currAttackTime >= attackTime) {
			if(inRange.Count >= 1) {
				currAttackTime -= attackTime;

				GameObject bulletgo = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
				Bullet b = bulletgo.GetComponent<Bullet>();

				b.speed = bulletSpeed;
				b.damage = bulletDamage;
				b.target = GetTarget();
			}
		}
		else {
			currAttackTime += Time.deltaTime;
		}
	}

	Health GetTarget() {
		if(inRange.Count == 1) {
			return inRange[0];
		}

		int nearestUnit = -1;
		int nearestBuilding = -1;
		float nearestUnitDist = -1;
		float nearestBuildingDist = -1;

		float thisDist;

		for(int i = 0; i < inRange.Count; ++i) {
			thisDist = (inRange[i].transform.position - transform.position).magnitude;

			if (inRange[i].IsUnit()) {
				if (nearestUnit == -1 || thisDist < nearestUnitDist) {
					nearestUnit = i;
					nearestUnitDist = thisDist;
				}
			}
			else if(nearestUnit == -1){
				if (nearestBuilding == -1 || thisDist < nearestBuildingDist) {
					nearestBuilding = i;
					nearestBuildingDist = thisDist;
				}
			}
		}

		if (nearestUnit != -1)
			return inRange[nearestUnit];
		return inRange[nearestBuilding];
	}
}
