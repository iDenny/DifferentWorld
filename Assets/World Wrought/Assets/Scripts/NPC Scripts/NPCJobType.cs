/// <summary>
/// Enumerates the types of jobs an NPC can perform.  Jobs represent
/// high‑level activities and routines that drive daily behaviour.  Expand
/// this list as your game grows to include crafting, farming, leading
/// squads or becoming a legend.  NPCs cycle through jobs via their
/// schedule in <see cref="NPCController"/>.
/// </summary>
public enum NPCJobType
{
    Idle,
    Sleep,
    Eat,
    Work,
    Patrol,
    Train,
    Craft,
    Farm,
    LeadSquad,
    Fight,
    ReturnHome,
    Celebrate,
    Complain,
    Promote,
    Defect,
    Mutiny,
    Legend
}