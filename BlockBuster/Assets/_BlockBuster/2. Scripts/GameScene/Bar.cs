using UnityEngine;
using System.Collections;

namespace GameScene {

	public class Bar:MonoBehaviour {

		private bool isPaused = false;

		// 일시정지
		public void Pause()
		{
			if(isPaused == false)
				isPaused = true;
		}

		// 재게
		public void Resume()
		{
			if(isPaused == true)
				isPaused = false;
		}

		// Update is called once per frame
		void Update()
		{
			if(isPaused == false) {
				if(Input.touchCount > 0) {
					Touch touch = Input.GetTouch(0);
					Vector2 t = this.transform.position;
					t.x = Camera.main.ScreenToWorldPoint(touch.position).x;
					this.transform.position = t;
				}

#if UNITY_EDITOR
				Vector2 mousePosition = Input.mousePosition;
				this.transform.position = new Vector2(Camera.main.ScreenToWorldPoint(mousePosition).x, this.transform.position.y);
#endif
			}
		}

		// 공과의 충돌
		void OnCollisionEnter2D(Collision2D coll)
		{
			foreach(ContactPoint2D contact in coll.contacts) {
				if(contact.collider.gameObject.tag == "Ball") {
					float speed = contact.collider.gameObject.GetComponent<Ball>().currentSpeed;
					float barWidth = this.GetComponent<Renderer>().bounds.size.x;
					float percentage = (contact.point.x - transform.position.x) / barWidth / 2;
					// 부착 모드인가?
					/*if(state == State.Attached) {
						AttachedInfo info = new AttachedInfo();
						info.velocity = contact.collider.GetComponent<Rigidbody2D>().velocity;
						contact.collider.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
						// 딱 닿았을 때의 좌표값과 퍼센트치를 저장해줌
						info.ball = contact.collider.gameObject;
						info.ballPosition = contact.collider.transform.position;
						info.barPosition = tf.position;
						info.percentage = percentage;
						attachedBalls.Add(info);
					} else {*/
						contact.collider.GetComponent<Rigidbody2D>().AddForce(new Vector2(percentage * 5 * (speed * 50), 0));
						if(contact.collider.GetComponent<Rigidbody2D>().velocity.y < 0)
							contact.collider.GetComponent<Rigidbody2D>().velocity *= -1;
					//}
					break;
				}
			}
		}
	}
}