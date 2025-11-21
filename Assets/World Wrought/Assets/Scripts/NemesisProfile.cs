using System.Collections.Generic;

/// <summary>
/// Contains detailed information about a nemesis or enemy individual.  A
/// profile stores rank, traits, strengths, weaknesses, personality and a
/// history of interactions.  Use this alongside <see cref="NemesisSystem"/>
/// to build a dynamic, evolving nemesis framework reminiscent of
/// Middle‑earth: Shadow of War.  Profiles can be updated whenever the
/// player encounters or defeats the nemesis to evolve their story.
/// </summary>
public class NemesisProfile
{
    public string Name;
    public int Rank;
    public List<string> Traits = new();
    public List<string> Strengths = new();
    public List<string> Weaknesses = new();
    public string Personality;
    public List<string> History = new();

    public NemesisProfile(string name, int rank = 0)
    {
        Name = name;
        Rank = rank;
    }
}