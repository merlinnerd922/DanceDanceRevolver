using ExtendSpace;
using UnityEngine;

public class MainCamera : MonoBehaviour {

    const float speed = 1f;
    bool goingClockwise;

	// Use this for initialization
	void Start () {
        goingClockwise = true;
	}
	
	// Rotate the camera around the screen.
	void Update () {

        if (goingClockwise) {
            if (this.gameObject.transform.rotation.ToVector3().y < 270f) {
                transform.RotateAround(Vector3.zero, Vector3.up, speed * Time.deltaTime);
            }
            else {
                goingClockwise = false;
                transform.RotateAround(Vector3.zero, Vector3.up, -speed * Time.deltaTime);
            }
        }
        else {
            if (this.gameObject.transform.rotation.ToVector3().y > 90f) {
                transform.RotateAround(Vector3.zero, Vector3.up, -speed * Time.deltaTime);
            }
            else {
                goingClockwise = true;
                transform.RotateAround(Vector3.zero, Vector3.up, speed * Time.deltaTime);
            }
        }



        
	}
}
