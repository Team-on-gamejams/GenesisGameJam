using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {
	[Header("Values"), Space]
	[SerializeField] float speed = 2;

	[Header("Refs"), Space]
	[SerializeField] Transform rendererParent;

	[NaughtyAttributes.ReadOnly] public Health health;
	[NaughtyAttributes.ReadOnly] public RangeAttacker rangeAttacker;

	Health target;
	Vector2 movePos = Vector2.zero;

	private void Awake() {
		health = GetComponentInChildren<Health>();
		rangeAttacker = GetComponentInChildren<RangeAttacker>();

		speed *= Random.Range(0.8f, 1.2f);
	}

	private void Start() {
		if (health.IsPlayer()) {
			GameManager.Instance.enemiesSpawner.AddUnit(this);
		}
	}

	private void OnDestroy() {
		if (health.IsPlayer()) {
			GameManager.Instance.enemiesSpawner.RemoveUnit(this);
		}
	}

	void Update() {
		if (movePos != Vector2.zero) {
			Vector2 moveVector = movePos - (Vector2)transform.position;
			float moveVal = speed * Time.deltaTime;
			if (moveVector.magnitude < moveVal) {
				moveVal = moveVector.magnitude;
				movePos = Vector2.zero;
			}
			transform.position += (Vector3)moveVector.normalized * moveVal;
		}
		else if (!target) {
			target = GameManager.Instance.player.GetNearestTargetForEnemy(transform.position);
		}
		else if((target.transform.position - transform.position).magnitude >= 2.0f) {
			MoveTo(target.transform.position + (Vector3)Random.insideUnitCircle.normalized * Random.Range(1, 3));
		}
	}

	public void SetTarget(Health h) {
		target = h;
		MoveTo(target.transform.position + (Vector3)Random.insideUnitCircle.normalized * Random.Range(1, 3));
	}

	void MoveTo(Vector2 worldPos) {
		movePos = worldPos - (Vector2)transform.position;

		if ((movePos.x < 0 && rendererParent.rotation.y != -180)) {
			LeanTween.cancel(rendererParent.gameObject);
			LeanTween.rotateY(rendererParent.gameObject, -180, 0.2f);
		}
		else if (movePos.x > 0 && rendererParent.rotation.y != 0) {
			LeanTween.cancel(rendererParent.gameObject);
			LeanTween.rotateY(rendererParent.gameObject, 0, 0.2f);
		}

		movePos = worldPos;
	}
}
