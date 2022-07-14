using UnityEngine;

namespace MassTweakSample
{
	class ShootWithGameObjectManager : Shooter.IShoot
	{
		private Transform[] _bulletObjList;
		private Bullet.BulletData[] _bulletDataList;
		//private NativeArray<Bullet.BulletData> _bulletDataList;

		private Shooter _shooter;
		public int _bulletCount = 0;

		public ShootWithGameObjectManager(Shooter shooter)
		{
			_shooter = shooter;

			_bulletObjList = new Transform[_shooter.bulletMaxCount];
			_bulletDataList = new Bullet.BulletData[_shooter.bulletMaxCount];
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

			_bulletObjList[_bulletCount] = transform;
			_bulletDataList[_bulletCount] = bulletData;
			_bulletCount++;
		}

		public void Update()
		{
			float dt = Time.deltaTime;

			for (int i = 0; i < _bulletCount; )
			{
				var transform = _bulletObjList[i];

				ref var data = ref _bulletDataList[i];
				data.position += data.dir * data.speed * dt;
				data.lifeTime -= dt;

				if (data.lifeTime > 0)
				{
					transform.position = data.position;
					i++;
				}
				else
				{
					GameObject.Destroy(transform.gameObject);

					_bulletCount--;

					_bulletDataList[i] = _bulletDataList[_bulletCount];
					_bulletObjList[i] = _bulletObjList[_bulletCount];
					_bulletObjList[_bulletCount] = null;
				}
			}
		}

		public void Destroy()
		{
			for (int i = 0; i < _bulletCount; i++)
				GameObject.Destroy(_bulletObjList[i].gameObject);

			_bulletObjList = null;
			_bulletDataList = null;
		}
	}
}
