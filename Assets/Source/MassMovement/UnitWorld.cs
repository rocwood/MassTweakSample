using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class UnitWorld
{
	public int unitCount { get; private set; }
	public int maxUnitCount { get; private set; }

	public GameObject[] unitList;
	public NativeArray<UnitBaseData> unitBaseArray;
	public NativeArray<UnitCombatData> unitCombatArray;
	public NativeArray<float> unitHealthArray;

	public NativeArray<float3> unitMoveArray;	// 当前帧的运动方向计算结果
	public NativeArray<int> unitAttackArray;	// 当前帧攻击目标的计算结果
	public TransformAccessArray unitTransformArray;

	public UnitWorld(int maxUnitCount)
	{
		this.maxUnitCount = maxUnitCount;

		unitList = new GameObject[maxUnitCount];
		unitBaseArray = new NativeArray<UnitBaseData>(maxUnitCount, Allocator.Persistent);
		unitCombatArray = new NativeArray<UnitCombatData>(maxUnitCount, Allocator.Persistent);
		unitHealthArray = new NativeArray<float>(maxUnitCount, Allocator.Persistent);
		unitMoveArray = new NativeArray<float3>(maxUnitCount, Allocator.Persistent);
		unitAttackArray = new NativeArray<int>(maxUnitCount, Allocator.Persistent);
		unitTransformArray = new TransformAccessArray(maxUnitCount);
	}

	public void Dispose()
	{
		unitList = null;
		unitBaseArray.Dispose();
		unitCombatArray.Dispose();
		unitHealthArray.Dispose();
		unitMoveArray.Dispose();
		unitAttackArray.Dispose();
		unitTransformArray.Dispose();
	}

	public bool IsFull() => unitCount >= maxUnitCount;

	public int AddUnit(in UnitBaseData baseData, in UnitCombatData combatData, GameObject dispObj)
	{
		if (dispObj == null)
			return -1;

		if (IsFull())
			return -1;

		var index = unitCount;
		unitCount++;

		unitList[index] = dispObj;
		unitBaseArray[index] = baseData;
		unitCombatArray[index] = combatData;
		unitMoveArray[index] = default;
		unitAttackArray[index] = -1;
		unitHealthArray[index] = combatData.hpMax;

		unitTransformArray.Add(dispObj.transform);
	
		return index;
	}

	public void RemoveUnit(int index)
	{
		if (index >= unitCount || index >= maxUnitCount)
			return;

		// 不减少unitCount，也不移动位置，只标记为无效
		unitBaseArray[index] = default;

		GameObject.Destroy(unitList[index]);
		unitList[index] = null;
	}
}
