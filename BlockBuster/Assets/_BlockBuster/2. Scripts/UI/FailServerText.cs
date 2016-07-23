using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FailServerText : MonoBehaviour {

	void Awake()
	{
		this.GetComponent<CanvasGroup>().alpha = 0;
	}

	public void Retry(int count, int maxCount)
	{
		this.GetComponent<CanvasGroup>().alpha = 1;
		this.GetComponent<Text>().text = "Fail to transfer to server!\nRetrying... (" + count + "/" + maxCount + ")";
	}

	public void FailRetry()
	{
		this.GetComponent<CanvasGroup>().alpha = 1;
		this.GetComponent<Text>().text = "Fail to transfer to server!\nChange to OFFLINE automatically...";
		StartCoroutine(DelayInvisible());
	}
	private IEnumerator DelayInvisible()
	{
		yield return new WaitForSeconds(7f);
		// 서서히 사라짐
		for(float a = 1f; a >= 0f; a -= 0.08f) {
			this.GetComponent<CanvasGroup>().alpha = a;
			yield return null;
		}
		this.GetComponent<CanvasGroup>().alpha = 0f;
	}

	public void SuccessRetry()
	{
		this.GetComponent<CanvasGroup>().alpha = 0;
	}
}
