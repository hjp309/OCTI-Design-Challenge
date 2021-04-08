using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBoard : MonoBehaviour
{
    public AudioClip[] bounceSounds;
    private AudioSource bounceSource;
    public float force;

    private void Start()
    {
        bounceSource = GetComponent<AudioSource>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            bounceSource.clip = bounceSounds[Random.Range(0, bounceSounds.Length - 1)];
            bounceSource.Play();
            collision.rigidbody.AddForce(transform.forward * force, ForceMode.Impulse);
        }
    }
}
