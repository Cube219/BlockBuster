var express = require("express");
var bodyParser = require("body-parser");
var config = require("./config");
var app = express();

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

// DB하고 연결
var mysql = require('mysql');

var db_connection = mysql.createConnection({
    host: 'localhost',
	port:3306,
	user:'root',
	password:'12345',
	database:'block_buster'
});
module.exports.db_connection = db_connection;

db_connection.connect(function(err){
	if(err){
		console.error('Cannot connect db!');
		throw err;
	}
	console.log('Success connect db');

	// DB Connection이 끊기지 않도록 주기적으로 쿼리를 보내는 함수 (임시방편, 이건 차후에 수정...)
	setInterval(queryOnDBForever, 10000);
});

// Log DB하고 연결
var logdb_connection = mysql.createConnection({
    host:'localhost',
	port:3306,
	user:'root',
	password:'12345',
	database:'block_buster_log'
});
module.exports.logdb_connection = logdb_connection;

logdb_connection.connect(function(err){
	if(err){
		console.error('Cannot connect logdb!');
		throw err;
	}
	console.log('Success connect logdb');

	// DB Connection이 끊기지 않도록 주기적으로 쿼리를 보내는 함수 (임시방편, 이건 차후에 수정...)
	setInterval(queryOnLogDBForever, 10000);
});

// All endpoints to be used in this application
var routes = require("./routes/routes.js")(app);

var server = app.listen(3000, function () {
    console.log("Listening on port %s...", server.address().port);
});

// DB Connection이 끊기지 않도록 주기적으로 쿼리를 보내는 함수 (임시방편, 이건 차후에 수정...)
var queryOnDBForever = function()
{
	db_connection.query("SELECT 1", function(err, res) { });
}

var queryOnLogDBForever = function()
{
	logdb_connection.query("SELECT 1", function(err, res) { });
}