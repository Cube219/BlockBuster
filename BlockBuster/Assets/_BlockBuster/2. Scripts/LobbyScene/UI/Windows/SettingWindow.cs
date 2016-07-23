using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

namespace LobbyScene.UI.Windows {

	public class SettingWindow:MonoBehaviour {

		public FocusOutBackground focusOutBackground;
		public GameObject panels;

		public Text accountPanel_nameText;
		public Text emailSignUpPanel_errorText;
		public InputField createAccountPanel_nameInputField;
		public InputField emailSignUpPanel_emailInputField;
		public InputField emailSignUpPanel_passwordInputField;


		public enum Panel { Account, Login, CreateAccount, EmailSignUp };
		public Panel currentPanel = Panel.Account;

		// 보여줌
		public void Show()
		{
			this.GetComponent<Animator>().SetTrigger("ShowTrigger");
			focusOutBackground.Show();
		}

		// 숨김
		public void Hide()
		{
			this.GetComponent<Animator>().SetTrigger("HideTrigger");
			focusOutBackground.Hide();
		}

		// 해당 패널 보여줌
		public void ShowPanel(Panel panel)
		{
			currentPanel = panel;

			switch(currentPanel) {
				case Panel.Account:
					panels.GetComponent<Animator>().SetTrigger("GoAccount");
					break;

				case Panel.Login:
					panels.GetComponent<Animator>().SetTrigger("GoLogin");
					break;

				case Panel.CreateAccount:
					panels.GetComponent<Animator>().SetTrigger("GoCreateAccount");
					break;

				case Panel.EmailSignUp:
					panels.GetComponent<Animator>().SetTrigger("GoEmailSignup");
					break;

				default:
					break;
			}
		}

		// 해당 패널 즉시 보여줌
		public void ShowPanelImmediately(Panel panel)
		{
			currentPanel = panel;

			switch(currentPanel) {
				case Panel.Account:
					panels.GetComponent<Animator>().Play("Account");
					break;

				case Panel.Login:
					panels.GetComponent<Animator>().Play("Login");
					break;

				case Panel.CreateAccount:
					panels.GetComponent<Animator>().Play("CreateAccount");
					break;

				case Panel.EmailSignUp:
					panels.GetComponent<Animator>().Play("EmailSignup");
					break;

				default:
					break;
			}
		}

		// ----- AccountPanel -----
		// 로그아웃
		public void AccountPanel_Logout()
		{
			// 로그아웃
			ServerManager.m.Post_LogoutResult += LogoutResult;
			ServerManager.m.Post_Logout_f(UserManager.uid, UserManager.sid);
		}
		public void LogoutResult(Dictionary<string, string> data, bool isSuccess, string error)
		{
			ServerManager.m.Post_LogoutResult -= LogoutResult;

			if(isSuccess == true) { // 전송 성공
				// 데이터 지움
				DataManager.accountType = "NoData";
				
				// 상태 변경
				UserManager.name = null;
				UserManager.userState = UserManager.State.NotLogin;
				// 오프라인 모드로 변경
				LobbyManager.m.SetOffline();

				// LoginPanel로 이동
				ShowPanel(Panel.Login);
			} else { // 전송 실패
				Debug.Log("Fail to transfer to server!");
				Debug.Log(error);
				// 오프라인 모드로 변경
				LobbyManager.m.SetOffline();
			}
		}

		// 이름 변경
		public void AccountPanel_ChangeName()
		{
		}

