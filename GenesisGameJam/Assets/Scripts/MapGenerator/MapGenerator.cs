using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
	public enum PropsType : byte { Grass1, Grass2, Grass3, Stone1, Stone2, Stone3, Stump, Tree1, Tree2, Tree3, Tree4, Tree5, Wood};
	[System.Serializable]
	struct PropsToPrefab {
		public PropsType type;
		public GameObject prefab;
	}

	[SerializeField] PropsToPrefab[] prefabs;
	[SerializeField] SpriteRenderer mapBorders;

	[SerializeField] Vector2 PropSingleCount = new Vector2(4, 8);
	[SerializeField] Vector2 grassCluster = new Vector2(20, 30);
	[SerializeField] Vector2 PropClusterCount = new Vector2(4, 8);
	[SerializeField] Vector2 PropInClusterCount = new Vector2(2, 5);

	List<Prop> placedProps = new List<Prop>();
	bool isLoaded = false;

	private void Awake() {
		GameManager.Instance.mapGenerator = this;
	}

	private void Start() {
		if (!isLoaded) {
            CreateLevel();
        }
	}

	public void Load(MapSaveData[] data) {
		isLoaded = true;

		foreach (var prop in data) {
			GameObject prefab = null;
			foreach (var p in prefabs) {
				if (p.type == prop.type) {
					prefab = p.prefab;
					break;
				}
			}

			placedProps.Add(Instantiate(prefab, prop.position, Quaternion.identity).GetComponent<Prop>());
		}
	}

	public MapSaveData[] Save() {
		List<MapSaveData> map = new List<MapSaveData>();

		foreach (var prop in placedProps) {
			map.Add(new MapSaveData(prop.type, prop.transform.position));
		}

		return map.ToArray();
	}

	void CreateLevel() {
        PlaceSingleProp();
        PlacePropCluster();
        PlaceGrassCluster();
    }

    void PlaceSingleProp() {
        int Props = PropSingleCount.GetRandomValue();

        while (Props > 0) {
            --Props;
            CreateProp(prefabs[Random.Range(0, prefabs.Length)].prefab);
        }
    }

    void PlacePropCluster() {
        int clusters = PropClusterCount.GetRandomValue();

        while (clusters > 0) {
            --clusters;

            int Props = PropInClusterCount.GetRandomValue();
            Vector2 randomPos = GetRandomPos();

            while (Props > 0) {
                --Props;
                randomPos = CreateProp(prefabs[Random.Range(0, prefabs.Length)].prefab, randomPos);
            }
        }
    }

	void PlaceGrassCluster() {
		int clusters = grassCluster.GetRandomValue();

		while (clusters > 0) {
			--clusters;

			int Props = PropInClusterCount.GetRandomValue();
			Vector2 randomPos = GetRandomPos();

			while (Props > 0) {
				--Props;
				randomPos = CreateProp(prefabs[Random.Range(0, 3)].prefab, randomPos);
			}
		}
	}

	void CreateProp(GameObject prefab) {
        Vector2 randomPos = GetRandomPos();

        Prop prop = Instantiate(prefab, randomPos, Quaternion.identity, transform).GetComponent<Prop>();
        placedProps.Add(prop);
    }

    Vector3 CreateProp(GameObject prefab, Vector3 aboutPos) {
        float angle = Random.Range(0, 360);
        float dist = Random.Range(0.5f, 2.0f);
        Vector3 randomPos = aboutPos + new Vector3(dist * Mathf.Cos(angle), dist * Mathf.Sin(angle));

        Prop prop = Instantiate(prefab, randomPos, Quaternion.identity, transform).GetComponent<Prop>();
		placedProps.Add(prop);

		return randomPos;
    }

    Vector2 GetRandomPos() {
		Vector2 vector;

		do {
			vector = new Vector2(Random.Range(mapBorders.bounds.min.x, mapBorders.bounds.max.x),
							   Random.Range(mapBorders.bounds.min.y, mapBorders.bounds.max.y));
		} while (Mathf.Abs(vector.x) <= 4 && Mathf.Abs(vector.y) <= 4);

		return vector;
	}
}
