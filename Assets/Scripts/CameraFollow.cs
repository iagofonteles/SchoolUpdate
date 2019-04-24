using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public Transform target;
	public float sensitivity;
	Vector3 lastRotation = Vector3.one;
	Vector3 rotation = Vector3.zero;
	public float distance, height;
	public float minAngle, maxAngle;
	bool lockAngle = true;

	public static Vector3 genericObjectEuler;
	public static Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0,0,0));
	public static bool updateRotation = true;

	void Update () {
        //distance -= Input.GetAxis("Mouse Wheel") * 8 * (lockAngle ? 0 : 1);
		//distance = Mathf.Clamp (distance, 3, 20);

		if(Input.GetKeyDown(KeyCode.Tab)) lockAngle = !lockAngle;
		if (!lockAngle) rotation += new Vector3 (-Input.GetAxis ("Mouse Y"), Input.GetAxis ("Mouse X")) * sensitivity;

		rotation.x = Mathf.Clamp (rotation.x, minAngle, maxAngle);
		transform.eulerAngles = rotation;
		transform.position = (target.position + Vector3.up * height) - transform.forward * distance;

		float il = Mathf.InverseLerp (minAngle, maxAngle, rotation.x);
		genericObjectEuler = new Vector3 (il * 60, rotation.y, 0);

		if (rotation != lastRotation) {
			lastRotation = rotation;
			updateRotation = true;
            rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(genericObjectEuler.x, rotation.y, 0));
        } else updateRotation = false;
	}
}
