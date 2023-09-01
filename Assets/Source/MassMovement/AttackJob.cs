using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
struct AttackJob : IJobParallelFor
{
	[ReadOnly] public NativeArray<UnitBaseData> unitBaseArray;
	[ReadOnly] public NativeArray<UnitCombatData> unitCombatArray;
	[ReadOnly] public NativeArray<int> unitAttackArray;

	public NativeArray<float> unitHealthArray;

	public int unitCount;

	public void Execute(int i)
	{
		var unit = unitBaseArray[i];
		if (unit.teamId <= 0)
			return;

		var hp = unitHealthArray[i];

		for (int j = 0; j < unitCount; j++)
		{
			if (i == j)
				continue;

			// 是对我进行攻击
			if (unitAttackArray[j] == i)
			{
				var combat2 = unitCombatArray[j];
				hp -= combat2.attack;
			}
		}

		unitHealthArray[i] = hp;
	}
}
