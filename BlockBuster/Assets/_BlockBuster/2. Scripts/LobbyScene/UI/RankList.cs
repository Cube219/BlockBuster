using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LobbyScene.UI {

	public class RankList:MonoBehaviour {

		public RankPanel[] rankPanel_up;
		public RankPanel rankPanel;
		public RankPanel[] rankPanel_down;

		// 랭킹 셋팅
		public void SetRanks(List<Dictionary<string, string>> data, string myUid)
		{
			// 안보이게 함
			foreach(RankPanel p in rankPanel_up) {
				p.gameObject.SetActive(false);
			}
			foreach(RankPanel p in rankPanel_down) {
				p.gameObject.SetActive(false);
			}

			// 내 랭킹을 찾음
			int i;
			for(i=0; i<data.Count; i++) {
				if(data[i]["uid"].Equals(myUid))
					break;
			}

			// 내 랭킹 채움
			rankPanel.SetRankPanel(int.Parse(data[i]["rank"]), data[i]["name"], int.Parse(data[i]["score"]), true);

			// 내 위쪽 랭킹 채움
			for(int up = 1; up <= 3; up++) {
				if(i - up < 0) break;
				rankPanel_up[up - 1].gameObject.SetActive(true);
				rankPanel_up[up - 1].SetRankPanel(int.Parse(data[i - up]["rank"]), data[i - up]["name"], int.Parse(data[i - up]["score"]));
			}

			// 내 아래쪽 랭킹 채움
			for(int down = 1; down <= 3; down++) {
				if(i + down >= data.Count) break;
				rankPanel_down[down - 1].gameObject.SetActive(true);
				rankPanel_down[down - 1].SetRankPanel(int.Parse(data[i + down]["rank"]), data[i + down]["name"], int.Parse(data[i + down]["score"]));
			}
		}
	}
}