using UnityEngine;
using System.Collections;

namespace LobbyScene.UI.Windows {

	public class RankingWindow:MonoBehaviour {

		public FocusOutBackground focusOutBackground;

		// 보여줌
		public void Show()
		{
			this.GetComponent<Animator>().SetTrigger("ShowTrigger");
			focusOutBackground.Show();
		}

		// 숨김
		public void Hide()
		{
			this.GetComponent<Animator>().SetTrigger("HideTrigger");
			focusOutBackground.Hide();
		}
	}
}