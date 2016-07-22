using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LobbyScene.UI.Windows {

	public class RankingWindow:MonoBehaviour {

		public FocusOutBackground focusOutBackground;
		public RankList rankList;

		// 보여줌
		public void Show()
		{
			UpdateRankingInfo();

			this.GetComponent<Animator>().SetTrigger("ShowTrigger");
			focusOutBackground.Show();
		}

		// 숨김
		public void Hide()
		{
			this.GetComponent<Animator>().SetTrigger("HideTrigger");
			focusOutBackground.Hide();
		}

		// 랭킹 정보 불러옴
		public void UpdateRankingInfo()
		{
			ServerManager.m.Get_GetRanksResult += GetRanksResult;
			ServerManager.m.Get_GetRanks_f(0, 999);
		}
		private void GetRanksResult(List<Dictionary<string, string>> data)
		{
			ServerManager.m.Get_GetRanksResult -= GetRanksResult;
			rankList.SetRanks(data, UserManager.uid);
		}
	}
}