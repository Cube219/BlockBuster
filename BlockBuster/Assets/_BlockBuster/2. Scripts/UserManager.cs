using UnityEngine;
using System.Collections;

public class UserManager {

	public enum State { NotLogin, Login };
	public static State userState = State.NotLogin;

	public static string uid;
	public static string sid;
	public static string name;
	public static string usertype;
	public static string accountType;
}
