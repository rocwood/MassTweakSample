using UnityEngine;

namespace MassTweakSample
{
	public class Shooter : MonoBehaviour
	{
		public enum ShootMode
		{
			GameObjectScript = 0,
			GameObjectManager,
			GameObjectJobSystem,
			PureObjectDrawMesh,
		}
		public ShootMode mode;
		public int JobBatchCount = 16;

		public float fireCD = 0.05f;
		public float fireRangeAngle = 60;
		public int fireBulletCount = 30;
		public float bulletSpeed = 10;
		public float bulletLifeTime = 2;
		public int bulletMaxCount = 10000;
		public GameObject bulletPrefab;

		private Plane _plane = new Plane(Vector3.up, 0);
		private Camera _camera;
		private float _fireTimer = 0;

		public interface IShoot
		{
			void Fire(in Bullet.BulletData bulletData);
			void Update();
			void Destroy();
		}
		private IShoot[] _shootImpls;

		void Start()
		{
			bulletPrefab?.SetActive(false);

			_camera = Camera.main;

			_shootImpls = new IShoot[] {
				new ShootWithGameObject(this),
				new ShootWithGameObjectManager(this),
				new ShootWithGameObjectJobSystem(this),
				null,
			};
		}

		void OnDestroy()
		{
			for (int i = 0; i < _shootImpls.Length; i++)
				_shootImpls[i]?.Destroy();

			_shootImpls = null;
		}

		void Update()
		{
			FaceToMouse();

			var shoot = _shootImpls[(int)mode];
			if (shoot == null)
				return;

			CheckFire(shoot);

			shoot.Update();
		}

		private void FaceToMouse()
		{
			var ray = _camera.ScreenPointToRay(Input.mousePosition);
			if (_plane.Raycast(ray, out var distance))
			{
				var target = ray.GetPoint(distance);
				var dir = target - transform.position;
				dir.y = 0;
				transform.rotation = Quaternion.LookRotation(dir);
			}
		}

		private void CheckFire(IShoot shoot)
		{
			_fireTimer -= Time.deltaTime;

			if (Input.GetKey(KeyCode.Mouse0) && _fireTimer <= 0)
			{
				_fireTimer = fireCD;

				var yaw = transform.rotation.eulerAngles;

				var bulletAngle = fireRangeAngle / fireBulletCount;
				yaw.y -= fireRangeAngle / 2;

				for (int i = 0; i < fireBulletCount; i++, yaw.y += bulletAngle)
				{
					var rotation = Quaternion.Euler(yaw);
					var bulletData = new Bullet.BulletData {
						speed = bulletSpeed,
						lifeTime = bulletLifeTime,
						position = transform.position,
						dir = rotation * Vector3.forward,
					};

					shoot.Fire(bulletData);
				}
			}
		}
	}
}
