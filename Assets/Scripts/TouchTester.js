#pragma strict

var texture : Texture2D;
var externalCamera : Camera;
var screenWidth : float;
var screenHeight : float;
var useMultiTouch : boolean;

function Start() {
    externalCamera = GameObject.Find("External Camera").GetComponent(Camera);
    screenWidth = UnityEngine.WiiU.Core.GetScreenWidth(externalCamera.targetDisplay);
    screenHeight = UnityEngine.WiiU.Core.GetScreenHeight(externalCamera.targetDisplay);

    useMultiTouch = false;
}

function OnGUI() {
    GUI.color = Color(1, 1, 1, 0.6);
    var size = screenWidth / 16;

    if(!useMultiTouch) {
        // Touch input with mouse emulation.
        if (Input.GetMouseButton(0)) {
            var rect = Rect(Input.mousePosition.x - size / 2, (screenHeight-Input.mousePosition.y) - size / 2, size, size);
            GUI.DrawTexture(rect, texture);
        }
    } else {
        // Touch input via multi-touch API.
        for (var touch in Input.touches) {
            var multi_rect = Rect(touch.position.x - size / 2, touch.position.y - size / 2, size, size);
            GUI.DrawTexture(multi_rect, texture);
        }
    }
}
