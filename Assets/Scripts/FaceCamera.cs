using UnityEngine;

public class FaceCamera : MonoBehaviour {
void Update() {
    transform.eulerAngles = CameraFollow.genericObjectEuler;
	}
}
