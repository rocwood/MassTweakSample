using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
struct SteeringJob : IJobParallelFor
{
	[ReadOnly]
	public NativeArray<UnitBaseData> unitBaseArray;
	public NativeArray<float3> unitMoveArray;

	public float spacing;

	public void Execute(int i)
	{
		var unit = unitBaseArray[i];
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
		int count = unitBaseArray.Length;
		for (int j = 0; j < count; j++)
		{
			if (i == j)
				continue;

			var other = unitBaseArray[j];
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
		{
			// 计算合速度向量，限制最大值
			dir += separation / neighbours;
			len = math.length(dir);
			if (len > 1)
				dir /= len;
		}

		unitMoveArray[i] = dir;
	}
}
