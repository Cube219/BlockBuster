var uuid = require("uuid");
var db = require("../app").db_connection;
var logdb = require("../app").logdb_connection;
var async = require("async");

function StateModel() { };

// 스코어 저장
StateModel.saveScore = function(uid, score, callback) {

	var previousRank, currentRank;
	var isNewRecord = false;

	// 내 기록 추가하는 함수
	var addMyScore = function(callback) {
		db.query("INSERT INTO Score SET ?", { uid: uid, score: score }, function(err, result) {
			if(err) throw err;

			callback(null);
		});
	};

	// 내 기록 갱신하는 함수
	var updateMyScore = function(callback) {
		// 내 점수 가져옴
		db.query("SELECT score FROM Score WHERE uid = ?", uid, function(err, result) {
			if(err) throw err;
			
			// 얻은 점수가 가져옴 점수보다 클 경우(신기록)
			if(result[0].score < score) {
				isNewRecord = true;
				// 새로운 점수로 갱신
				db.query("UPDATE Score SET score = ? WHERE uid = ?", [score, uid], function(err, result) {
					if(err) throw err;

					callback(null);
				});
			} else {
				callback(null);
			}
		});
	};

	// DB 기록 가져오는 함수
	var getScoreDataInDB = function(callback) {
		db.query("SELECT uid, score, (SELECT COUNT(*) + 1 FROM Score WHERE score > t.score) AS rank FROM Score AS t ORDER BY rank ASC;", function(err, result) {
			if(err) throw err;

			// 쿼리 결과값 보내주고 계속 진행
			callback(null, result);
		});
	};

	// 랭킹 가져오는 함수
	var getRank = function(scoreData, callback) {
		for(var i in scoreData) {
			if(scoreData[i].uid == uid) {
				// 랭킹 값 보내주고 계속 진행
				callback(null, scoreData[i].rank);
			}
		}
	};

	// 마지막 함수
	var final = function(err, result) {
		currentRank = result;
		// 처음 한 경우는 변화가 없으므로 0으로 만들기 위해 현재 랭크로 넣어줌
		if(previousRank == null) {
			previousRank = currentRank;
			isNewRecord = true;
		}

		// 로그에 남김
		var log = { uid: uid, score: score, get_time: new Date() };
		logdb.query("INSERT INTO Log_Score SET ?", log);

		var r = {
            "result": true,
            
			"isNewRecord": isNewRecord, "rank": currentRank, "rankChange": previousRank - currentRank,
			
			"error": 0
		};

		// DB에 previous_rank 값 넣어줌
		db.query("UPDATE User SET previous_rank = ? WHERE uid = ?", [currentRank, uid], function(err, result) {
			if(err) throw err;

			// 결과값 보내줌
			callback(r);
		});
	};
	// -------------------------------

	// 이전 랭킹 가져옴
	db.query("SELECT previous_rank FROM User WHERE uid = ?", uid, function(err, result) {
		if(err) throw err;
		previousRank = result[0].previous_rank;

		// 처음 등록하는 경우(previousRank가 null인 경우)
		if(previousRank == null) {
			// 추가 -> 데이터 가져옴 -> 랭킹 가져옴 -> 마지막
			async.waterfall([addMyScore, getScoreDataInDB, getRank], final);
		} else { // 갱신인 경우
			// 갱신 -> 데이터 가져옴 -> 랭킹 가져옴 -> 마지막
			async.waterfall([updateMyScore, getScoreDataInDB, getRank], final);
		}
	});
}

// 랭킹 가져옴
StateModel.getRanks = function(uid, callback) {

	// DB 기록 가져오는 함수
	var getScoreDataInDB = function(callback) {
		db.query("SELECT uid, score, (SELECT COUNT(*) + 1 FROM Score WHERE score > t.score) AS rank FROM Score AS t ORDER BY rank ASC;", function(err, result) {
			if(err) throw err;

			// 쿼리 결과값 보내주고 계속 진행
			callback(null, result);
		});
	};

	// 랭킹 가져오는 함수
	var getRank = function(scoreData, callback) {
		for(var i in scoreData) {
			if(scoreData[i].uid == uid) {
				// 랭킹 값 보내주고 계속 진행
				callback(null, scoreData[i].rank);
			}
		}
	};

	// 데이터 가져옴 -> 랭킹 가져옴 -> 마지막
	async.waterfall([getScoreDataInDB, getRank], function(err, result) {
		callback({"result": true, "rank": result, "error": 0});
	});
};

// 랭킹들 가져옴
StateModel.getRanks = function(rankStart, rankEnd, callback) {
	// 랭킹 다 가져옴
	db.query("SELECT u.uid, u.name, s.score, (SELECT COUNT(*) + 1 FROM Score WHERE score > s.score) AS rank FROM Score AS s JOIN User AS u ON u.uid=s.uid ORDER BY rank ASC;", function(err, result) {
		if(err) throw err;

		// 초과할 경우 자름
		if(rankEnd > result.length)
			rankEnd = result.length;
		if(rankStart < 1)
			rankStart = 1;

		// 리스트에 다 넣어줌
		var list = [];
		for(var i = rankStart; i <= rankEnd; i++) {
			list[i - rankStart] = { "uid": result[i-1].uid, "name": result[i-1].name, "score": result[i-1].score, "rank": result[i-1].rank };
		}

		callback({"result": true, "data": list, "error": 0});
	});
};

module.exports = StateModel;