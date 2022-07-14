using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace MassTweakSample
{
	class ShootWithGameObjectJobSystem : Shooter.IShoot
	{
		private TransformAccessArray _bulletTransformList;
		private NativeArray<Bullet.BulletData> _bulletDataList;

		private Shooter _shooter;
		public int _bulletCount = 0;

		public ShootWithGameObjectJobSystem(Shooter shooter)
		{
			_shooter = shooter;

			_bulletTransformList = new TransformAccessArray(shooter.bulletMaxCount);
			_bulletDataList = new NativeArray<Bullet.BulletData>(shooter.bulletMaxCount, Allocator.Persistent);
		}

		public void Fire(in Bullet.BulletData bulletData)
		{
			if (_bulletCount >= _shooter.bulletMaxCount)
				return;

			var obj = GameObject.Instantiate(_shooter.bulletPrefab);
			var transform = obj.transform;
			transform.position = bulletData.position;
			transform.rotation = bulletData.rotation;
			obj.SetActive(true);

			_bulletTransformList.Add(transform);
			_bulletDataList[_bulletCount] = bulletData;
			_bulletCount++;
		}

		public void Update()
		{
			if (_bulletCount <= 0)
				return;

			var job = new UpdateBulletJob();
			job.dt = Time.deltaTime;
			job.bullets = _bulletDataList;

			var handle = job.Schedule(_bulletTransformList);
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
					var transform = _bulletTransformList[i];
					GameObject.Destroy(transform.gameObject);

					_bulletCount--;
					_bulletDataList[i] = _bulletDataList[_bulletCount];
					_bulletTransformList.RemoveAtSwapBack(i);
				}
			}
		}

		public void Destroy()
		{
			_bulletTransformList.Dispose();
			_bulletDataList.Dispose();
		}

		[BurstCompile]
		struct UpdateBulletJob : IJobParallelForTransform
		{
			public NativeArray<Bullet.BulletData> bullets;
			public float dt;

			public void Execute(int index, TransformAccess transform)
			{
				var data = bullets[index];
				data.position += data.dir * data.speed * dt;
				data.lifeTime -= dt;
				bullets[index] = data;

				transform.position = data.position;
			}
		}
	}
}
