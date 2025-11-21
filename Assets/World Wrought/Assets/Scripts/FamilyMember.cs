using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a member of a family/dynasty. Stores relationships to parents and children.
/// </summary>
[Serializable]
public class FamilyMember
{
    public Character Character;
    public FamilyMember Father;
    public FamilyMember Mother;
    public List<FamilyMember> Children = new List<FamilyMember>();

    public FamilyMember(Character character)
    {
        Character = character;
    }

    public void AddChild(FamilyMember child)
    {
        if (!Children.Contains(child))
        {
            Children.Add(child);
            if (Character != null)
            {
                child.Father = this; // simplistic, for demonstration only
            }
        }
    }
}