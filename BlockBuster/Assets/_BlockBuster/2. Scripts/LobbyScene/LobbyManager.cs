using UnityEngine;
using System.Collections;
using LobbyScene.UI.Windows;
using UnityEngine.SceneManagement;

namespace LobbyScene {

	public class LobbyManager:MonoBehaviour {

		public SettingWindow settingWindow;
		public RankingWindow rankingWindow;
		public FocusOutBackground hideScreenBackground;

		void Awake()
		{
			// 맨 처음에는 화면을 즉시 가림
			hideScreenBackground.ShowImmediately();
			// 화면을 보여줌
			hideScreenBackground.Hide();
		}

		// 설정 Window 보여줌
		public void ShowSettingWindow()
		{
			settingWindow.Show();
		}

		// 랭킹 Window 보여줌
		public void ShowRankingWindow()
		{
			rankingWindow.Show();
		}

		// 게임 시작
		public void PlayGame()
		{
			// 화면을 가림
			hideScreenBackground.Show();
			// 잠시 후 시작
			StartCoroutine(PlayeGame_c());
		}
		private IEnumerator PlayeGame_c()
		{
			yield return new WaitForSeconds(0.4f);
			// 게임 Scene 로드
			SceneManager.LoadScene("GameScene");
		}
	}
}