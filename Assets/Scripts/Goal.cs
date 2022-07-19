using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    private bool isGrounded = false;
    public bool paused;
    public float groundDistance = 2;
    public LayerMask playerMask;
    public bool goalReached;
    public Player player;
    private int timer;

    void Update()
    {
        //Debug.Log(isGrounded);
        GroundCheck();
        if (isGrounded)
        {
            goalReached = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                paused = true;
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
            }
            else if (paused)
            {
                paused = false;
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

    }
    void GroundCheck()
    {
        isGrounded = Physics.CheckBox(transform.position, new Vector3(transform.localScale.x/2 - 1, groundDistance, transform.localScale.z/2 - 1), Quaternion.identity, playerMask);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, new Vector3(transform.localScale.x - 2, groundDistance * 2, transform.localScale.z - 2));
    }

    private void OnGUI()
    {
        if (paused)
        {
            GUI.skin.label.fontSize = 19;
            GUI.Label(new Rect(20, 20, 500, 500), "You paused the game");
            if (GUI.Button(new Rect(20, 50, 100, 50), "Restart"))
            {
                SceneManager.LoadScene("FirstScene");
                Time.timeScale = 1;
            }
        }
        if (goalReached && !paused)
        {
            Cursor.lockState = CursorLockMode.None;
            GUI.skin.label.fontSize = 19;
            GUI.Label(new Rect(20, 20, 500, 500), "You completed the game");
            if (GUI.Button(new Rect(20, 50, 100, 50), "Restart"))
            {
                SceneManager.LoadScene("FirstScene");
                Time.timeScale = 1;
            }
        }
        if (!goalReached && !paused)
        {
            GUI.skin.label.fontSize = 17;
            GUI.Label(new Rect(20, 20, 500, 500), "Walk = wasd");
            GUI.Label(new Rect(20, 35, 500, 500), "Jump = space");
            GUI.Label(new Rect(20, 50, 500, 500), "Dash = shift");
            GUI.Label(new Rect(20, 65, 500, 500), "Restart = R");
            GUI.Label(new Rect(20, 80, 500, 500), "Pause = Escape");
            GUI.Label(new Rect(20, 95, 500, 500), "Look around using the mouse");
        }

    }
}