		// ----- LoginPanel -----
		// 페이스북 로그인
		public void LoginPanel_LoginWithFacebook()
		{
			// 로그인
			FB.LogInWithReadPermissions(new List<string>{"public_profile"}, FBLoginCallback);
		}
		private void FBLoginCallback(ILoginResult result) {
			if(FB.IsLoggedIn) { // 로그인 됨
				Debug.Log("SuccessFul Login");
				// 페북 userid 저장
				DataManager.fbUserid = AccessToken.CurrentAccessToken.UserId;

				UserManager.name = null;
				// 서버로 로그인 시도
				ServerManager.m.Post_LoginResult += LoginWithFacebookResult;
				ServerManager.m.Post_LoginWithFacebook_f(DataManager.fbUserid);
			} else {
				Debug.Log("Login Denied");
			}
		}
		private void LoginWithFacebookResult(Dictionary<string, string> data, bool isSuccess, string error)
		{
			ServerManager.m.Post_LoginResult -= LoginWithFacebookResult;

			if(isSuccess == true) { // 전송 성공
				if(data["result"] == "False") { // 페북으로 로그인 실패
					// 계정을 새로 만듬
					ServerManager.m.Post_RegisterResult += SignUpWithFacebookResult;
					ServerManager.m.Post_RegisterWithFacebook_f(DataManager.fbUserid);
				} else { // 성공
					// 오프라인 모드 제거
					LobbyManager.m.DestroyOffline();

					// 받은 유저 정보를 UserManager에 넣어줌(name 제외)
					UserManager.userState = UserManager.State.Login;
					UserManager.uid = data["uid"];
					UserManager.sid = data["sid"];
					UserManager.accountType = data["accountType"];
					UserManager.userType = data["userType"];
					//UserManager.name = data["name"];

					// 로그인 정보 저장
					DataManager.accountType = UserManager.accountType;
					DataManager.SaveAccountData();

					// 방금 새로 만든 계정인 경우(UserManager.name에 이미 값이 있는 경우)
					if(UserManager.name != null) {
						// CreateAccountPanel로 이동
						ShowPanel(Panel.CreateAccount);
					} else {
						// name 값 넣어줌
						UserManager.name = data["name"];

						// AccountPanel로 이동
						accountPanel_nameText.text = UserManager.name;
						ShowPanel(Panel.Account);
					}
				}
			} else { // 전송 실패
				Debug.Log("Fail to transfer to server!");
				Debug.Log(error);
				// 오프라인 모드로 변경
				LobbyManager.m.SetOffline();
			}
			
		}
		private void SignUpWithFacebookResult(Dictionary<string, string> data, bool isSuccess, string error)
		{
			ServerManager.m.Post_RegisterResult -= SignUpWithFacebookResult;

			if(isSuccess == true) { // 전송 성공
				if(data["result"] == "False") { // 페북으로 계정 새로 만들기 실패
				} else { // 성공
					// 방금 새로 만든 계정이라는 흔적
					UserManager.name = "!@#$";
					// 서버로 로그인 시도
					ServerManager.m.Post_LoginResult += LoginWithFacebookResult;
					ServerManager.m.Post_LoginWithFacebook_f(DataManager.fbUserid);
				}
			} else { // 전송 실패
				Debug.Log("Fail to transfer to server!");
				Debug.Log(error);
				// 오프라인 모드로 변경
				LobbyManager.m.SetOffline();
			}
		}

		// 트위터 로그인
		public void LoginPanel_LoginWithTwitter()
		{
		}

		// 구글 로그인
		public void LoginPanel_LoginWithGoogle()
		{
		}

		// 이메일 로그인
		public void LoginPanel_LoginWithEmail()
		{
			// EmailSignUpPanel로 이동
			ShowPanel(Panel.EmailSignUp);
		}

		// ----- CreateAccountPanel -----
		// 계정 생성
		public void CreateAccountPanel_CreateAccount()
		{
			// 이름 바꾸라고 서버로 정보 보냄
			ServerManager.m.Post_ChangeNameResult += ChangeNameResult;
			ServerManager.m.Post_ChangeName_f(UserManager.uid, createAccountPanel_nameInputField.text);
		}
		public void ChangeNameResult(Dictionary<string, string> data, bool isSuccess, string error)
		{
			ServerManager.m.Post_ChangeNameResult -= ChangeNameResult;

			if(isSuccess == true) { // 전송 성공
				if(data["result"] == "False") { // 이름바꾸기 실패
				} else { // 성공
						 // UserManager에 반영
					UserManager.name = createAccountPanel_nameInputField.text;

					// AccountPanel로 이동
					accountPanel_nameText.text = UserManager.name;
					ShowPanel(Panel.Account);
				}
			} else { // 전송 실패
				Debug.Log("Fail to transfer to server!");
				Debug.Log(error);
				// 오프라인 모드로 변경
				LobbyManager.m.SetOffline();
			}
		}

