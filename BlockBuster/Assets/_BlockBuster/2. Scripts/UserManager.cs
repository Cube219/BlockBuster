using UnityEngine;
using System.Collections;

public class UserManager {

	public static string uid;
	public static string sid;

	public enum State { NotLogin, Login };
	public static State userState = State.NotLogin;
}
