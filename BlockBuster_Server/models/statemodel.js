var uuid = require("uuid");
var db = require("../app").db_connection;
var logdb = require("../app").logdb_connection;
var async = require("async");

function StateModel() { };

// ���ھ� ����
StateModel.saveScore = function(uid, score, callback) {

	var previousRank, currentRank;
	var isNewRecord = false;

	// �� ��� �߰��ϴ� �Լ�
	var addMyScore = function(callback) {
		db.query("INSERT INTO Score SET ?", { uid: uid, score: score }, function(err, result) {
			if(err) throw err;

			callback(null);
		});
	};

	// �� ��� �����ϴ� �Լ�
	var updateMyScore = function(callback) {
		// �� ���� ������
		db.query("SELECT score FROM Score WHERE uid = ?", uid, function(err, result) {
			if(err) throw err;
			
			// ���� ������ ������ �������� Ŭ ���(�ű��)
			if(result[0].score < score) {
				isNewRecord = true;
				// ���ο� ������ ����
				db.query("UPDATE Score SET score = ? WHERE uid = ?", [score, uid], function(err, result) {
					if(err) throw err;

					callback(null);
				});
			} else {
				callback(null);
			}
		});
	};

	// DB ��� �������� �Լ�
	var getScoreDataInDB = function(callback) {
		db.query("SELECT uid, score, (SELECT COUNT(*) + 1 FROM Score WHERE score > t.score) AS rank FROM Score AS t ORDER BY rank ASC;", function(err, result) {
			if(err) throw err;

			// ���� ����� �����ְ� ��� ����
			callback(null, result);
		});
	};

	// ��ŷ �������� �Լ�
	var getRank = function(scoreData, callback) {
		for(var i in scoreData) {
			if(scoreData[i].uid == uid) {
				// ��ŷ �� �����ְ� ��� ����
				callback(null, scoreData[i].rank);
			}
		}
	};

	// ������ �Լ�
	var final = function(err, result) {
		currentRank = result;
		// ó�� �� ���� ��ȭ�� �����Ƿ� 0���� ����� ���� ���� ��ũ�� �־���
		if(previousRank == null) {
			previousRank = currentRank;
			isNewRecord = true;
		}

		// �α׿� ����
		var log = { uid: uid, score: score, get_time: new Date() };
		logdb.query("INSERT INTO Log_Score SET ?", log);

		var r = {
            "result": true,
            
			"isNewRecord": isNewRecord, "rank": currentRank, "rankChange": previousRank - currentRank,
			
			"error": 0
		};

		// DB�� previous_rank �� �־���
		db.query("UPDATE User SET previous_rank = ? WHERE uid = ?", [currentRank, uid], function(err, result) {
			if(err) throw err;

			// ����� ������
			callback(r);
		});
	};
	// -------------------------------

	// ���� ��ŷ ������
	db.query("SELECT previous_rank FROM User WHERE uid = ?", uid, function(err, result) {
		if(err) throw err;
		previousRank = result[0].previous_rank;

		// ó�� ����ϴ� ���(previousRank�� null�� ���)
		if(previousRank == null) {
			// �߰� -> ������ ������ -> ��ŷ ������ -> ������
			async.waterfall([addMyScore, getScoreDataInDB, getRank], final);
		} else { // ������ ���
			// ���� -> ������ ������ -> ��ŷ ������ -> ������
			async.waterfall([updateMyScore, getScoreDataInDB, getRank], final);
		}
	});
}

// ��ŷ ������
StateModel.getRanks = function(uid, callback) {

	// DB ��� �������� �Լ�
	var getScoreDataInDB = function(callback) {
		db.query("SELECT uid, score, (SELECT COUNT(*) + 1 FROM Score WHERE score > t.score) AS rank FROM Score AS t ORDER BY rank ASC;", function(err, result) {
			if(err) throw err;

			// ���� ����� �����ְ� ��� ����
			callback(null, result);
		});
	};

	// ��ŷ �������� �Լ�
	var getRank = function(scoreData, callback) {
		for(var i in scoreData) {
			if(scoreData[i].uid == uid) {
				// ��ŷ �� �����ְ� ��� ����
				callback(null, scoreData[i].rank);
			}
		}
	};

	// ������ ������ -> ��ŷ ������ -> ������
	async.waterfall([getScoreDataInDB, getRank], function(err, result) {
		callback({"result": true, "rank": result, "error": 0});
	});
};

// ��ŷ�� ������
StateModel.getRanks = function(rankStart, rankEnd, callback) {
	// ��ŷ �� ������
	db.query("SELECT u.uid, u.name, s.score, (SELECT COUNT(*) + 1 FROM Score WHERE score > s.score) AS rank FROM Score AS s JOIN User AS u ON u.uid=s.uid ORDER BY rank ASC;", function(err, result) {
		if(err) throw err;

		// �ʰ��� ��� �ڸ�
		if(rankEnd > result.length)
			rankEnd = result.length;
		if(rankStart < 1)
			rankStart = 1;

		// ����Ʈ�� �� �־���
		var list = [];
		for(var i = rankStart; i <= rankEnd; i++) {
			list[i - rankStart] = { "uid": result[i-1].uid, "name": result[i-1].name, "score": result[i-1].score, "rank": result[i-1].rank };
		}

		callback({"result": true, "data": list, "error": 0});
	});
};

module.exports = StateModel;