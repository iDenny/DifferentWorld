using UnityEngine;

[DisallowMultipleComponent]
public class BeliefSystem : MonoBehaviour
{
    [Tooltip("Modifier applied to mood changes (can be positive or negative).")]
    public float beliefModifier = 0f;

    public float GetBeliefModifier()
    {
        return beliefModifier;
    }
}
