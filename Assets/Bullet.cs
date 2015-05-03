using UnityEngine;

public class Bullet : MonoBehaviour
{
    // INFORMATION FOR SHOOTING BULLETS
    float bulletSpeed = 1.6f; // Speed the bullet moves
    float secondsUntilDestroyBullet = 10f; // Number of seconds before bullet is automatically destroyed.
    float startTime;
    private bool shouldFire;

    void Update()
    {
        if (shouldFire) {
            // Move forward
            this.gameObject.transform.position += bulletSpeed * this.gameObject.transform.forward;

            // If the Bullet has existed as long as SecondsUntilDestroy, destroy it 
            if (Time.time - startTime >= secondsUntilDestroyBullet) {
                Destroy(this.gameObject);
            } 
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Remove the Bullet from the world 
        Destroy(this.gameObject);
    }

    public void Fire()
    {
        shouldFire = true;
    }

    internal void PreStart()
    {
        // Set the start time of the bullet:
        startTime = Time.time;
        shouldFire = false;
    }
}