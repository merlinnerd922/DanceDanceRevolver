using UnityEngine;

// A class representing a bullet that is fired by a person.
public class Bullet : MonoBehaviour
{
    // INFORMATION FOR SHOOTING BULLETS
    float bulletSpeed = 1.6f; // Speed the bullet moves
    float secondsUntilDestroyBullet = 10f; // Number of seconds before bullet is automatically destroyed.
    float startTime; // The time the bullet was created
    private bool shouldFire; // Indicates whether we should fire the bullet.

    // Update the bullet's trajectory (if it is meant to be fired).
    void Update()
    {
        if (shouldFire) {
            
            // Propel the bullet forwards.
            this.gameObject.transform.position += bulletSpeed * this.gameObject.transform.forward;

            // If the Bullet has existed as long as SecondsUntilDestroy, destroy it.
            if (Time.time - startTime >= secondsUntilDestroyBullet) {
                Destroy(this.gameObject);
            } 
        }
    }

    // Destroy this bullet if it collides into anything.
    void OnCollisionEnter(Collision collision)
    {
        // Remove the Bullet from the world 
        Destroy(this.gameObject);
    }

    // Fire this bullet (simply by setting the flag indicating if we should fire).
    public void Fire()
    {
        shouldFire = true;
    }

    // Initialize all variables associated with this bullet.
    internal void PreStart()
    {
        // Set the start time of the bullet:
        startTime = Time.time;
        shouldFire = false;
    }
}