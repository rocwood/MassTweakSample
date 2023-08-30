using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

public partial class MassMovementManager
{
	[BurstCompile]
	struct SteeringBehaviourJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeArray<UnitData> unitDataArray;
		public NativeArray<float3> unitMoveArray;

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

			// 计算单位之间的挤压
			int count = unitDataArray.Length;
			for (int j = 0; j < count; j++)
			{
				if (i == j)
					continue;

				var other = unitDataArray[j];
				if (other.teamId <= 0)
					continue;

				// TODO

			}

			// 保存速度向量
			unitMoveArray[i] = dir;
		}
	}
}
