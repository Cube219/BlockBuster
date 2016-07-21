using UnityEngine;
using System.Collections;

namespace GameScene {

	public class Ball:MonoBehaviour {

		public float speed = 14f;
		public float currentSpeed = 14f;

		private bool isPaused = false;
		private Vector2 pausedVelocity;

		// 공 시작
		public void BallStart()
		{
			this.GetComponent<Rigidbody2D>().AddForce(new Vector2(-(currentSpeed * 50), -(currentSpeed * 50)));
		}

		// 공 일시정지
		public void Pause()
		{
			if(isPaused == false) {
				isPaused = true;
				pausedVelocity = GetComponent<Rigidbody2D>().velocity;
				GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			}
		}

		// 공 재게
		public void Resume()
		{
			if(isPaused == true) {
				isPaused = false;
				GetComponent<Rigidbody2D>().velocity = pausedVelocity;
			}
		}

		void FixedUpdate()
		{
			
			if(isPaused == false) {

				if(speed + 0.001f <= 20f)
					speed += 0.001f;

				Vector2 direction = GetComponent<Rigidbody2D>().velocity.normalized;
				// 너무 가로로 왔다갔다 하지 않게 해줌
				if(direction.x >= 0.8f) {
					direction.x = 0.6f;
				}
				GetComponent<Rigidbody2D>().velocity = direction * speed;
			}
		}

		// 공 충돌
		void OnCollisionEnter2D(Collision2D coll)
		{
			//AudioSource.PlayClipAtPoint(tockSound, Vector2.zero);
			// 죽음 벽에 부딫치면 죽음
			if(coll.gameObject.tag == "DeathWall") {
				GameManager.m.BallDied(this);
				Destroy(this.gameObject);
			}
		}
	}
}