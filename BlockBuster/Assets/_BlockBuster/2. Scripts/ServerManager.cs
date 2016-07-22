﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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
				DontDestroyOnLoad(c);
			}
			return _m;
		}
	}

	private string url = "http://52.78.64.117:3000";

	public delegate void Post_Register(Dictionary<string, string> data);
	public delegate void Post_ChangeName(Dictionary<string, string> data);
	public delegate void Get_GetUserByUid(Dictionary<string, string> data);
	public delegate void Post_Login(Dictionary<string, string> data);
	public delegate void Post_Score(Dictionary<string, string> data);
	public delegate void Post_Logout(Dictionary<string, string> data);

	public event Post_Register Post_RegisterResult;
	public event Post_ChangeName Post_ChangeNameResult;
	public event Get_GetUserByUid Get_GetUserByUidResult;
	public event Post_Login Post_LoginResult;
	public event Post_Score Post_ScoreResult;
	public event Post_Logout Post_LogoutResult;
	
	void OnApplicationQuit()
	{
		// 로그아웃 처리
		if(UserManager.userState == UserManager.State.Login)
			Post_Logout_f(UserManager.uid, UserManager.sid);
	}

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
			{"password", password}
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

	// 회원 조회(uid)
	public void Get_GetUserByUid_f(string uid)
	{
		Dictionary<string, string> t = new Dictionary<string, string> {
			{"uid", uid}
		};

		StartCoroutine(Get(url + "/api/v1/user/uid", t, (string result) => {
			Get_GetUserByUidResult(ProcessData(result));
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
	private IEnumerator Get(string url, Dictionary<string, string> datas, System.Action<string> callback)
	{
		StringBuilder b = new StringBuilder(url);
		b.Append("?");

		if(datas.Count != 0) {
			foreach(KeyValuePair<string, string> data in datas) {
				b.Append(data.Key).Append("=").Append(data.Value).Append("&");
			}
			b.Remove(b.Length - 1, 1);
		}

		WWW www = new WWW(b.ToString());

		// 보냄
		yield return www;

		if(www.error == "Null") {
		} else {
			callback(www.text);
		}
	}

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
