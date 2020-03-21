﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    
    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float CameraRotationX = 0f;
    private float currentCameraRotationX = 0f;
    private Vector3 thrustedForce = Vector3.zero;

    [SerializeField]
    private float cameraRotationLimit = 85f;

    private Rigidbody rb;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    //Get a move vector from playerController
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }


    //Get a rotation Camera vector from playerController
    public void RotateCamera(float camera_rotationX)
    {
        CameraRotationX = camera_rotationX;
    }

    //Get a rotation vector from playerController
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    //Get a force vector from playerController
    public void ApplyThruster(Vector3 _thrustedForce)
    {
        thrustedForce = _thrustedForce;
    }
    void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }


    //DoMove
    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
        if (thrustedForce != Vector3.zero)
        {
            rb.AddForce(thrustedForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    //DoRotation For Camera and Character
    void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if (cam != null)
        {
            currentCameraRotationX -= CameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }

    }
}
