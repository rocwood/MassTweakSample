using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;

[BurstCompile]
struct MovementJob : IJobParallelForTransform
{
	public NativeArray<UnitBaseData> unitDataArray;

	[ReadOnly]
	public NativeArray<float3> unitMoveArray;
	public float dt;

	public void Execute(int i, TransformAccess transform)
	{
		var unit = unitDataArray[i];
		if (unit.teamId <= 0)
			return;

		// 根据速度向量和时间计算最新位置
		var dir = unitMoveArray[i];
		unit.position += dir * unit.speed * dt;

		// 保存最新位置
		unitDataArray[i] = unit;

		transform.position = unit.position;
	}
}
