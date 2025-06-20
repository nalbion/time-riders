using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;


public class MotorbikeController : MonoBehaviour {
    public WheelCollider WColForward;
    public WheelCollider WColBack;

    public Transform wheelF;
    public Transform wheelB;
    [SerializeField]
    public GameObject handles;
    [SerializeField]
    public GameObject RearMudGuard;
    public Vector3 RearMudGuardSusOffset;
    [Tooltip("Lower values mean higher sensitivity")]

    public float preventGlitchAngle = 40;

    [Tooltip("Experimental Feature : Only for controlled low speeds")]
    public bool canArtificialStoppie = false;
    [Range(0.1f, 1f)]
    public float stoppieAmount = 0.3f;
    [HideInInspector]
    public bool fallen = false;

    public float maxSteerAngle = 45;
    public float maxMotorTorque = 500;
    [Tooltip("Adds more speed. Inaccurate from a physics standpoint. Arcade Feature. Values too high will break the realism of the system and make the bike glitch badly.")]
    public float ArtificialAcceleration = 1000f;
    [Tooltip("Adds more braking power. Inaccurate from a physics standpoint. Arcade Feature. Values too high will break the realism of the system, but it will definitely apply hard brakes")]
    [Range(0, 1)]

    public float ArtificialBrake = 0;
    public float maxForwardBrake = 400;
    public float maxBackBrake = 400;

    public float wheelRadius = 0.7f;

    public float steerSensivity = 30;
    public float controlAngle = 25;
    public float controlOmega = 30;

    public float lowSpeed = 8;
    public float highSpeed = 25;

    private WheelData[] wheels;

    private Transform thisTransform;
    public Vector3 com;
    Rigidbody rb;
    float startSteerSensitivity;


    public int currentGear = 1;
    public float revValue;
    float initialMotorTorque;
    public GameObject Rider;
    public GameObject RagdollAnimation;
    public GameObject Ragdoll;
    bool HardHit;
    GameObject tempRagdollClone, tempAnimRiderClone;

    [HideInInspector]
    public Vector3 collisionRelativeVelocity;

    float turnAngle;


    public class WheelData {
        public WheelData(Transform transform, WheelCollider collider) {
            wheelTransform = transform;
            wheelCollider = collider;
            wheelStartPos = transform.transform.localPosition;
        }

        public Transform wheelTransform;
        public WheelCollider wheelCollider;
        public Vector3 wheelStartPos;
        public float rotation = 0f;
    }

    public struct MotorbikeInput {
        public float steer;
        public float acceleration;
        public float brakeForward;
        public float brakeBack;
    }

    void Start() {
        wheels = new WheelData[2];
        wheels[0] = new WheelData(wheelF, WColForward);
        wheels[1] = new WheelData(wheelB, WColBack);

        Input.gyro.enabled = true;

        thisTransform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = com;
        startSteerSensitivity = steerSensivity;
        initialMotorTorque = maxMotorTorque;
    }

    // --- Touch drag state for mobile controls ---
    private Vector2 touchStartPos;
    private bool isDragging = false;
    private float lastTouchTime = 0f;
    private float dragSteer = 0f;
    private float dragThrottle = 0f;

    void FixedUpdate() {
        turnAngle = transform.eulerAngles.z;
        if (transform.eulerAngles.z > 180)
            turnAngle = transform.eulerAngles.z - 360;

        uprightCheck();
        if (!fallen)
        {
            uprightForce();
            var input = new MotorbikeInput();

            if (Application.isMobilePlatform || Input.touchSupported) {
                // --- Touch drag controls ---
                if (Input.touchCount > 0) {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == UnityEngine.TouchPhase.Began) {
                        touchStartPos = touch.position;
                        isDragging = true;
                    } else if (touch.phase == UnityEngine.TouchPhase.Moved || touch.phase == UnityEngine.TouchPhase.Stationary) {
                        if (isDragging) {
                            Vector2 delta = touch.position - touchStartPos;
                            // Sensitivity can be tweaked for best feel
                            dragSteer = Mathf.Clamp(delta.x / (Screen.width * 0.3f), -1f, 1f);
                            dragThrottle = Mathf.Clamp(delta.y / (Screen.height * 0.3f), -1f, 1f);
                            input.steer = dragSteer;
                            if (dragThrottle > 0.1f) {
                                input.acceleration = dragThrottle;
                            } else if (dragThrottle < -0.1f) {
                                input.brakeForward = -dragThrottle;
                            }
                        }
                    } else if (touch.phase == UnityEngine.TouchPhase.Ended || touch.phase == UnityEngine.TouchPhase.Canceled) {
                        isDragging = false;
                        dragSteer = 0f;
                        dragThrottle = 0f;
                    }
                    lastTouchTime = Time.time;
                }
                // If not dragging, fallback to tilt controls
                else {
                    float tiltX = Input.gyro.attitude.x; // left/right
                    float tiltY = Input.gyro.attitude.y; // up = forward (landscape mode)
                    // float tiltX = Input.acceleration.x; // left/right
                    // float tiltY = -Input.acceleration.y; // up = forward (landscape mode)
                    input.steer = Mathf.Clamp(tiltX * 2f, -1f, 1f); // Sensitivity tweakable
                    if (tiltY > 0.1f) {
                        input.acceleration = Mathf.Clamp01(tiltY);
                    } else if (tiltY < -0.1f) {
                        input.brakeForward = Mathf.Clamp01(-tiltY);
                    }
                }
            } else {
                // Keyboard/gamepad controls
                if ((Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)) input.acceleration = 1;
                if ((Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)) input.steer += 1;
                if ((Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)) input.steer -= 1;

                if ((Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)) {
                    input.brakeBack = 0.3f;
                    input.brakeForward = 0.5f;
                }
                if (Keyboard.current.spaceKey.isPressed) {
                    input.brakeForward = 1f;
                }
            }

            motoMove(motoControl(input));
            steerHelper();
            steerHandles();
        }
        updateWheels();
        RearMudGuardSuspension();
        CalcGear();
        if (canArtificialStoppie) // for natural stoppie increase forward brake to 50000 and Front Wheel collider forward friction to 5. Around those values a natural stoppie can be performed.
            CalcStoppie(); //Requires prevent falling

