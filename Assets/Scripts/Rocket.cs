using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 200f;
    [SerializeField] float mainThrust = 50f;
    [SerializeField] int currentLevel = 0;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending };
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // stop sound
        if (state == State.Alive)
        {
            Thrust();
            Rotate();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) // ignore collisions when dead
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly": // do nothing
                break;
            case "Finish":
                {
                    state = State.Transcending;
                    currentLevel += 1;
                    Invoke("LoadNextScene", 1f); // parameterize time.
                }
                break;
            default:
                {
                    state = State.Dying;
                    currentLevel = 0;
                    Invoke("LoadNextScene", 1f); // parameterize time.
                }
                break;
        }
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up * mainThrust);
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
    }
    
    private void Rotate()
    {
        rigidBody.freezeRotation = true; // takes manual control of rotation
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; // resumes physics control of rotation
    }

    private void LoadNextScene()
    {
        // Change if statement after figuring......
        if (currentLevel > 1)
        {
            currentLevel = 1;
        }
        SceneManager.LoadScene(currentLevel);
    }

}
