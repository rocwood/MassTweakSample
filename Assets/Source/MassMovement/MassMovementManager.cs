using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public partial class MassMovementManager : MonoBehaviour
{
	public List<TeamConfig> teamList = new List<TeamConfig>();

	public int maxUnitCount = 10000;
	public int unitCount;
	public int jobBatchCount = 1;
	public float spacing = 0.1f;

	private GameObject[] unitList;
	private NativeArray<UnitBaseData> unitDataArray;
	private NativeArray<float3> unitMoveArray;
	private TransformAccessArray unitTransformArray;

	void Start()
	{
		unitList = new GameObject[maxUnitCount];
		unitDataArray = new NativeArray<UnitBaseData>(maxUnitCount, Allocator.Persistent);
		unitMoveArray = new NativeArray<float3>(maxUnitCount, Allocator.Persistent);
		unitTransformArray = new TransformAccessArray(maxUnitCount);

		// 创建队伍方阵
		for (int i = 0; i < teamList.Count; i++)
			StartCoroutine(CreateTeam(teamList[i]));
	}

	IEnumerator CreateTeam(TeamConfig team)
	{
		for (int i = 0; i < team.totalCount; i++)
		{
			if (unitCount >= maxUnitCount)
				yield break;

			// 随机方阵位置
			var pos = team.baseData.position;
			if (team.startPosRandomRange > 0)
			{
				pos.x += UnityEngine.Random.Range(-team.startPosRandomRange, team.startPosRandomRange);
				pos.z += UnityEngine.Random.Range(-team.startPosRandomRange, team.startPosRandomRange);
			}

			// 创建单位
			var unit = Instantiate(team.prefab, pos, team.prefab.transform.rotation, this.transform);
			unit.SetActive(true);

			var baseData = team.baseData;
			baseData.position = pos;
			baseData.targetPos = team.baseData.targetPos;

			// 加入单位列表
			unitList[unitCount] = unit;
			unitDataArray[unitCount] = baseData;
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
