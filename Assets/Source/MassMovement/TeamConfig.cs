using System;
using UnityEngine;

[Serializable]
public struct TeamConfig
{
	public int teamId;

	public GameObject prefab;
	public float speed;
	public float radius;

	public int count;
	public int createPerFrame;

	public Vector3 startPos;
	public float startRange;

	public Vector3 targetPos;
}