		// ----- EmailSignUpPanel -----
		// 회원가입
		public void EmailSignUpPanel_SignUp()
		{
			emailSignUpPanel_errorText.text = "";
			// 서버로 정보 보냄
			ServerManager.m.Post_RegisterResult += SignUpResult;
			ServerManager.m.Post_RegisterWithEmail_f(emailSignUpPanel_emailInputField.text, emailSignUpPanel_passwordInputField.text);
		}
		public void SignUpResult(Dictionary<string, string> data, bool isSuccess, string error)
		{
			ServerManager.m.Post_RegisterResult -= SignUpResult;

			if(isSuccess == true) { // 전송 성공
				if(data["result"] == "False") { // 회원가입 실패
					if(data["error"] == "104") // 이미 이 이메일로 회원가입이 되어 있음
						emailSignUpPanel_errorText.text = "This Email is already existing.";
				} else { // 성공
						 // 방금 새로 만든 계정이라는 흔적
					UserManager.name = "!@#$";

					// 해당 정보로 바로 로그인
					EmailSignUpPanel_Login();
				}
			} else { // 전송 실패
				Debug.Log("Fail to transfer to server!");
				Debug.Log(error);
				// 오프라인 모드로 변경
				LobbyManager.m.SetOffline();
			}
		}

		// 로그인
		public void EmailSignUpPanel_Login()
		{
			emailSignUpPanel_errorText.text = "";
			// 서버로 정보 보냄
			ServerManager.m.Post_LoginResult += LoginResult;
			ServerManager.m.Post_LoginWithEmail_f(emailSignUpPanel_emailInputField.text, emailSignUpPanel_passwordInputField.text);
		}
		public void LoginResult(Dictionary<string, string> data, bool isSuccess, string error)
		{
			ServerManager.m.Post_LoginResult -= LoginResult;

			if(isSuccess == true) { // 전송 성공
				if(data["result"] == "False") { // 로그인 실패
					if(data["error"] == "500") // 이 이메일로 된 계정이 없음
						emailSignUpPanel_errorText.text = "Wrong email";
					else if(data["error"] == "501") // 비밀번호가 맞지 않음
						emailSignUpPanel_errorText.text = "Wrong password";
					else if(data["error"] == "502") // 이미 접속 중임.
						emailSignUpPanel_errorText.text = "Already accessing";

				} else { // 성공
					// 오프라인 모드 제거
					LobbyManager.m.DestroyOffline();

					// 받은 유저 정보를 UserManager에 넣어줌(name 제외)
					UserManager.userState = UserManager.State.Login;
					UserManager.uid = data["uid"];
					UserManager.sid = data["sid"];
					UserManager.accountType = data["accountType"];
					UserManager.userType = data["userType"];

					// 로그인 정보 저장
					DataManager.accountType = UserManager.accountType;
					switch(DataManager.accountType) {
						case "Email":
							DataManager.email = emailSignUpPanel_emailInputField.text;
							DataManager.password = emailSignUpPanel_passwordInputField.text;
							break;

						default:
							break;
					}
					DataManager.SaveAccountData();

					// 방금 새로 만든 계정인 경우(UserManager.name에 이미 값이 있는 경우)
					if(UserManager.name != null) {
						// CreateAccountPanel로 이동
						ShowPanel(Panel.CreateAccount);
					} else {
						// name 값 넣어줌
						UserManager.name = data["name"];

						// AccountPanel로 이동
						accountPanel_nameText.text = UserManager.name;
						ShowPanel(Panel.Account);
					}
				}
			} else { // 전송 실패
				Debug.Log("Fail to transfer to server!");
				Debug.Log(error);
				// 오프라인 모드로 변경
				LobbyManager.m.SetOffline();
			}
		}

		// 취소
		public void EmailSignUpPanel_Cancel()
		{
			// LoginPanel로 이동
			ShowPanel(Panel.Login);
		}

	}
}