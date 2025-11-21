using UnityEngine;

/// <summary>
/// Represents a single need for a character, such as hunger or rest.
/// Needs decay over time and can be fulfilled by actions (eating, sleeping, etc.).
/// </summary>
public class Need
{
    public NeedType Type { get; private set; }
    public float CurrentValue { get; private set; }
    public float MaxValue { get; private set; }
    public float DecayRate { get; private set; }

    /// <summary>
    /// Create a new need with initial parameters.
    /// </summary>
    /// <param name="type">Type of need.</param>
    /// <param name="maxValue">Maximum value for the need.</param>
    /// <param name="decayRate">Rate at which the need decreases per second.</param>
    public Need(NeedType type, float maxValue, float decayRate)
    {
        Type = type;
        MaxValue = maxValue;
        CurrentValue = maxValue;
        DecayRate = decayRate;
    }

    /// <summary>
    /// Update the need, decreasing it by the decay rate. Should be called every frame.
    /// </summary>
    /// <param name="deltaTime">Time since last update.</param>
    public void Update(float deltaTime)
    {
        CurrentValue -= DecayRate * deltaTime;
        CurrentValue = Mathf.Clamp(CurrentValue, 0, MaxValue);
    }

    /// <summary>
    /// Fulfill the need by adding value, clamped to the maximum.
    /// </summary>
    /// <param name="amount">Amount to add.</param>
    public void Fulfill(float amount)
    {
        CurrentValue = Mathf.Clamp(CurrentValue + amount, 0, MaxValue);
    }

    /// <summary>
    /// Get a normalized value between 0 and 1 representing how satisfied this need is.
    /// </summary>
    public float Normalized => MaxValue > 0 ? CurrentValue / MaxValue : 0f;
}