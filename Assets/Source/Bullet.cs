using UnityEngine;

namespace Demo
{
	public class Bullet : MonoBehaviour
	{
		public float speed = 5;
		public float lifeTime = 3;
	
		void Update()
		{
			transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);

			lifeTime -= Time.deltaTime;
			if (lifeTime < 0)
				Destroy(gameObject);
		}
	}
}
