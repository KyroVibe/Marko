using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController ActiveController;

    public GameObject Camera;
    public JumpCollider JumpColl;
    public bool ShowCursor;

    [Header("Look")]
    public float LookSensitivityHoriz;
    public float LookSensitivityVerti;
    public float MouseAcceleration;

    [Header("Movement")]
    public float MaxSpeed;
    public float Acceleration;
    public float AirAccelerationMod;

    [Header("Jetpack"), SerializeField]
    public float MaxGas;
    public float GasRefillDelay;
    public float GasRefillRate;
    public float MaxUpwardVelocity;
    public float UpwardAcceleration;
    public float Gas { get; private set; }

    private float meshMin;
    private float lastUsed;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        ActiveController = this;
        rb = GetComponent<Rigidbody>();
        Gas = 0.0f;
        lastUsed = Time.realtimeSinceStartup;

        meshMin = GetComponent<MeshFilter>().mesh.bounds.min.y;
        Debug.Log($"Min: {meshMin}");
        EvaluateCursor();
    }

    private void OnDestroy()
    {
        if (ActiveController.gameObject.name == transform.name)
            ActiveController = null;
    }

    Vector3 targetVel, diff;
    Vector2 rot = new Vector2(0, 0), target = new Vector2(0, 0), mosDiff;
    float mosX, mosY;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ShowCursor = !ShowCursor;
            EvaluateCursor();
        }

        #region Camera
        mosX = Input.GetAxis("Mouse X") * LookSensitivityVerti;
        mosY = -Input.GetAxis("Mouse Y") * LookSensitivityHoriz;

        rot += new Vector2(mosY, mosX);
        rot.x = Mathf.Clamp(rot.x, -80.0f, 80.0f);
        Camera.transform.localRotation = Quaternion.Euler(rot.x, 0.0f, 0.0f);
        transform.rotation = Quaternion.Euler(0.0f, rot.y, 0.0f);

        // int mosXSign = mosX == 0 ? 1 : (int)mosX / Mathf.Abs((int)mosX);
        // int mosYSign = mosY == 0 ? 1 : (int)mosY / Mathf.Abs((int)mosY);

        /*rotX += mosY > 0 ? Mathf.Min(targetX - rotX, MouseAcceleration * Time.deltaTime)
            : Mathf.Max(targetX - rotX, -MouseAcceleration * Time.deltaTime);
        rotY += mosX > 0 ? Mathf.Min(targetY - rotY, MouseAcceleration * Time.deltaTime)
            : Mathf.Max(targetY - rotY, -MouseAcceleration * Time.deltaTime);
        targetX = Mathf.Clamp(targetX, -80.0f, 80.0f);

        Camera.transform.localRotation = Quaternion.Euler(rotX, 0.0f, 0.0f);
        transform.rotation = Quaternion.Euler(0.0f, rotY, 0.0f);*/
        #endregion

        #region Movement

        bool onGround = JumpColl.Triggered; // Physics.Raycast(new Ray(transform.position, -transform.up), Mathf.Abs(meshMin));
        if (onGround)
            Debug.Log("On Ground");
        else
            Debug.Log("In Air");

        targetVel = ((transform.right * Input.GetAxisRaw("Horizontal")) + (transform.forward * Input.GetAxisRaw("Vertical"))).normalized * MaxSpeed + new Vector3(0.0f, rb.velocity.y, 0.0f);
        // targetVel = new Vector3(Input.GetAxisRaw("Horizontal") * MaxSpeed,
        //     rb.velocity.y, Input.GetAxisRaw("Vertical") * MaxSpeed);
        diff = targetVel - rb.velocity;

        rb.velocity +=
            Mathf.Clamp(
                Acceleration * Time.deltaTime * (onGround ? 1.0f : AirAccelerationMod),
                -Mathf.Abs(diff.magnitude),
                Mathf.Abs(diff.magnitude))
            * (targetVel - rb.velocity).normalized;
        #endregion

        #region Jetpack
        if (Input.GetKey(KeyCode.Space))
        {
            if (Gas > 0)
            {
                Gas -= Time.deltaTime;
                rb.velocity = new Vector3(
                    rb.velocity.x,
                    Mathf.Clamp(
                        rb.velocity.y + (Time.deltaTime * UpwardAcceleration),
                        rb.velocity.y,
                        Mathf.Max(MaxUpwardVelocity, rb.velocity.y)
                    ),
                    rb.velocity.z);
            }
            lastUsed = Time.realtimeSinceStartup;
        } else if (Time.realtimeSinceStartup > lastUsed + GasRefillDelay)
        {
            Gas += GasRefillRate * Time.deltaTime;
        }
        Gas = Mathf.Clamp(Gas, 0, MaxGas);
        #endregion
    }

    void EvaluateCursor()
    {
        if (ShowCursor)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
