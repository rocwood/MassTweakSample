using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class UnitWorld
{
	public int unitCount { get; private set; }
	public int maxUnitCount { get; private set; }

	public GameObject[] unitList;
	public NativeArray<UnitBaseData> unitBaseDataArray;
	public NativeArray<UnitCombatData> unitCombatDataArray;
	public NativeArray<float3> unitMoveArray;
	public TransformAccessArray unitTransformArray;

	public UnitWorld(int maxUnitCount)
	{
		this.maxUnitCount = maxUnitCount;

		unitList = new GameObject[maxUnitCount];
		unitBaseDataArray = new NativeArray<UnitBaseData>(maxUnitCount, Allocator.Persistent);
		unitCombatDataArray = new NativeArray<UnitCombatData>(maxUnitCount, Allocator.Persistent);
		unitMoveArray = new NativeArray<float3>(maxUnitCount, Allocator.Persistent);
		unitTransformArray = new TransformAccessArray(maxUnitCount);
	}

	public void Dispose()
	{
		unitList = null;
		unitBaseDataArray.Dispose();
		unitCombatDataArray.Dispose();
		unitMoveArray.Dispose();
		unitTransformArray.Dispose();
	}

	public bool IsFull() => unitCount >= maxUnitCount;

	public int AddUnit(UnitBaseData baseData, UnitCombatData combatData, GameObject dispObj)
	{
		if (dispObj == null)
			return -1;

		if (IsFull())
			return -1;

		var index = unitCount;
		unitCount++;

		unitList[index] = dispObj;
		unitBaseDataArray[index] = baseData;
		unitCombatDataArray[index] = combatData;
		unitTransformArray.Add(dispObj.transform);
	
		return index;
	}

	public void RemoveUnit(int index)
	{
		// TODO
	}
}
