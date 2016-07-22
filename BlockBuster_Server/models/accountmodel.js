var uuid = require("uuid");
var forge = require("node-forge");
var db = require("../app").db_connection;
var SessionModel = require("./sessionmodel");
//var N1qlQuery = require('couchbase').N1qlQuery;
var async = require("async");

function AccountModel() { };

// 계정 생성
AccountModel.create = function(params, callback) {
	// 계정 정보
    var userDoc = {
        uid: uuid.v4(),
		name: "New User",
        user_type: params.userType,
		account_type: params.accountType,
		email: params.email,
        password: forge.md.sha1.create().update(params.password).digest().toHex(),
		join_date: new Date()
    };
	// 이메일의 경우 중복된 이메일이 있는지 확인
	if(params.accountType == "Email"){
		db.query("SELECT * FROM User WHERE email = ?", [params.email], function(err, result){
			if(result.length == 0){ // 중복된 이메일이 없음(쿼리 결과가 없음)
				// 계정 DB에 넣음
				db.query("INSERT INTO User SET ?", userDoc, function(err, result){
					if(err) throw err;
					
					// 성공
					callback({"result": true, "error": 0});
					console.log("Complete to Create Account.");
				});
			} else { // 중복된 이메일이 있음(쿼리 결과가 있음)
				callback({"result": false, "error": 104});
			}
		});
	}
};

// 이름 변경
AccountModel.changeName = function(params, callback){
	// 이름 변경
	db.query("UPDATE User SET name = ? WHERE uid = ?", [params.name, params.uid], function(err, result){
		if(err) throw err;
		
		if(result.affectedRows == 0){// Row를 찾을 수 없다 (맞는 uid가 없다)
			callback({"result": false, "error": 200});
		} else {// 성공 (값이 안 바뀌어도 성공으로 치자)
			callback({"result": true, "error": 0});
		}
	});
};

// uid로 유저 정보 찾음
AccountModel.getByUid = function(params, callback) {
	// 검색
	db.query("SELECT * FROM User WHERE uid = ?", params.uid, function(err, result) {
		if(err) throw err;

		if(result.length == 0) { // 없음
			callback({"result": false, "error": 200});
		} else { // 있음
			callback({
				"result": true,
				"name": result[0].name, "userType": result[0].user_type, "accountType": result[0].account_type,
				"error": 0
			});
		}
	});
};

// 기타 정보로 유저 정보 찾음
AccountModel.getByOther = function(params, callback){
	if(params.accountType == "Email"){// 이메일 정보로 찾음
		// 검색
		db.query("SELECT * FROM User WHERE account_type = ? AND email = ?",
			[params.accountType, params.email],
			function(err, result){
				if(err) throw err;
				
				if(result.length == 0){// 없음
					callback({"result": false, "error": 201});
				} else {// 있음
					callback({"result": true, "error": 0});
				}
			});
	}
};

// 로그인
AccountModel.login = function (params, callback) {

	// 이메일 로그인 함수
	var loginWithEmail = function(callback){
		// 해당 이메일 계정 정보 가져옴
		db.query("SELECT * FROM User WHERE account_type = ? AND email = ?",
			[params.accountType, params.email],
			function(err, result){
				if(err) throw err;
				
				if(result.length == 0){// 없음
					// 에러 띄우고 종료
					callback({"result": false, "error": 500});
				} else {// 있음
					// 쿼리 결과값 보내주고 계속 진행
					callback(null, result[0]);
				}
			});
    };

	// 비밀번호 확인 함수
	var checkPassword = function(info, callback){
		// 비밀번호 비교
		var currentPass = forge.md.sha1.create().update(params.password).digest().toHex();
		if(currentPass === info.password) {// 맞음
			// 값 보내주고 계속 진행
			callback(null, info);
		} else {
			// 에러 띄우고 종료
			callback({"result": false, "error": 501});
		}
	}
	
	// 세션 생성 함수
	var createSession = function(err, result){
		if(err){// 에러가 있나?
			// 에러를 보내줌
			return callback(err);
		}
		
		// 세션을 생성
        SessionModel.create(result, function(result) {
			callback(result);
        });
	}
	
	// 이메일인 경우
	if(params.accountType == "Email"){
		async.waterfall([loginWithEmail, checkPassword], createSession);
	}
};

// 로그아웃
AccountModel.logout = function(params, callback) {
	// 세션 제거
	SessionModel.destroy(params.sid, function(result) {
		// 마지막 플레이 시간 기록
		db.query("UPDATE User SET last_play_date = ? WHERE uid = ?", [new Date(), params.uid], function(err, res) {
			if(err) throw err;

			callback(result);
		});
	});
};

module.exports = AccountModel;