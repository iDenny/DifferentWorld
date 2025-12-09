using UnityEngine;

/// <summary>
/// Simple lock component placed on potential targets so NPCs can claim a target
/// and avoid multiple NPCs engaging the same target simultaneously.
/// </summary>
public class TargetLock : MonoBehaviour
{
    public GameObject Engager;

    public bool IsClaimed => Engager != null;

    public void Claim(GameObject engager)
    {
        Engager = engager;
    }

    public void Release(GameObject engager)
    {
        if (Engager == engager)
            Engager = null;
    }
}
