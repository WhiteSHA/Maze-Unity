/*
 * written by Arsen Sargsyan 2021
 */

using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

// basic WASD-style movement control
public class FpsMovement : MonoBehaviour
{
    [SerializeField] private Camera headCam;
    [SerializeField] private GameObject torch;
    [SerializeField] private GameObject exitGameDialog;

    public float speed = 6.0f;
    public float gravity = -9.8f;

    public float sensitivityHor = 9.0f;
    public float sensitivityVert = 9.0f;

    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;

    private float rotationVert = 0;

    private DateTime startTime;
    private int timeLimit = 1;

    private CharacterController charController;

    void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    void Update()
    {
        MoveCharacter();
        RotateCharacter();
        RotateCamera();
        SpecialCommands();
    }

    private void MoveCharacter()
    {
        float deltaX = Input.GetAxis("Horizontal") * speed;
        float deltaZ = Input.GetAxis("Vertical") * speed;

        Vector3 movement = new Vector3(deltaX, 0, deltaZ);
        movement = Vector3.ClampMagnitude(movement, speed);

        movement.y = gravity;
        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);

        charController.Move(movement);
    }

    private void RotateCharacter()
    {
        transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityHor, 0);
    }

    private void RotateCamera()
    {
        rotationVert -= Input.GetAxis("Mouse Y") * sensitivityVert;
        rotationVert = Mathf.Clamp(rotationVert, minimumVert, maximumVert);

        headCam.transform.localEulerAngles = new Vector3(
            rotationVert, headCam.transform.localEulerAngles.y, 0
        );
    }

    private void SpecialCommands()
    {
        if (Input.GetKeyDown((KeyCode)116))
        {
#pragma warning disable CS0618 // Type or member is obsolete
            torch.SetActive(!torch.active);
#pragma warning restore CS0618 // Type or member is obsolete
        }
        else if(Input.GetKeyDown((KeyCode)27))
        {
#pragma warning disable CS0618 // Type or member is obsolete
            exitGameDialog.SetActive(!exitGameDialog.active);
#pragma warning restore CS0618 // Type or member is obsolete
        }
        else if(Input.GetKeyDown((KeyCode)304))
        {
            speed = 12.0f;
            startTime = DateTime.Now;
        }
        else if(Input.GetKeyUp((KeyCode)304))
        {
            speed = 6.0f;
        }

        if((int)(DateTime.Now - startTime).TotalSeconds > timeLimit)
        {
            speed = 6.0f;
        }
    }
} 