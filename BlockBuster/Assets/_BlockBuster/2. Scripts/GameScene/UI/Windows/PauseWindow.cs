using UnityEngine;
using System.Collections;

namespace GameScene.UI.Windows {

	public class PauseWindow:MonoBehaviour {

		public FocusOutBackground focusOutBackground;

		// 보임
		public void Show()
		{
			if(!this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
				this.GetComponent<Animator>().SetTrigger("ShowTrigger");
				focusOutBackground.Show();
			}
		}

		// 숨김
		public void Hide()
		{
			if(!this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Init")) {
				this.GetComponent<Animator>().SetTrigger("HideTrigger");
				focusOutBackground.Hide();
			}
		}
	}
}