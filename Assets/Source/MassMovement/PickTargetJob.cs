using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
struct PickTargetJob : IJobParallelFor
{
	[ReadOnly] public NativeArray<UnitBaseData> unitBaseArray;
	[ReadOnly] public NativeArray<float> unitHealthArray;
	[WriteOnly] public NativeArray<int> unitAttackArray;
	public NativeArray<UnitCombatData> unitCombatArray;

	public int unitCount;
	public float dt;

	public void Execute(int i)
	{
		var unit = unitBaseArray[i];
		if (unit.teamId <= 0)
			return;

		if (unitHealthArray[i] <= 0)
			return;

		var combat = unitCombatArray[i];
		if (combat.attack <= 0 || combat.range < 0)
			return;

		// 检查冷却时间
		combat.timer += dt;
		if (combat.timer < combat.cooldown)
		{
			unitCombatArray[i] = combat;
			unitAttackArray[i] = -1;
			return;
		}

		int targetId = -1;
		var distMin = -1f;

		// 已有目标，检查是否依然有效
		if (combat.target >= 0)
		{
			if (CheckTarget(unit, combat.range, combat.target, ref distMin))
				targetId = combat.target;
		}

		// 做一次目标查找
		if (targetId < 0)
		{
			for (int j = 0; j < unitCount; j++)
			{
				if (i == j)
					continue;

				if (CheckTarget(unit, combat.range, j, ref distMin))
					targetId = j;
			}
		}

		// 锁定目标
		combat.target = targetId;

		//if (targetId >= 0)
			combat.timer = 0;

		unitCombatArray[i] = combat;
		unitAttackArray[i] = targetId;
	}

	private bool CheckTarget(in UnitBaseData unit, float attackRange, int j, ref float distMin)
	{
		var unit2 = unitBaseArray[j];
		if (unit2.teamId <= 0 || unit2.teamId == unit.teamId)
			return false;

		if (unitHealthArray[j] <= 0)
			return false;

		var v = unit.position - unit2.position;
		var len = math.length(v);
		if (len < distMin || len < unit.radius + unit2.radius + attackRange)
		{
			distMin = len;
			return true;
		}

		return false;
	}
}
