#pragma strict

function Update() {
	transform.localScale = (Input.GetButtonDown(name) ? 0.4 : 0.2) * Vector3.one;
	GetComponent.<Renderer>().material.color = Input.GetButton(name) ? Color.red : Color.white;
}
