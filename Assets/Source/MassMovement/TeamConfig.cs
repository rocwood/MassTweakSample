using System;
using UnityEngine;

[Serializable]
public struct TeamConfig
{
	public UnitBaseData baseData;
	public UnitCombatData combatData;

	public GameObject prefab;

	public int totalCount;
	public int createPerFrame;			// 每帧至多创建多少个单位
	public float startPosRandomRange;	// 初始随机范围
}
