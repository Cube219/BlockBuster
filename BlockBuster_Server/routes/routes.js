var AccountModel = require("../models/accountmodel");
var SessionModel = require("../models/sessionmodel");
var StateModel = require("../models/statemodel");

var appRouter = function(app) {

    app.get("/", function(req, res) {
        res.status(403).send("Not a valid endpoint");
    });

	// 1. 회원가입
	app.post("/api/v1/user", function(req, res) {
		// 값 없는것 있으면 오류 출력
		if(!req.body.userType) {
			return res.status(400).send({"status": "error", "message": "A userType is required"});
		} else if(!req.body.accountType) {
			return res.status(400).send({"status": "error", "message": "A accountType is required"});
		}
		// 계정 생성
		AccountModel.create(req.body, function(result) {
			if(result.result == false) {
				return res.status(400).send(result);
			}
			res.send(result);
		});
	});
	
	// 2. 회원 이름 변경
	app.post("/api/v1/user/change_name", function(req, res){
		// 값 업는것 있으면 오류 출력
		if(!req.body.uid){
			return res.status(400).send({"status": "error", "message": "A uid is required"});
		} else if(!req.body.name){
			return res.status(400).send({"status": "error", "message": "A name is required"});
		}
		// 회원 이름 변경
		AccountModel.changeName(req.body, function(result){
			res.send(result);
		});
	});
	
	// 3. 회원 조회(uid)
    app.get("/api/v1/user/uid", function(req, res) {
		// 값 없는 것 있으면 오류 출력
		if(!req.query.uid) {
			return res.state(400).send({ "status": "error", "message": "A uid is required" });
		}
		// 조회
		AccountModel.getByUid(req.query, function(result) {
			res.send(result);
		});
    });
	
	// 4. 회원 조회(기타)
	app.get("/api/v1/user/etc", function(req, res){
		// 값 없는것 있으면 오류 출력
		if(!req.query.accountType){
			return res.status(400).send({"status": "error", "message": "A accountType is required"});
		}
		// 조회
		AccountModel.getByOther(req.query, function(result){
			res.send(result);
		});
	});
	
	// 5. 로그인
	app.post("/api/v1/user/login", function(req, res){
		// 값 없는것 있으면 오류 출력
		if(!req.body.accountType){
			return res.status(400).send({"status": "error", "message": "A accountType is required"});
		}
		
		// 로그인
		AccountModel.login(req.body, function(result){
			res.send(result);
		});
	});

	// 6. 결과 전송
	app.post("/api/v1/score", SessionModel.auth, function(req, res) {
		// 값 없는것 있으면 오류 출력
		if(!req.body.score) {
			return res.status(400).send({ "status": "error", "message": "A score is required" });
		}
		// 값 저장
		StateModel.saveScore(req.body.uid, req.body.score, function(result) {
			res.send(result);
		});
	});

	// 7. 로그아웃
	app.post("/api/v1/user/logout", SessionModel.auth, function(req, res) {
		// 로그아웃
		AccountModel.logout(req.body, function(result) {
			res.send(result);
		});
	});

	// 8. 순위 가져옴
	app.get("/api/v1/score", function(req, res) {
		// 값 없는것 있으면 오류 출력
		if(!req.query.uid) {
			return res.status(400).send({ "status": "error", "message": "A uid is required" });
		}
		// 가져옴
		StateModel.getRank(req.query.uid, function(result) {
			res.send(result);
		});
	});

	// 9. 순위들 가져옴
	app.get("/api/v1/score/multi", function(req, res) {
		// 값 없는것 있으면 오류 출력
		if(!req.query.rankStart) {
			return res.status(400).send({ "status": "error", "message": "A rankStart is required" });
		} else if(!req.query.rankEnd) {
			return res.status(400).send({ "status": "error", "message": "A rankEnd is required" });
		}
		// 가져옴
		StateModel.getRanks(req.query.rankStart, req.query.rankEnd, function(result) {
			res.send(result);
		});
	});

	// 10. 세션 로그인
	app.post("/api/v1/user/login/session", function(req, res) {
		// 값 없는것 있으면 오류 출력
		if(!req.body.sid) {
			return res.status(400).send({ "status": "error", "message": "A sid is required" });
		}

		// 로그인
		AccountModel.loginWithSession(req.body, function(result) {
			res.send(result);
		});
	});
}

module.exports = appRouter;