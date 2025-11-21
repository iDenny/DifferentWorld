using UnityEngine;

/// <summary>
/// Base class for character traits. Traits influence needs, mood and behaviour.
/// Create subclasses to implement specific effects (e.g., Brave, Calm, Quick).
/// </summary>
public abstract class Trait : ScriptableObject
{
    public string TraitName;
    [TextArea]
    public string Description;

    /// <summary>
    /// Apply trait effects to a character at startup.
    /// This could modify needs, skills or other stats.
    /// </summary>
    public virtual void Apply(Character character) { }
}