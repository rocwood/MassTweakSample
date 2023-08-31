using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
struct SteeringBehaviourJob : IJobParallelFor
{
	[ReadOnly]
	public NativeArray<UnitBaseData> unitDataArray;
	public NativeArray<float3> unitMoveArray;

	public float spacing;

	public void Execute(int i)
	{
		var unit = unitDataArray[i];
		if (unit.teamId <= 0)
			return;

		// 朝向目标点移动
		var dir = unit.targetPos - unit.position;
		var len = math.length(dir);
		if (len > 0)
			dir /= len;

		// 计算单位之间的挤压分离
		var separation = float3.zero;
		int neighbours = 0;

		// 遍历单位列表
		// TODO: 可用四叉树或网格优化
		int count = unitDataArray.Length;
		for (int j = 0; j < count; j++)
		{
			if (i == j)
				continue;

			var other = unitDataArray[j];
			if (other.teamId <= 0)
				continue;

			var xdir = unit.position - other.position;
			var xlen = math.length(xdir);
			if (xlen > 0 && xlen < unit.radius + other.radius + spacing)
			{
				separation += xdir / xlen;
				neighbours++;
			}
		}

		if (neighbours > 0)
			separation /= neighbours;

		// 保存合速度向量
		unitMoveArray[i] = dir + separation;
	}
}
