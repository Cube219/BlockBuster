using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LobbyScene.UI.Windows;
using UnityEngine.SceneManagement;

namespace LobbyScene {

	public class LobbyManager:MonoBehaviour {

		public static LobbyManager m;

		public SettingWindow settingWindow;
		public RankingWindow rankingWindow;
		public FocusOutBackground hideScreenBackground;

		void Awake()
		{
			m = this;

			// 맨 처음에는 화면을 즉시 가림
			hideScreenBackground.ShowImmediately();
			// 화면을 보여줌
			hideScreenBackground.Hide();

			settingWindow.emailSignUpPanel_errorText.text = "";
		}

		void Start()
		{
			// 로그인 돼 있지 않은가? (Offline 모드)
			if(UserManager.userState == UserManager.State.NotLogin) {
				Debug.Log("Not Login");
				SetOffline();

				// 마지막으로 전송한 것이 성공했는가(서버 오류가 아닌 경우. 즉, 처음 접속한 경우)
				if(ServerManager.m.isLastTransferFailed == false) {
					// 설정 창 띄움
					ShowSettingWindow();
				}
			} else { // 로그인 되어 있음
				DestroyOffline();
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
				if(UserManager.userState == UserManager.State.Login) {
					ServerManager.m.Post_LogoutResult += LogoutResult;
					ServerManager.m.Post_Logout_f(UserManager.uid, UserManager.sid);
				}
				Application.Quit();
			}
		}
		private void LogoutResult(Dictionary<string, string> data, bool isSuccess, string error)
		{
			// 아무 짓도 안 함...
		}

		// 오프라인 모드
		public void SetOffline()
		{
			// 로그인 상태 해제
			UserManager.userState = UserManager.State.NotLogin;
			// OFFLINE Text 보여줌
			GameObject.FindGameObjectWithTag("OfflineText").GetComponent<CanvasGroup>().alpha = 1f;
			// 설정 창 패널을 LoginPanel로 바꿔줌
			settingWindow.ShowPanelImmediately(SettingWindow.Panel.Login);
		}

		// 오프라인 모드 해제
		public void DestroyOffline()
		{
			// OFFLINE Text 지워줌
			GameObject.FindGameObjectWithTag("OfflineText").GetComponent<CanvasGroup>().alpha = 0f;
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