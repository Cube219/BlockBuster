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
			// 로그인 상태인가?
			if(UserManager.userState == UserManager.State.Login) {
				// 서버로 전송
				ServerManager.m.Get_GetRanksResult += GetRanksResult;
				ServerManager.m.Get_GetRanks_f(0, 999);
			} else { // 아님
			}
		}
		private void GetRanksResult(List<Dictionary<string, string>> data, bool isSuccess, string error)
		{
			ServerManager.m.Get_GetRanksResult -= GetRanksResult;

			if(isSuccess == true) { // 전송 성공
				rankList.SetRanks(data, UserManager.uid);
			} else { // 전송 실패
				Debug.Log("Fail to transfer to server!");
				Debug.Log(error);
				// 오프라인 모드로 변경
				LobbyManager.m.SetOffline();
			}
		}
	}
}