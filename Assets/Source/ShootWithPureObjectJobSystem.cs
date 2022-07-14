using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace MassTweakSample
{
	class ShootWithPureObjectJobSystem : Shooter.IShoot
	{
		private NativeArray<Bullet.BulletData> _bulletDataList;
		private NativeArray<Matrix4x4> _bulletMatrixList;

		private Shooter _shooter;
		public int _bulletCount = 0;

		private Mesh _bulletMesh;
		private Material _bulletMaterial;

		public ShootWithPureObjectJobSystem(Shooter shooter)
		{
			_shooter = shooter;

			_bulletMesh = shooter.bulletPrefab.GetComponent<MeshFilter>().mesh;
			_bulletMaterial = shooter.bulletPrefab.GetComponent<MeshRenderer>().material;

			_bulletDataList = new NativeArray<Bullet.BulletData>(shooter.bulletMaxCount, Allocator.Persistent);
			_bulletMatrixList = new NativeArray<Matrix4x4>(shooter.bulletMaxCount, Allocator.Persistent);
		}

		public void Fire(in Bullet.BulletData bulletData)
		{
			if (_bulletCount >= _shooter.bulletMaxCount)
				return;

			_bulletDataList[_bulletCount] = bulletData;
			_bulletCount++;
		}

		public void Update()
		{
			if (_bulletCount <= 0)
				return;

			var job = new UpdateBulletJob();
			job.dt = Time.deltaTime;
			job.dataList = _bulletDataList;
			job.matrixList = _bulletMatrixList;

			var handle = job.Schedule(_bulletCount, _shooter.JobBatchCount);
			handle.Complete();

			for (int i = 0; i < _bulletCount; )
			{
				var data = _bulletDataList[i];
				if (data.lifeTime > 0)
				{
					i++;
				}
				else
				{
					_bulletCount--;
					_bulletDataList[i] = _bulletDataList[_bulletCount];
				}
			}

			DoRender();
		}

		private void DoRender()
		{
			for (int i = 0; i < _bulletCount; ++i)
			{
				var matrix = _bulletMatrixList[i];
				Graphics.DrawMesh(_bulletMesh, matrix, _bulletMaterial, 0, null, 0, null, false, false, false);
			}
		}

		public void Destroy()
		{
			_bulletDataList.Dispose();
			_bulletMatrixList.Dispose();
		}

		[BurstCompile]
		struct UpdateBulletJob : IJobParallelFor
		{
			public float dt;
			public NativeArray<Bullet.BulletData> dataList;

			[WriteOnly]
			public NativeArray<Matrix4x4> matrixList;

			public void Execute(int index)
			{
				var data = dataList[index];
				data.position += data.dir * data.speed * dt;
				data.lifeTime -= dt;
				dataList[index] = data;

				matrixList[index] = Matrix4x4.TRS(data.position, data.rotation, Vector3.one);
			}
		}
	}
}