        if (Keyboard.current.rKey.wasPressedThisFrame && fallen == true)
        {
            Reset();
        }
    }

    void Awake() {
        Time.timeScale = 1.15f; //Makes simulation movement more agile. You can delete this line if it interferes with your project settings.
    }

    private void Reset() {
        Transform t = GetComponent<Transform>();
        t.position = t.position + new Vector3(0, 0.1f, 0);
        t.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
        rb.angularDamping = 100;
        rb.centerOfMass = com;
        HardHit = false;
        fallen = false;
        Destroy(tempRagdollClone);
        Destroy(tempAnimRiderClone);
        Rider.SetActive(true);
    }

    private Vector3 prevPos = new Vector3();
    private float prevAngle = 0;
    private float prevOmega = 0;
    private float speedVal = 0;
    private float prevSteer = 0f;

    private MotorbikeInput motoControl(MotorbikeInput input) {
        var posNow = thisTransform.position;
        var speed = (posNow - prevPos) / Time.fixedDeltaTime;
        prevPos = posNow;

        speedVal = speed.magnitude;
        var moveForward = speed.normalized;

        var angle = Vector3.Dot(moveForward, Vector3.Cross(thisTransform.up, new Vector3(0, 1, 0)));
        var omega = (angle - prevAngle) / Time.fixedDeltaTime;
        prevAngle = angle;
        prevOmega = omega;


        if (speedVal < lowSpeed) {
            float t = speedVal / lowSpeed;
            input.steer *= t * t;
            omega *= t * t;
            angle = angle * (2 - t);
            input.acceleration += Mathf.Abs(angle) * 3 * (1 - t);
        }

        if (speedVal > highSpeed) {
            float t = speedVal / highSpeed;
            if (omega * angle < 0f) {
                omega *= t;
            }
        }
        input.steer *= (1 - 2.3f * angle * angle);
        input.steer = 1f / (speed.sqrMagnitude + 1f) * (input.steer * steerSensivity + angle * controlAngle + omega * controlOmega);
        float steerDelta = 10 * Time.fixedDeltaTime;
        input.steer = Mathf.Clamp(input.steer, prevSteer - steerDelta, prevSteer + steerDelta);
        prevSteer = input.steer;

        return input;
    }

    private void uprightForce() {
        rb.angularDamping -= 100 * Time.deltaTime;
        rb.angularDamping = Mathf.Clamp(rb.angularDamping, 0.1f, 100);

        if (speedVal < 1 && !Keyboard.current.wKey.isPressed)
        {
            // var rot = Quaternion.FromToRotation(transform.up, Vector3.up);
            // rb.AddTorque(new Vector3(rot.x, rot.y, rot.z)* 10 , ForceMode.Acceleration);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        else {
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    void OnCollisionEnter(Collision collision) {
        collisionRelativeVelocity = collision.relativeVelocity;
        if (collision.relativeVelocity.magnitude > 30)
            HardHit = true;
    }

    public void uprightCheck() {
        if ((Mathf.Abs(turnAngle) > preventGlitchAngle || Keyboard.current.fKey.wasPressedThisFrame || HardHit == true) && fallen == false) {
            Rider.SetActive(false);
            tempRagdollClone = Instantiate(Ragdoll);
            tempAnimRiderClone = Instantiate(RagdollAnimation);
            rb.centerOfMass = new Vector3(0, 0.5f, 0);
            fallen = true;
        }
    }


    private void motoMove(MotorbikeInput input) {
        if (speedVal > 1)
            WColForward.steerAngle = Mathf.Clamp(input.steer, -1, 1) * maxSteerAngle;
        else
            WColForward.steerAngle = Mathf.Clamp(input.steer, -speedVal, speedVal);

        WColForward.brakeTorque = maxForwardBrake * input.brakeForward;
        WColBack.brakeTorque = maxBackBrake * input.brakeBack;
        WColBack.motorTorque = maxMotorTorque * input.acceleration;
        if (speedVal < highSpeed)
            rb.AddForce(transform.forward * ArtificialAcceleration * input.acceleration);

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x * (1 - ArtificialBrake / 10), rb.linearVelocity.y, rb.linearVelocity.z * (1 - ArtificialBrake / 10));
    }

    private void updateWheels() {
        float delta = Time.fixedDeltaTime;

        foreach (WheelData w in wheels) {
            WheelHit hit;

            Vector3 localPos = w.wheelTransform.localPosition;
            if (w.wheelCollider.GetGroundHit(out hit)) {
                localPos.y -= Vector3.Dot(w.wheelTransform.position - hit.point, transform.up) - wheelRadius;
                w.wheelTransform.localPosition = localPos;
            }
            else {
                localPos.y = w.wheelStartPos.y;
            }

            w.rotation = Mathf.Repeat(w.rotation + delta * w.wheelCollider.rpm * 360.0f / 60.0f, 360f);
            w.wheelTransform.localRotation = Quaternion.Euler(w.rotation, Mathf.Lerp(w.wheelTransform.localRotation.y, w.wheelCollider.steerAngle, Time.deltaTime * 10), 0);

        }
    }

    private void steerHandles() {
        handles.transform.localRotation = Quaternion.Euler(0, Mathf.Lerp(handles.transform.localRotation.y, WColForward.steerAngle, Time.deltaTime * 10), 0);
    }

    private void RearMudGuardSuspension() {
        WheelHit hit;
        if (WColBack.GetGroundHit(out hit))
            RearMudGuard.transform.rotation = Quaternion.LookRotation(transform.position - wheelB.transform.position - RearMudGuardSusOffset, transform.forward);
    }

    private void CalcStoppie() {
        var stoppieAngle = transform.eulerAngles.x;
        if (transform.eulerAngles.x > 180)
            stoppieAngle = transform.eulerAngles.x - 360;
        if ((Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed || Keyboard.current.spaceKey.isPressed))
            com.z += (speedVal * Time.deltaTime) / 5;
        else
            com.z -= Time.deltaTime * 100;
        if (com.z < 0 || stoppieAngle > 5 + speedVal)
        {
            com.z = 0;
            if (stoppieAngle > 50)
                com.z -= stoppieAngle / 10;
        }

        else if (com.z > stoppieAmount)
            com.z = stoppieAmount;
    }

    void steerHelper() {
        steerSensivity = Mathf.Clamp(startSteerSensitivity - Mathf.Abs(turnAngle) * 0.9f, 10, startSteerSensitivity);
        controlAngle = Mathf.Clamp(controlAngle, 48, 65);
        if (Keyboard.current.anyKey.isPressed)
            controlAngle -= 1;
        else
            controlAngle += 1;

        if (turnAngle > 42 || turnAngle < -42)
            controlAngle += 2;


        if ((turnAngle > 10 && (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed))) {
            rb.AddTorque(-transform.forward * 0.1f * turnAngle, ForceMode.Acceleration);
        }

        else if (turnAngle > 20 && (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)) {
            rb.AddTorque(-rb.angularVelocity * 2, ForceMode.Acceleration);
        }
        else if (turnAngle < -10 && (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)) {
            rb.AddTorque(transform.forward * 0.1f * -turnAngle, ForceMode.Acceleration);
        }
        else if (turnAngle < -20 && (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)) {
            rb.AddTorque(-rb.angularVelocity * 2, ForceMode.Acceleration);
        }
        //Sets Sideways friction with speed gradations
        if (speedVal < 10)
            SetWheelFriction(1.5f);
        else if(speedVal < 20 && speedVal > 10)
            SetWheelFriction(2);
        else if(speedVal < 30 && speedVal > 20)
            SetWheelFriction(2.5f);
        else if(speedVal < 40 && speedVal > 20)
            SetWheelFriction(3);
        else
            SetWheelFriction(3.5f);
    }

    void SetWheelFriction(float friction) {
        WheelFrictionCurve wfc;
        wfc = WColBack.sidewaysFriction;
        wfc.stiffness = friction;
        WColBack.sidewaysFriction = wfc;
        WColForward.sidewaysFriction = wfc;
    }
    
    void CalcGear() {
        var prevGear = currentGear;
        currentGear = Mathf.FloorToInt(speedVal / 13);
        if (currentGear != prevGear)
            StartCoroutine(MotorDisengage());
        revValue = speedVal % 13 / 13;
    }

    IEnumerator MotorDisengage() {
        maxMotorTorque = 0;
        yield return new WaitForSeconds(0.1f);
        maxMotorTorque = initialMotorTorque;
    }
}
