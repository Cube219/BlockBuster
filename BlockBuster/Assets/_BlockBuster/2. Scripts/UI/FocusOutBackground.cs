using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class FocusOutBackground:MonoBehaviour {

	// 보여줌
	public void Show()
	{
		// 뒷부분 마우스 클릭을 막음
		this.GetComponent<Image>().raycastTarget = true;
		// 애니메이션 재생
		this.GetComponent<Animator>().SetTrigger("ShowTrigger");
	}

	// 숨김
	public void Hide()
	{
		// 뒷부분 마우스 클릭을 품
		this.GetComponent<Image>().raycastTarget = false;
		// 애니메이션 재생
		this.GetComponent<Animator>().SetTrigger("HideTrigger");
	}

	// 즉시 보여줌
	public void ShowImmediately()
	{
		this.GetComponent<Image>().raycastTarget = true;
		this.GetComponent<Animator>().Play("Idle");
	}

	// 즉시 숨겨줌
	public void HideImmediately()
	{
		this.GetComponent<Image>().raycastTarget = false;
		this.GetComponent<Animator>().Play("Init");
	}

}