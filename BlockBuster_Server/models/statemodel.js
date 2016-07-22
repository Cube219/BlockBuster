var uuid = require("uuid");
//var couchbase = require("couchbase");
//var N1qlQuery = require('couchbase').N1qlQuery;
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

		var r = {
            "result": true,
            
			"isNewRecord": isNewRecord, "rank": currentRank, "rankChange": previousRank - currentRank,
			
			"error": null
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

/*
StateModel.save = function(uid, name, preVer, data, callback) {
    db.get("user::" + uid + "::state", function(error, result) {
        if(error && error.code !== couchbase.errors.keyNotFound) {
            callback(error, null);
            return;
        }
        var stateDoc = {
            type: "state",
            uid: uid,
            states: {}
        };
        if(result != null && result.value) {
            stateDoc = result.value;
        }
        var stateBlock = {
            version: 0,
            data: null
        };
        if(stateDoc.states[name]) {
            stateBlock = stateDoc.states[name];
        } else {
            stateDoc.states[name] = stateBlock;
        }
        if(stateBlock.version !== preVer) {
            return callback({"status": "error", "message": "Your version does not match the server version"});
        } else {
            stateBlock.version++;
            stateBlock.data = data;
        }
        var stateOptions = {};
        if(result != null && result.value) {
            stateOptions.cas = result.cas;
        }
        db.upsert("user::" + uid + "::state", stateDoc, stateOptions, function(error, result) {
            if(error) {
                return callback(error, null);
            }
            callback(null, stateBlock);
        });
    });
};

StateModel.getByUserIdAndName = function(uid, name, callback) {
    db.get("user::" + uid + "::state", function(error, result) {
        if(error) {
            if(error.code !== couchbase.errors.keyNotFound) {
                return callback(null, {});
            } else {
                return callback(error, null);
            }
        }
        if(!result.value.states[name]) {
            return callback({"status": "error", "message": "State does not exist"}, null);
        }
        callback(null, result.value.states[name]);
    });
};*/

module.exports = StateModel;