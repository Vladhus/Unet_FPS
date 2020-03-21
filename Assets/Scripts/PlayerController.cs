using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float mouseSensa=3f;

    [SerializeField]
    private float thrusterForce = 1000f;

    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;

    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f;
    private float thrusterFuelAmount = 1f;

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }


    [SerializeField]
    private LayerMask enviromentMask;

    [Header("Spring Settings:")]
    
    [SerializeField]
    private float jointSpring = 20f;    
    [SerializeField]
    private float jointMaxForce = 40f;

    //COMPONENT CESHING
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;


    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();

        SetJointSettings(jointSpring);
    }

    void Update()
    {

        if (HusStop.IsHus)
        {
            return;
        }
        RaycastHit _hit;
        if (Physics.Raycast(transform.position,Vector3.down,out _hit,100f,enviromentMask))
        {
            joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }
        float xMov = Input.GetAxis("Horizontal");
        float zMov = Input.GetAxis("Vertical");

        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;

        Vector3 _velocity = (moveHorizontal + moveVertical) * speed;

        animator.SetFloat("ForwardVelocity", zMov); 

        motor.Move(_velocity);


        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3(0f, yRot, 0f) * mouseSensa;

        motor.Rotate(_rotation);


        float xRot = Input.GetAxisRaw("Mouse Y");

        float camera_rotationX = xRot * mouseSensa;

        motor.RotateCamera( camera_rotationX);

        Vector3 _thrustedForce = Vector3.zero;

        if (Input.GetButton("Jump") && thrusterFuelAmount > 0f) 
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if (thrusterFuelAmount >= 0.01f)
            {
                _thrustedForce = Vector3.up * thrusterForce;
                SetJointSettings(0f);
            }
            
        }
        else
        {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
            SetJointSettings(jointSpring); 
        }
        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

        motor.ApplyThruster(_thrustedForce);
    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive { positionSpring = _jointSpring, maximumForce = jointMaxForce };
    }
}
