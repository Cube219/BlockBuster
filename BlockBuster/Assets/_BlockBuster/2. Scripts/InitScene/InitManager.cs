using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace InitScene {

	public class InitManager:MonoBehaviour {

		void Start()
		{
			// 계정 정보를 불러옴
			DataManager.LoadAccountData();

			// 로그인 시도
			switch(DataManager.accountType) {
				case "Email":
					ServerManager.m.Post_LoginResult += LoginComplete;
					ServerManager.m.Post_Login_f(DataManager.email, DataManager.password);
					break;

				default:
					// 다음 Scene으로 
					SceneManager.LoadScene("LobbyScene");
					break;
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