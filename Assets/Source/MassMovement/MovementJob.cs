using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;

[BurstCompile]
struct MovementJob : IJobParallelForTransform
{
	[ReadOnly] public NativeArray<float3> unitMoveArray;

	public NativeArray<UnitBaseData> unitBaseArray;

	public float dt;

	public void Execute(int i, TransformAccess transform)
	{
		var unit = unitBaseArray[i];
		if (unit.teamId <= 0)
			return;

		// 根据速度向量和时间计算最新位置
		var dir = unitMoveArray[i];
		unit.position += dir * unit.speed * dt;

		// 保存最新位置
		unitBaseArray[i] = unit;

		transform.position = unit.position;
	}
}
