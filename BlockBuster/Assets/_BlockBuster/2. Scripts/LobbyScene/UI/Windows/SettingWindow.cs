using UnityEngine;
using System.Collections;

namespace LobbyScene.UI.Windows {

	public class SettingWindow:MonoBehaviour {

		public FocusOutBackground focusOutBackground;
		public GameObject panels;

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
			// LoginPanel로 이동
			ShowPanel(Panel.Login);
		}

		// 이름 변경
		public void AccountPanel_ChangeName()
		{
		}

		// ----- LoginPanel -----
		// 페이스북 로그인
		public void LoginPanel_LoginWithFacebook()
		{
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
		}

		// ----- EmailSignUpPanel -----
		// 회원가입
		public void EmailSignUpPanel_SignUp()
		{
		}

		// 로그인
		public void EmailSignUpPanel_Login()
		{
		}

		// 취소
		public void EmailSignUpPanel_Cancel()
		{
			// LoginPanel로 이동
			ShowPanel(Panel.Login);
		}

	}
}