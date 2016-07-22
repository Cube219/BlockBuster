using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserManager {

	public enum State { NotLogin, Login };
	public static State userState = State.NotLogin;

	public static string uid;
	public static string sid;
	public static string name;
	public static string userType;
	public static string accountType;

}
