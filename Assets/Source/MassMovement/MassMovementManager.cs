using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public partial class MassMovementManager : MonoBehaviour
{
	public TeamConfig redTeam;
	public TeamConfig blueTeam;

	public int maxUnitCount = 10000;
	public int unitCount;
	public int jobBatchCount = 1;
	public float spacing = 0.1f;

	private GameObject[] unitList;
	private NativeArray<UnitData> unitDataArray;
	private NativeArray<float3> unitMoveArray;
	private TransformAccessArray unitTransformArray;

	IEnumerator Start()
	{
		maxUnitCount = redTeam.count + blueTeam.count;

		unitList = new GameObject[maxUnitCount];
		unitDataArray = new NativeArray<UnitData>(maxUnitCount, Allocator.Persistent);
		unitMoveArray = new NativeArray<float3>(maxUnitCount, Allocator.Persistent);
		unitTransformArray = new TransformAccessArray(maxUnitCount);

		// 创建两个队伍方阵
		var c1 = StartCoroutine(CreateTeam(redTeam));
		var c2 = StartCoroutine(CreateTeam(blueTeam));

		yield return c1;
		yield return c2;
	}

	IEnumerator CreateTeam(TeamConfig team)
	{
		for (int i = 0; i < team.count; i++)
		{
			if (unitCount >= maxUnitCount)
				yield break;

			// 随机方阵位置
			var pos = team.startPos;
			if (team.startRange > 0)
			{
				pos.x += UnityEngine.Random.Range(-team.startRange, team.startRange);
				pos.z += UnityEngine.Random.Range(-team.startRange, team.startRange);
			}

			// 创建单位
			var unit = Instantiate(team.prefab, pos, Quaternion.identity, this.transform);
			unit.SetActive(true);

			var unitData = new UnitData { position = pos,
				teamId = team.teamId, targetPos = team.targetPos, speed = team.speed, radius = team.radius,
			};

			// 加入单位列表
			unitList[unitCount] = unit;
			unitDataArray[unitCount] = unitData;
			unitTransformArray.Add(unit.transform);
			unitCount++;

			// 分帧,等下一帧继续创建
			if (team.createPerFrame > 0 &&
				(i + 1) % team.createPerFrame == 0)
				yield return null;
		}
	}

	void OnDestroy()
	{
		unitDataArray.Dispose();
		unitMoveArray.Dispose();
		unitTransformArray.Dispose();
		unitList = null;
	}

	void Update()
	{
		// 计算速度向量
		var job1 = new SteeringBehaviourJob {
			unitDataArray = unitDataArray,
			unitMoveArray = unitMoveArray,
			spacing = spacing,
		};

		var handle1 = job1.Schedule(unitCount, jobBatchCount);

		// 执行位移计算
		var job2 = new MovementJob {
			unitDataArray = unitDataArray,
			unitMoveArray = unitMoveArray,
			dt = Time.deltaTime,
		};

		var handle2 = job2.Schedule(unitTransformArray, handle1); // 依赖job1
		handle2.Complete();
	}
}
