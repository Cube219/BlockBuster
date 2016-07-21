using UnityEngine;
using System.Collections;

namespace GameScene.Blocks {

	public class BreakedBlock:MonoBehaviour {

		public GameObject[] fragments;

		// 색 설정
		public void SetColor(Color color)
		{
			foreach(GameObject t in fragments) {
				t.GetComponent<SpriteRenderer>().color = color;
			}
		}

		// 부셔지는 힘을 줌
		public void AddForce(Vector2 force)
		{
			float horizontalMin = 0f;
			float horizontalMax = 1.5f;
			float verticalMin = 0.5f;
			float verticalMax = 1.8f;
			foreach(GameObject f in fragments) {
				f.GetComponent<Rigidbody2D>().AddForce(new Vector2(force.x * 500 * Random.Range(horizontalMin, horizontalMax), force.y * 500 * Random.Range(verticalMin, verticalMax)));
				f.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-360f, 360f);
			}
			StartCoroutine(Remove(8));
		}

		// 일정 시간이 지나면 지움
		private IEnumerator Remove(float time)
		{
			yield return new WaitForSeconds(time);
			Destroy(this.gameObject);
		}
	}
}