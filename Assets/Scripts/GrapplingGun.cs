using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;
    public float maxDistance = 100f;
    private SpringJoint joint;

    [Header("Zip Settings")]
    public float zipSpeed = 10f;
    public float zipCooldown = 5f;
    private float lastZipTime = -Mathf.Infinity;
    private bool isZipping = false;

    [Header("Grapple Settings")]
    public float grappleCooldown = 5f;
    private float lastGrappleTime = -Mathf.Infinity;

    private Vector3 currentGrapplePosition;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        // Left click to grapple
        if (Input.GetMouseButtonDown(0) && Time.time >= lastGrappleTime + grappleCooldown)
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }

        // Right click hold to zip
        if (Input.GetMouseButtonDown(1) && Time.time >= lastZipTime + zipCooldown)
        {
            StartZip();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StopZip();
        }

        // Perform zipping movement
        if (isZipping)
        {
            Vector3 direction = (grapplePoint - player.position).normalized;
            float distance = Vector3.Distance(player.position, grapplePoint);

            if (distance < 2f)
            {
                StopZip();
            }
            else
            {
                Rigidbody rb = player.GetComponent<Rigidbody>();
                rb.linearVelocity = direction * zipSpeed;
            }
        }
    }

    void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        if (TryGetGrapplePoint(out Vector3 point))
        {
            lastGrappleTime = Time.time;
            grapplePoint = point;

            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;

            GameAudioManager.Instance?.PlayGrapple();


        }
    }

    void StopGrapple()
    {
        lr.positionCount = 0;
        if (joint != null)
        {
            Destroy(joint);
        }
        isZipping = false;
    }

    void StartZip()
    {
        if (TryGetGrapplePoint(out Vector3 point))
        {
            lastZipTime = Time.time;
            grapplePoint = point;
            isZipping = true;

            if (joint != null)
                Destroy(joint);

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;

            GameAudioManager.Instance?.PlayZip();
        }
    }

    void StopZip()
    {
        isZipping = false;
        lr.positionCount = 0;
    }

    void DrawRope()
    {
        if (!IsGrappling()) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    private bool TryGetGrapplePoint(out Vector3 point)
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            point = hit.point;
            return true;
        }

        point = Vector3.zero;
        return false;
    }

    public bool IsGrappling()
    {
        return joint != null || isZipping;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}
