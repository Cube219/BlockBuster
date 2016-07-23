using UnityEngine;
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

	public delegate void Post_Register(Dictionary<string, string> data, bool isSuccess, string error = null);
	public delegate void Post_ChangeName(Dictionary<string, string> data, bool isSuccess, string error = null);
	public delegate void Get_GetUserByUid(Dictionary<string, string> data, bool isSuccess, string error = null);
	public delegate void Post_Login(Dictionary<string, string> data, bool isSuccess, string error = null);
	public delegate void Post_Score(Dictionary<string, string> data, bool isSuccess, string error = null);
	public delegate void Post_Logout(Dictionary<string, string> data, bool isSuccess, string error = null);
	public delegate void Get_GetRank(Dictionary<string, string> data, bool isSuccess, string error = null);
	public delegate void Get_GetRanks(List<Dictionary<string, string>> data, bool isSuccess, string error = null);

	public event Post_Register Post_RegisterResult;
	public event Post_ChangeName Post_ChangeNameResult;
	public event Get_GetUserByUid Get_GetUserByUidResult;
	public event Post_Login Post_LoginResult;
	public event Post_Score Post_ScoreResult;
	public event Post_Logout Post_LogoutResult;
	public event Get_GetRank Get_GetRankResult;
	public event Get_GetRanks Get_GetRanksResult;

	public bool isLastTransferFailed = false;
	
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
	public void Post_RegisterWithEmail_f(string email, string password)
	{
		Dictionary<string, string> t = new Dictionary<string, string> {
			{"userType", "User"},
			{"accountType", "Email"},
			{"email", email},
			{"password", password}
		};

		StartCoroutine(Post(url + "/api/v1/user", t, (string result, bool isSuccess, string error) => {
			if(isSuccess == true)
				Post_RegisterResult(ProcessData(result), isSuccess);
			else
				Post_RegisterResult(null, isSuccess, error);
		}));
	}
	// 회원가입(Facebook)
	public void Post_RegisterWithFacebook_f(string fbUserId)
	{
		Dictionary<string, string> t = new Dictionary<string, string> {
			{"userType", "User"},
			{"accountType", "Facebook"},
			{"fbUserId", fbUserId}
		};

		StartCoroutine(Post(url + "/api/v1/user", t, (string result, bool isSuccess, string error) => {
			if(isSuccess == true)
				Post_RegisterResult(ProcessData(result), isSuccess);
			else
				Post_RegisterResult(null, isSuccess, error);
		}));
	}

	// 회원 이름 변경
	public void Post_ChangeName_f(string uid, string name)
	{
		Dictionary<string, string> t = new Dictionary<string, string> {
			{"uid", uid},
			{"name", name}
		};
		
		StartCoroutine(Post(url + "/api/v1/user/change_name", t, (string result, bool isSuccess, string error) => {
			if(isSuccess == true)
				Post_ChangeNameResult(ProcessData(result), isSuccess);
			else
				Post_ChangeNameResult(null, isSuccess, error);
		}));
	}

	// 회원 조회(uid)
	public void Get_GetUserByUid_f(string uid)
	{
		Dictionary<string, string> t = new Dictionary<string, string> {
			{"uid", uid}
		};

		StartCoroutine(Get(url + "/api/v1/user/uid", t, (string result, bool isSuccess, string error) => {
			if(isSuccess == true)
				Get_GetUserByUidResult(ProcessData(result), isSuccess);
			else
				Get_GetUserByUidResult(null, isSuccess, error);
		}));
	}
	
	// 로그인(Email)
	public void Post_LoginWithEmail_f(string email, string password)
	{
		Dictionary<string, string> t = new Dictionary<string, string> {
			{"accountType", "Email"},
			{"email", email},
			{"password", password}
		};
		
		StartCoroutine(Post(url + "/api/v1/user/login", t, (string result, bool isSuccess, string error) => {
			if(isSuccess == true)
				Post_LoginResult(ProcessData(result), isSuccess);
			else
				Post_LoginResult(null, isSuccess, error);
		}));
	}
	// 로그인(Facebook)
	public void Post_LoginWithFacebook_f(string fbUserId)
	{
		Dictionary<string, string> t = new Dictionary<string, string> {
			{"accountType", "Facebook"},
			{"fbUserId", fbUserId}
		};

		StartCoroutine(Post(url + "/api/v1/user/login", t, (string result, bool isSuccess, string error) => {
			if(isSuccess == true)
				Post_LoginResult(ProcessData(result), isSuccess);
			else
				Post_LoginResult(null, isSuccess, error);
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
		
		StartCoroutine(Post(url + "/api/v1/score", t, (string result, bool isSuccess, string error) => {
			if(isSuccess == true)
				Post_ScoreResult(ProcessData(result), isSuccess);
			else
				Post_ScoreResult(null, isSuccess, error);
		}));
	}

	// 로그아웃
	public void Post_Logout_f(string uid, string sid)
	{
		Dictionary<string, string> t = new Dictionary<string, string> {
			{"uid", uid},
			{"sid", sid}
		};
		
		StartCoroutine(Post(url + "/api/v1/user/logout", t, (string result, bool isSuccess, string error) => {
			if(isSuccess == true)
				Post_LogoutResult(ProcessData(result), isSuccess);
			else
				Post_LogoutResult(null, isSuccess, error);
		}));
	}
	
	// 순위 가져옴
	public void Get_GetRank_f(string uid)
	{
		Dictionary<string, string> t = new Dictionary<string, string> {
			{"uid", uid}
		};

		StartCoroutine(Get(url + "/api/v1/score", t, (string result, bool isSuccess, string error) => {
			if(isSuccess == true)
				Get_GetRankResult(ProcessData(result), isSuccess);
			else
				Get_GetRankResult(null, isSuccess, error);
		}));
	}

	// 순위들 가져옴
	public void Get_GetRanks_f(int rankStart, int rankEnd)
	{
		Dictionary<string, string> t = new Dictionary<string, string> {
			{"rankStart", rankStart.ToString()},
			{"rankEnd", rankEnd.ToString() }
		};

		StartCoroutine(Get(url + "/api/v1/score/multi", t, (string result, bool isSuccess, string error) => {
			if(isSuccess == true) {
				JSONObject j = new JSONObject(result);

				List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
				foreach(JSONObject o in j.list[1].list) {
					data.Add(o.ToDictionary());
				}
				Get_GetRanksResult(data, isSuccess);
			} else
				Get_GetRanksResult(null, isSuccess, error);
		}));
	}

	// ------------------------------------
	private IEnumerator Get(string url, Dictionary<string, string> datas, System.Action<string, bool, string> callback)
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

		// 보냄 (3번까지 시도)
		for(int i = 1; i <= 3; i++) {
			yield return www;
			Debug.Log(i + "번째 시도");
			if(www.error == null) { // 이상 없음
				isLastTransferFailed = false;
				GameObject.FindGameObjectWithTag("FailServerText").GetComponent<FailServerText>().SuccessRetry();
				callback(www.text, true, null); // 값 보내줌
				yield break;
			}
			GameObject.FindGameObjectWithTag("FailServerText").GetComponent<FailServerText>().Retry(i, 3);
		}

		// 이상 있음
		GameObject.FindGameObjectWithTag("FailServerText").GetComponent<FailServerText>().FailRetry();
		isLastTransferFailed = true;
		callback(null, false, www.error);
	}

	private IEnumerator Post(string url, Dictionary<string, string> datas, System.Action<string, bool, string> callback)
	{
		WWWForm form = new WWWForm();
		foreach(KeyValuePair<string, string> data in datas) {
			form.AddField(data.Key, data.Value);
		}
		
		WWW www = new WWW(url, form);

		// 보냄 (3번까지 시도)
		for(int i = 1; i <= 3; i++) {
			yield return www;
			Debug.Log(i + "번째 시도");
			if(www.error == null) { // 이상 없음
				isLastTransferFailed = false;
				GameObject.FindGameObjectWithTag("FailServerText").GetComponent<FailServerText>().SuccessRetry();
				callback(www.text, true, www.error); // 값 보내줌
				yield break;
			}
			GameObject.FindGameObjectWithTag("FailServerText").GetComponent<FailServerText>().Retry(i, 3);
		}

		// 이상 있음
		GameObject.FindGameObjectWithTag("FailServerText").GetComponent<FailServerText>().FailRetry();
		isLastTransferFailed = true;
		callback(null, false, www.error);

	}
}
