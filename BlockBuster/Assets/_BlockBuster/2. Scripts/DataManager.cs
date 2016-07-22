using UnityEngine;
using System.Collections;

public class DataManager {

	public static string accountType;
	public static string email;
	public static string password;
	public static string sid;

	// 계정 정보를 불러옴
	public static void LoadAccountData()
	{
		// 저장된 계정 정보를 불러온다
		accountType = PlayerPrefs.GetString("accountType", "NoData");

		if(accountType.Equals("NoData")) { // 계정 정보가 없다

		} else {
			switch(accountType) {
				case "Email":
					email = PlayerPrefs.GetString("email");
					password = PlayerPrefs.GetString("password");
					sid = PlayerPrefs.GetString("sid");
					break;

				default:
					break;
			}
		}
	}

	// 계정 정보를 저장
	public static void SaveAccountData()
	{
		PlayerPrefs.SetString("accountType", accountType);

		switch(accountType) {
			case "Email":
				PlayerPrefs.SetString("email", email);
				PlayerPrefs.SetString("password", password);
				PlayerPrefs.SetString("sid", sid);
				break;

			default:
				break;
		}
	}

}
