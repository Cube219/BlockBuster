using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace LobbyScene.UI {

	public class RankPanel:MonoBehaviour {

		public Text rankText;
		public Text nameText;
		public Text scoreText;

		// 설정
		public void SetRankPanel(int rank, string name, int score, bool isShowNameColor = false)
		{
			rankText.text = rank.ToString();
			nameText.text = name;
			scoreText.text = score.ToString("N0");

			if(isShowNameColor == true)
				nameText.color = Color.blue;
			else
				nameText.color = new Color(0.6f, 0.6f, 0.6f);
		}
	}
}