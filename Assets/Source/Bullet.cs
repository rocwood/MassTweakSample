using UnityEngine;

namespace MassTweakSample
{
	public class Bullet : MonoBehaviour
	{
		public struct BulletData
		{
			public float speed;
			public float lifeTime;
			public Vector3 dir;
			public Vector3 position;
			public Quaternion rotation;
		}

		public BulletData data;

		void Update()
		{
			data.position += data.dir * data.speed * Time.deltaTime;
			transform.position = data.position; 

			data.lifeTime -= Time.deltaTime;
			if (data.lifeTime < 0)
			{
				Destroy(gameObject);
				
				ShootWithGameObject.bulletCount--;
			}
		}
	}
}
