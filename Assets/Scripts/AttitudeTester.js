#pragma strict

function Update() {
    // Converts rotation on right-hand coordinate system to left hand one.
    var rha = Input.gyro.attitude;
    var lha = Quaternion(rha.x, rha.y, rha.z, rha.w);
    transform.localRotation = lha;
}
