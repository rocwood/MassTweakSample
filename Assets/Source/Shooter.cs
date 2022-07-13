using UnityEngine;

namespace Demo
{
	public class Shooter : MonoBehaviour
	{
		public GameObject bulletPrefab;

		public float bulletRange = 60;
		public int bulletCount = 30;
		public float fireInterval = 0.1f;

		private Plane _plane = new Plane(Vector3.up, 0);
		private Camera _camera;

		private float _fireTimer = 0;

		void Start()
		{
			_camera = Camera.main;
		}

		void Update()
		{
			var ray = _camera.ScreenPointToRay(Input.mousePosition);
			if (_plane.Raycast(ray, out var distance))
			{
				var target = ray.GetPoint(distance);
				var dir = target - transform.position;
				dir.y = 0;
				transform.rotation = Quaternion.LookRotation(dir);
			}

			_fireTimer -= Time.deltaTime;

			if (Input.GetKey(KeyCode.Mouse0) && _fireTimer <= 0)
			{
				_fireTimer = fireInterval;

				var yaw = transform.rotation.eulerAngles;
				
				var bulletAngle = bulletRange / bulletCount;
				yaw.y -= bulletRange / 2;

				for (int i = 0; i < bulletCount; i++, yaw.y += bulletAngle)
					CreateBullet(yaw);
			}
		}

		private void CreateBullet(Vector3 yaw)
		{
			var obj = Instantiate(bulletPrefab);

			obj.transform.position = transform.position;
			obj.transform.rotation = Quaternion.Euler(yaw);

			obj.SetActive(true);
		}
	}
}
