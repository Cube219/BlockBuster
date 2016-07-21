var express = require("express");
var bodyParser = require("body-parser");
//var couchbase = require("couchbase");
var config = require("./config");
var app = express();

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

// Global declaration of the Couchbase server and bucket to be used
//module.exports.bucket = (new couchbase.Cluster(config.couchbase.server)).openBucket(config.couchbase.bucket);

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
});

// All endpoints to be used in this application
var routes = require("./routes/routes.js")(app);

var server = app.listen(3000, function () {
    console.log("Listening on port %s...", server.address().port);
});