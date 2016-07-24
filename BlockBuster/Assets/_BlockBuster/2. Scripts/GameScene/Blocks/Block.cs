using UnityEngine;
using System.Collections;

namespace GameScene.Blocks {

	public class Block:MonoBehaviour {

		public GameObject breakedBlock;
		public GameObject effect;
		public AudioClip breakSound;

		public int hp;

		private Color color;

		// 색 변경
		public void SetColor(Color color)
		{
			this.color = color;
			this.GetComponent<SpriteRenderer>().color = color;
		}

		// 부딫쳐서 HP가 감소
		public void HPDown(int damage, Vector2 force, Vector2 hitPos)
		{
			hp -= damage;
			if(hp <= 0) {

				AudioSource.PlayClipAtPoint(breakSound, Vector2.zero);

				GameManager.m.BreakBlock();

				Destroy(Instantiate(effect, hitPos, Quaternion.identity), 3);
				GameObject b = Instantiate(breakedBlock, transform.position, transform.rotation) as GameObject;

				b.GetComponent<BreakedBlock>().SetColor(color);
				b.GetComponent<BreakedBlock>().AddForce(force);

				Destroy(this.gameObject);
			}
		}

		// 충돌
		IEnumerator OnCollisionEnter2D(Collision2D col)
		{
			foreach(ContactPoint2D contact in col.contacts) {
				if(contact.collider.gameObject.tag == "Ball") {
					yield return null;
					yield return null;

					HPDown(1, col.relativeVelocity.normalized * -1, contact.point);
					break;
				}
			}
		}
	}
}