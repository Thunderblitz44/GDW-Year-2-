using UnityEngine;

public class JumpAbility : Ability
{
    [SerializeField] float minJumpDist = 5f;
    [SerializeField] float maxJumpDist = 15f;
    [SerializeField] float jumpChargeSpeed = 7f;
    [SerializeField] float height = 10f;
    [SerializeField] GameObject falloffMarker;
    Transform jumpFalloff;
    bool chargeJump;
    float jumpChargeTime;
    float falloffDist;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (chargeJump)
        {
            jumpChargeTime += Time.deltaTime * jumpChargeSpeed;
            SetJumpFalloffPosition(jumpChargeTime);
        }
    }


    void SetJumpFalloffPosition(float time)
    {
        falloffDist = Mathf.Clamp(time + minJumpDist, minJumpDist, maxJumpDist);
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, StaticUtilities.GetCameraDir()), out hit, falloffDist))
        {
            jumpFalloff.position = new Vector3(hit.point.x, 0.01f, hit.point.z) - StaticUtilities.GetCameraDir();
        }
        else
        {
            jumpFalloff.position = new Vector3(transform.position.x, 0.01f, transform.position.z) + StaticUtilities.GetCameraDir() * falloffDist;
        }
    }

    public override void Part1()
    {
        if (chargeJump) return;
        chargeJump = true;

        jumpFalloff = Instantiate(falloffMarker, transform.position, Quaternion.identity).transform;
    }

    public override void Part2()
    {
        chargeJump = false;
        jumpChargeTime = 0;
        rb.AddForce(StaticUtilities.CalculateLaunchVelocity(transform.position, jumpFalloff.position, height) * rb.mass, ForceMode.Impulse);
        Destroy(jumpFalloff.gameObject, 0.5f);
    }
}
