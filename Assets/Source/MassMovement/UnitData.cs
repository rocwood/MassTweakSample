using System;
using Unity.Mathematics;

[Serializable]
public struct UnitBaseData
{
	public int teamId;
	public float radius;
	public float speed;

	public float3 position;
	public float3 targetPos;
}

[Serializable]
public struct UnitCombatData
{
	public float attack;
	public float range;
	public float cooldown;
	public float hpMax;

	public int target;
	public float timer;
}
