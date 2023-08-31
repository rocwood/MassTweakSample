using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class MassMovementController : MonoBehaviour
{
	public List<TeamConfig> teamList = new List<TeamConfig>();

	public int maxUnitCount = 10000;
	public int jobBatchCount = 1;
	public float spacing = 0.1f;

	private UnitWorld world;

	void Start()
	{
		world = new UnitWorld(maxUnitCount);

		for (int i = 0; i < teamList.Count; i++)
			StartCoroutine(CreateTeam(world, teamList[i]));
	}

	IEnumerator CreateTeam(UnitWorld world, TeamConfig team)
	{
		for (int i = 0; i < team.totalCount; i++)
		{
			if (world.IsFull())
				yield break;

			// 随机方阵位置
			var pos = team.baseData.position;
			if (team.startPosRandomRange > 0)
			{
				pos.x += Random.Range(-team.startPosRandomRange, team.startPosRandomRange);
				pos.z += Random.Range(-team.startPosRandomRange, team.startPosRandomRange);
			}

			// 创建单位
			var dispObj = Instantiate(team.prefab, pos, team.prefab.transform.rotation, this.transform);
			dispObj.SetActive(true);

			var unit = team.baseData;
			unit.position = pos;
			unit.targetPos = team.baseData.targetPos;

			var combat = team.combatData;
			combat.hp = combat.hpMax;
			combat.target = -1;

			// 加入单位列表
			world.AddUnit(unit, combat, dispObj);

			// 分帧,等下一帧继续创建
			if (team.createPerFrame > 0 &&
				(i + 1) % team.createPerFrame == 0)
				yield return null;
		}
	}

	void OnDestroy()
	{
		world.Dispose();
		world = null;
	}

	void Update()
	{
		if (world == null)
			return;

		// 计算速度向量
		var job1 = new SteeringJob {
			unitBaseArray = world.unitBaseDataArray,
			unitMoveArray = world.unitMoveArray,
			spacing = spacing,
		};

		var handle1 = job1.Schedule(world.unitCount, jobBatchCount);

		// 执行位移计算
		var job2 = new MovementJob {
			unitBaseArray = world.unitBaseDataArray,
			unitMoveArray = world.unitMoveArray,
			dt = Time.deltaTime,
		};

		var handle2 = job2.Schedule(world.unitTransformArray, handle1); // 依赖job1
		handle2.Complete();
	}
}
