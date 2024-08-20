#pragma strict

var showGravity = false;
var axisIndex = 0;

function Start() {
	Input.gyro.enabled = true;
}

function Update() {
	var input = showGravity ? Input.gyro.gravity : Input.gyro.userAcceleration;
	transform.localScale.x = input[axisIndex] * 5.0;
}
