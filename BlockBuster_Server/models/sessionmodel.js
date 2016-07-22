var uuid = require("uuid");
var db = require("../app").db_connection;
var logdb = require("../app").logdb_connection;

var sessionList = [];

function SessionModel() { };

// 세션 생성
SessionModel.create = function(params, callback){
	
	// 해당 uid로 세션이 있는지 확인
	var isAlreadyLogined = false;
	for(var s in sessionList) {
		if(sessionList[s].uid == params.uid){
			isAlreadyLogined = true;
			break;
		}
	}
	if(isAlreadyLogined == true){
		return callback({"result": false, error: 502});
	}
	
	// 세션 생성
	var session = {
        uid: params.uid,
		sid: uuid.v4(),
		remainTime: 3600
    };
	sessionList.push(session);

	console.log("A session is created. (current num : " + sessionList.length + ")");

	// 로그에 남김
	var log = { uid: session.uid, access_date: new Date(), close_date: null };
	logdb.query("INSERT INTO Log_Access SET ?", log);

	// 유저 정보 보냄
	callback({
		"result": true,
		"uid": params.uid, "sid": session.sid, "name": params.name, "accountType": params.account_type, "userType": params.user_type,
		error: 0
	});
};

// 세션 인증
SessionModel.auth = function(req, res, next) {
	var isOK = false;

	// 해당 sid가 있는지, sid랑 uis랑 맞는지 확인
	for(var s in sessionList) {
		if(sessionList[s].sid == req.body.sid && sessionList[s].uid == req.body.uid) {
			isOK = true;
			break;
		}
	}

	if(isOK == false) {
		res.send({ "result": false, "error": 600 });
	} else {
		// 맞으면 연장도 해줌
		SessionModel.refresh(req.body.sid);
		next();
	}
};

// 세션 갱신
SessionModel.refresh = function(sid) {
	for(var i = 0; i < sessionList.length; i++) {
		if(sessionList[i].sid == sid) {
			sessionList[i].remainTime = 3600;
			break;
		}
	}
};

// 세션 제거
SessionModel.destroy = function(sid, callback) {
	for(var s in sessionList) {
		if(sessionList[s].sid == sid) {
			// 로그에 남김
			var log = { uid: sessionList[s].uid, access_date: null, close_date: new Date() };
			logdb.query("INSERT INTO Log_Access SET ?", log);

			// 제거
			sessionList.splice(s, 1);
			console.log("A session is destroied. (current num : " + sessionList.length + ")");

			return callback({"result": true, "error": 0});
		}
	}
};

// 세션 관리
function controlSession() {
	// 시간 다운
	for(var i = 0; i < sessionList.length; i++) {
		sessionList[i].remainTime -= 1;

		if(sessionList[i].remainTime < 0) {// 시간 다 된것은 지움
			sessionList.splice(i, 1);
			console.log("A session is destroied. (current num : " + sessionList.length + ")");
			i--;
		}
	}
}

//------------------------------------------------------------
/*
// 
SessionModel.authenticate = function(req, res, next) {
    if(!req.headers.authorization) {
        next("Must be authorized to use");
    }
    var authInfo = req.headers.authorization.split(" ");
    if(authInfo[0] === "Bearer") {
        var sid = authInfo[1];
        SessionModel.get(sid, function(error, result) {
            if(error) {
                return next(error);
            }
            SessionModel.refresh(sid, function() {});
            req.uid = result.value.uid;
            next();
        });
    }
};

// 세션 가져옴
SessionModel.get = function(sid, callback) {
    db.get("session::" + sid, function(error, result) {
        if(error) {
            callback(error, null);
            return;
        }
        callback(null, result);
    });
};

// 갱신?
SessionModel.refresh = function(sid, callback) {
    db.touch("session::" + sid, 3600, function(error, result) {
        if(error) {
            callback(error, null);
        }
    });
};*/

setInterval(controlSession, 1000);

module.exports = SessionModel;