using ExtendSpace;
using UnityEngine;

// A class implementing functions of the main camera.
public class MainCamera : MonoBehaviour {

    const float speed = 1f; // Speed of camera rotation (around the origin)
    bool goingClockwise; // Indicates whether we are moving forwards or backwards.

	// Use this for initialization
	void Start () {

        // Randomly determine whether we are moving clockwise or counterclockwise.
        goingClockwise = Helper.GetRandom(true, false);

	}
	
	// Rotate the camera around the screen.
	void Update () {

        // While we are moving clockwise
        if (goingClockwise) {

            // Continue to rotate clockwise.
            if (this.gameObject.transform.rotation.ToVector3().y < 270f) {
                transform.RotateAround(Vector3.zero, Vector3.up, speed * Time.deltaTime);
            }

            // If we are about to go clockwise over 270 degrees, rotate back in the other direction.
            else {
                goingClockwise = false;
                transform.RotateAround(Vector3.zero, Vector3.up, -speed * Time.deltaTime);
            }
        }

        // While we are moving counterclockwise
        else {

            // Continue to rotate counterclockwise.
            if (this.gameObject.transform.rotation.ToVector3().y > 90f) {
                transform.RotateAround(Vector3.zero, Vector3.up, -speed * Time.deltaTime);
            }

            // If we are about to go counterclockwise below 90 degrees, rotate back in the other direction.
            else {
                goingClockwise = true;
                transform.RotateAround(Vector3.zero, Vector3.up, speed * Time.deltaTime);
            }
        }

	}

}
