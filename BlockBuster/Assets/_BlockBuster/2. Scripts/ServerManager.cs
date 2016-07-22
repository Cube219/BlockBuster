using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerManager : MonoBehaviour {

	private static ServerManager _m;
	public static ServerManager m
	{
		get
		{
			if(!_m) {
				GameObject c = new GameObject();
				c.name = "ServerManager";
				_m = c.AddComponent<ServerManager>();
			}
			return _m;
		}
	}

	private string url = "http://52.78.64.117:3000";

	public delegate void Post_Register(Dictionary<string, string> data);
	public delegate void Post_ChangeName(Dictionary<string, string> data);
	public delegate void Post_Login(Dictionary<string, string> data);
	public delegate void Post_Score(Dictionary<string, string> data);
	public delegate void Post_Logout(Dictionary<string, string> data);

	public event Post_Register Post_RegisterResult;
	public event Post_ChangeName Post_ChangeNameResult;
	public event Post_Login Post_LoginResult;
	public event Post_Score Post_ScoreResult;
	public event Post_Logout Post_LogoutResult;

	// 데이터 처리
	private Dictionary<string, string> ProcessData(string jsonrawData)
	{
		JSONObject t = new JSONObject(jsonrawData);
		return t.ToDictionary();
	}

	// 회원가입(Email)
	public void Post_Register_f(string email, string password)
	{
		Dictionary<string, string> t = new Dictionary<string, string> {
			{"userType", "User"},
			{"accountType", "Email"},
			{"email", email},
			{"passsword", password}
		};

		StartCoroutine(Post(url + "/api/v1/user", t, (string result)=> {
			Post_RegisterResult(ProcessData(result));
		}));
	}

	// 회원 이름 변경
	public void Post_ChangeName_f(string uid, string name)
	{
		Dictionary<string, string> t = new Dictionary<string, string> {
			{"uid", uid},
			{"name", name}
		};
		
		StartCoroutine(Post(url + "/api/v1/user/change_name", t, (string result) => {
			Post_ChangeNameResult(ProcessData(result));
		}));
	}

	// 로그인(Email)
	public void Post_Login_f(string email, string password)
	{
		Dictionary<string, string> t = new Dictionary<string, string> {
			{"accountType", "Email"},
			{"email", email},
			{"password", password}
		};
		
		StartCoroutine(Post(url + "/api/v1/user/login", t, (string result) => {
			Post_LoginResult(ProcessData(result));
		}));
	}

	// 결과 전송
	public void Post_Score_f(string uid, string sid, int score)
	{
		Dictionary<string, string> t = new Dictionary<string, string> {
			{"uid", uid},
			{"sid", sid},
			{"score", score.ToString()}
		};
		
		StartCoroutine(Post(url + "/api/v1/score", t, (string result) => {
			Post_ScoreResult(ProcessData(result));
		}));
	}

	// 로그아웃
	public void Post_Logout_f(string uid, string sid)
	{
		Dictionary<string, string> t = new Dictionary<string, string> {
			{"uid", uid},
			{"sid", sid}
		};
		
		StartCoroutine(Post(url + "/api/v1/user/logout", t, (string result) => {
			Post_LogoutResult(ProcessData(result));
		}));
	}

	// ------------------------------------
	private IEnumerator Post(string url, Dictionary<string, string> datas, System.Action<string> callback)
	{
		WWWForm form = new WWWForm();
		foreach(KeyValuePair<string, string> data in datas) {
			form.AddField(data.Key, data.Value);
		}
		
		WWW www = new WWW(url, form);

		// 보냄
		yield return www;

		if(www.error == "Null") {
		} else {
			callback(www.text);
		}

	}
}
