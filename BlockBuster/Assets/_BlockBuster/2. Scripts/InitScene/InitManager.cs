using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Facebook.Unity;

namespace InitScene {

	public class InitManager:MonoBehaviour {

		void Awake()
		{
			// 페북 SDK 초기화
			FB.Init(CompleteInitFB);
		}

		// 페북 SDK 초기화 완료
		private void CompleteInitFB()
		{
			FB.ActivateApp();
			// 계정 정보를 불러옴
			DataManager.LoadAccountData();

			// 로그인 시도
			switch(DataManager.accountType) {
				case "Email":
					// 서버로 로그인 보냄
					ServerManager.m.Post_LoginResult += LoginComplete;
					ServerManager.m.Post_LoginWithEmail_f(DataManager.email, DataManager.password);
					break;

				case "Facebook":
					// 페북으로 로그인
					FB.LogInWithReadPermissions(new List<string> { "public_profile" }, FBLoginCallback);
					break;

				default:
					// 다음 Scene으로 
					SceneManager.LoadScene("LobbyScene");
					break;
			}
		}

		// 페북으로 로그인 콜백
		private void FBLoginCallback(ILoginResult result)
		{
			if(FB.IsLoggedIn) { // 로그인 됨
				Debug.Log("SuccessFul Login");
				// 페북 userid 저장
				DataManager.fbUserid = AccessToken.CurrentAccessToken.UserId;

				// 서버로 로그인 시도
				ServerManager.m.Post_LoginResult += LoginComplete;
				ServerManager.m.Post_LoginWithFacebook_f(DataManager.fbUserid);
			} else {
				Debug.Log("Login Denied");
				// 다음 Scene으로 
				SceneManager.LoadScene("LobbyScene");
			}
		}

		public void LoginComplete(Dictionary<string, string> data)
		{
			ServerManager.m.Post_LoginResult -= LoginComplete;

			if(data["result"] == "False") { // 로그인 실패
			} else { // 로그인 성공
				// 받은 유저 정보들 UserManager에 넣어줌
				UserManager.userState = UserManager.State.Login;
				UserManager.uid = data["uid"];
				UserManager.sid = data["sid"];
				UserManager.name = data["name"];
				UserManager.accountType = data["accountType"];
				UserManager.userType = data["userType"];
			}

			// 다음 Scene으로 
			SceneManager.LoadScene("LobbyScene");
		}
	}
}