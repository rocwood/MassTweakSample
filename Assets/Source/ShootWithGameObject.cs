using UnityEngine;

namespace MassTweakSample
{
	class ShootWithGameObject : Shooter.IShoot
	{
		public static int bulletCount = 0;

		private Shooter _shooter;

		public ShootWithGameObject(Shooter shooter) => _shooter = shooter;

		public void Fire(in Bullet.BulletData bulletData)
		{
			if (bulletCount >= _shooter.bulletMaxCount)
				return;

			bulletCount++;

			var obj = GameObject.Instantiate(_shooter.bulletPrefab);
			obj.transform.position = bulletData.position;
			obj.transform.rotation = bulletData.rotation;
			obj.SetActive(true);

			obj.AddComponent<Bullet>().data = bulletData;
		}

		public void Update()
		{
		}

		public void Destroy()
		{
		}
	}
}
