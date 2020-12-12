﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GeneralType {
	None = 0,

	PlayerBuilding = 10,
	PlayerUnit = 11,

	EnemyBuilding = 1000,
	EnemyUnit = 1001,
}

public enum ResourceType : byte { 
	Time = 0,
	Sunlight = 1,
	Water = 2,

	LastResource,
}
