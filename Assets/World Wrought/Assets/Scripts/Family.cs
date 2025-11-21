using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Family
{
    public string FamilyName;
    public List<Character> Members = new List<Character>();

    public Family() { }
    public Family(string name) { FamilyName = name; }

    public void AddMember(Character c)
    {
        if (c == null) return;
        if (!Members.Contains(c))
        {
            Members.Add(c);
            c.FamilyName = FamilyName;
        }
    }

    public void RemoveMember(Character c)
    {
        if (c == null) return;
        if (Members.Remove(c))
        {
            c.FamilyName = string.Empty;
        }
    }
}
