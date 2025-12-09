using UnityEngine;

[RequireComponent(typeof(Character))]
public class FriendlyNPC : MonoBehaviour
{
    public float WaveDistance = 3f;
    public float InteractionCooldown = 10f;

    private Character character;
    private float lastInteractTime = -999f;
    private Animator animator;
    private Transform playerTransform;

    public void Setup(Character c)
    {
        character = c;
        animator = GetComponent<Animator>();
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    private void Start()
    {
        if (playerTransform == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }
    }

    private void Update()
    {
        if (character == null) return;
        if (playerTransform == null) return;

        float dst = Vector3.Distance(transform.position, playerTransform.position);
        if (dst <= WaveDistance && Time.time - lastInteractTime >= InteractionCooldown)
        {
            // Face player and wave
            Vector3 dir = (playerTransform.position - transform.position);
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir.normalized), 10f * Time.deltaTime);
            }

            if (animator != null)
            {
                animator.SetTrigger("Wave");
            }

            lastInteractTime = Time.time;
        }
    }
}
