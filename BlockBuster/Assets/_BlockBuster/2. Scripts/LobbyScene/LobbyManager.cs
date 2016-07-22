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

			settingWindow.emailSignUpPanel_errorText.text = "";
		}

		void Start()
		{
			// 로그인 돼 있지 않은가?
			if(UserManager.userState == UserManager.State.NotLogin) {
				Debug.Log("Not Login");
				// 설정 창 패널을 LoginPanel로 바꿔줌
				settingWindow.ShowPanelImmediately(SettingWindow.Panel.Login);

				// 설정 창 띄움
				ShowSettingWindow();
			} else { // 로그인 되어 있음
					 // 설정 창 패널을 AccountPanel로 바꿔줌
				settingWindow.ShowPanelImmediately(SettingWindow.Panel.Account);
				settingWindow.accountPanel_nameText.text = UserManager.name;
			}
		}

		void Update()
		{
			// 뒤로 가기 버튼 누르면 게임 종료
			if(Input.GetKeyDown(KeyCode.Escape)) {
				// 로그아웃 처리
				if(UserManager.userState == UserManager.State.Login)
					ServerManager.m.Post_Logout_f(UserManager.uid, UserManager.sid);
				Application.Quit();
			}
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