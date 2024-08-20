#pragma strict

var xAxisName = "Horizontal";
var yAxisName = "Vertical";

private var cubeRenderer : Renderer;

function Start() {
	cubeRenderer = GetComponentInChildren(Renderer);
}

function Update() {
	var x = Input.GetAxis(xAxisName);
	var y = Input.GetAxis(yAxisName);
	var length = Mathf.Sqrt(x * x + y * y);
	if (length > 0.01) {
		transform.localRotation = Quaternion.FromToRotation(Vector3.right, Vector3(x, y, 0));
		transform.localScale = Vector3(length * 5, 1, 1);
		cubeRenderer.enabled = true;
	} else {
		// Hide the cube if the vector is too short.
		cubeRenderer.enabled = false;
	}
}