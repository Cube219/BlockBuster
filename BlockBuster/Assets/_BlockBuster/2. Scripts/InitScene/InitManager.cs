using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace InitScene {

	public class InitManager:MonoBehaviour {

		void Start()
		{
			ServerManager.m.Post_LoginResult += LoginComplete;
			ServerManager.m.Post_Login_f("max804@naver.com", "12345");
			
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
				// 다음 Scene으로 
				SceneManager.LoadScene("LobbyScene");
			} else { // 로그인 성공
				// 받은 uid와 sid를 UserManager에 넣어줌
				UserManager.userState = UserManager.State.Login;
				UserManager.uid = data["uid"];
				UserManager.sid = data["sid"];

				// 유저 정보를 가져옴
				ServerManager.m.Get_GetUserByUidResult += GetUserInfoComplete;
				ServerManager.m.Get_GetUserByUid_f(UserManager.uid);
			}
		}

		public void GetUserInfoComplete(Dictionary<string, string> data)
		{
			ServerManager.m.Get_GetUserByUidResult -= GetUserInfoComplete;

			if(data["result"] == "False") { // 유저 정보 가져오기 실패
			} else { // 성공
				// 가져온 정보들을 UserManager에 넣어줌
				UserManager.name = data["name"];
				UserManager.usertype = data["userType"];
				UserManager.accountType = data["accountType"];
				Debug.Log(data);
			}

			// 다음 Scene으로 
			SceneManager.LoadScene("LobbyScene");
		}
	}
}