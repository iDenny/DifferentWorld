using UnityEngine;

[RequireComponent(typeof(Character))]
public class FriendlyNPC : MonoBehaviour
{
    public float WaveDistance = 3f;
    public float InteractionCooldown = 10f;

    private Character character;
    private float lastInteractTime = -999f;
    private Animator animator;

    public void Setup(Character c)
    {
        character = c;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (character == null) return;
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float dst = Vector3.Distance(transform.position, player.transform.position);
        if (dst <= WaveDistance && Time.time - lastInteractTime >= InteractionCooldown)
        {
            // Face player and wave
            Vector3 dir = (player.transform.position - transform.position);
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
