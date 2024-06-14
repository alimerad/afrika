using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Plane : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private MouseFlightController controller = null;

    [Header("Physics")]
    [Tooltip("Force to push plane forwards with")] public float thrust = 100f;
    [Tooltip("Pitch, Yaw, Roll")] public Vector3 turnTorque = new Vector3(90f, 25f, 45f);
    [Tooltip("Multiplier for all forces")] public float forceMult = 1000f;
    [Tooltip("Maximum thrust")] public float maxThrust = 500f;

    [Header("Autopilot")]
    [Tooltip("Sensitivity for autopilot flight.")] public float sensitivity = 5f;
    [Tooltip("Angle at which airplane banks fully into target.")] public float aggressiveTurnAngle = 10f;

    [Header("Input")]
    [SerializeField][Range(-1f, 1f)] private float pitch = 0f;
    [SerializeField][Range(-1f, 1f)] private float yaw = 0f;
    [SerializeField][Range(-1f, 1f)] private float roll = 0f;

    [Header("Wheels")]
    public WheelCollider[] wheelColliders;
    public Transform[] wheelMeshes;
    public float maxSteeringAngle = 30f;

    public float Pitch { set { pitch = Mathf.Clamp(value, -1f, 1f); } get { return pitch; } }
    public float Yaw { set { yaw = Mathf.Clamp(value, -1f, 1f); } get { return yaw; } }
    public float Roll { set { roll = Mathf.Clamp(value, -1f, 1f); } get { return roll; } }

    private Rigidbody rigid;

    private bool rollOverride = false;
    private bool pitchOverride = false;

    public bool playerIn = false;

    private float thrustIncreaseDuration = 0f;
    private float thrustDecreaseDuration = 0f;
    public float thrustChangeRate = 10f;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();

        if (controller == null)
            Debug.LogError(name + ": Plane - Missing reference to MouseFlightController!");
    }

    private void Update()
    {
        // Only allow control if the player is in the plane
        if (!playerIn)
            return;

        // When the player commands their own stick input, it should override what the
        // autopilot is trying to do.
        rollOverride = false;
        pitchOverride = false;

        float keyboardRoll = Input.GetAxis("Horizontal");
        if (Mathf.Abs(keyboardRoll) > .25f)
        {
            rollOverride = true;
        }

        float keyboardPitch = Input.GetAxis("Vertical");
        if (Mathf.Abs(keyboardPitch) > .25f)
        {
            pitchOverride = true;
            rollOverride = true;
        }

        // Calculate the autopilot stick inputs.
        float autoYaw = 0f;
        float autoPitch = 0f;
        float autoRoll = 0f;
        if (controller != null)
            RunAutopilot(controller.MouseAimPos, out autoYaw, out autoPitch, out autoRoll);

        // Use either keyboard or autopilot input.
        yaw = autoYaw;
        pitch = (pitchOverride) ? keyboardPitch : autoPitch;
        roll = (rollOverride) ? keyboardRoll : autoRoll;

        // Adjust thrust based on input
        AdjustThrust();

        // Update steering based on input
        UpdateSteering();
    }

    private void RunAutopilot(Vector3 flyTarget, out float yaw, out float pitch, out float roll)
    {
        var localFlyTarget = transform.InverseTransformPoint(flyTarget).normalized * sensitivity;
        var angleOffTarget = Vector3.Angle(transform.forward, flyTarget - transform.position);

        yaw = Mathf.Clamp(localFlyTarget.x, -1f, 1f);
        pitch = -Mathf.Clamp(localFlyTarget.y, -1f, 1f);

        var agressiveRoll = Mathf.Clamp(localFlyTarget.x, -1f, 1f);
        var wingsLevelRoll = transform.right.y;

        var wingsLevelInfluence = Mathf.InverseLerp(0f, aggressiveTurnAngle, angleOffTarget);
        roll = Mathf.Lerp(wingsLevelRoll, agressiveRoll, wingsLevelInfluence);
    }

    private void FixedUpdate()
    {
        // Only apply forces if the player is in the plane
        if (!playerIn)
            return;

        rigid.AddRelativeForce(Vector3.forward * thrust * forceMult, ForceMode.Force);
        rigid.AddRelativeTorque(new Vector3(turnTorque.x * pitch,
                                            turnTorque.y * yaw,
                                            -turnTorque.z * roll) * forceMult,
                                ForceMode.Force);
    }

    private void AdjustThrust()
    {
        if (Input.GetKey(KeyCode.R))
        {
            thrustIncreaseDuration += Time.deltaTime;
            thrustDecreaseDuration = 0f; // Reset decrease duration
            float thrustIncrease = thrustChangeRate * thrustIncreaseDuration;
            thrust = Mathf.Clamp(thrust + thrustIncrease * Time.deltaTime, 0f, maxThrust);
        }
        else
        {
            thrustIncreaseDuration = 0f; // Reset increase duration
        }

        if (Input.GetKey(KeyCode.F))
        {
            thrustDecreaseDuration += Time.deltaTime;
            thrustIncreaseDuration = 0f; // Reset increase duration
            float thrustDecrease = thrustChangeRate * thrustDecreaseDuration;
            thrust = Mathf.Clamp(thrust - thrustDecrease * Time.deltaTime, 0f, maxThrust);
        }
        else
        {
            thrustDecreaseDuration = 0f; // Reset decrease duration
        }
    }

    private void UpdateSteering()
    {
        float steerInput = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            steerInput = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            steerInput = 1f;
        }

        float steering = maxSteeringAngle * steerInput;

        // Assuming the front wheels are the first two colliders in the wheelColliders array
        wheelColliders[0].steerAngle = steering;
        wheelColliders[1].steerAngle = steering;
    }

    private void UpdateWheelPoses()
    {
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            UpdateWheelPose(wheelColliders[i], wheelMeshes[i]);
        }
    }

    private void UpdateWheelPose(WheelCollider collider, Transform mesh)
    {
        Vector3 pos;
        Quaternion quat;
        collider.GetWorldPose(out pos, out quat);
        mesh.position = pos;
        mesh.rotation = quat;
    }
}
