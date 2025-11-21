using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls an NPC's daily routine and high‑level behaviour.  Each NPC
/// schedules a series of <see cref="NPCJobType"/> entries which are
/// processed over time.  Jobs determine what the NPC is currently doing
/// (sleeping, eating, working, patrolling, etc.) and can influence mood or
/// relationships.  This controller is intentionally lightweight and
/// extensible; specific jobs can be implemented in specialised components
/// (e.g. FarmingSystem, CraftingSystem) or simple method calls.
/// </summary>
[RequireComponent(typeof(Character))]
public class NPCController : MonoBehaviour
{
    /// <summary>
    /// Sequence of jobs that form the NPC's daily schedule.  By default the
    /// schedule contains a basic day: sleep → eat → work → eat → patrol →
    /// celebrate → sleep.  You can modify this list at runtime to assign
    /// different professions or respond to world events.
    /// </summary>
    public List<NPCJobType> Schedule = new List<NPCJobType>
    {
        NPCJobType.Sleep,
        NPCJobType.Eat,
        NPCJobType.Work,
        NPCJobType.Eat,
        NPCJobType.Patrol,
        NPCJobType.Celebrate
    };

    /// <summary>
    /// Duration (in seconds) each job occupies before moving on to the next
    /// entry in the schedule.  If the number of durations does not match
    /// the number of schedule entries, durations will be recycled.
    /// </summary>
    public List<float> JobDurations = new List<float> { 6f, 2f, 8f, 2f, 4f, 2f };

    /// <summary>
    /// Index of the current job in the schedule.
    /// </summary>
    private int currentJobIndex;

    /// <summary>
    /// Time remaining (in seconds) for the current job.
    /// </summary>
    private float timeRemaining;

    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
        ResetSchedule();
    }

    private void Update()
    {
        if (Schedule == null || Schedule.Count == 0) return;
        // Countdown the timer; when it reaches zero, progress to the next job.
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            AdvanceToNextJob();
        }
        // Perform behaviour associated with the current job.  In a full
        // implementation, each job would trigger animations, pathfinding or
        // interactions.  For now we adjust mood slightly to simulate
        // satisfaction or fatigue.
        PerformJob(Schedule[currentJobIndex]);
    }

    /// <summary>
    /// Resets the schedule to the first job and resets timers.
    /// </summary>
    public void ResetSchedule()
    {
        currentJobIndex = 0;
        timeRemaining = GetJobDuration(currentJobIndex);
    }

    /// <summary>
    /// Advances to the next job in the schedule, looping when the end is
    /// reached.  Resets the timer for the new job.
    /// </summary>
    public void AdvanceToNextJob()
    {
        currentJobIndex = (currentJobIndex + 1) % Schedule.Count;
        timeRemaining = GetJobDuration(currentJobIndex);
    }

    /// <summary>
    /// Retrieves the duration for the specified job index, cycling the
    /// durations list if needed.
    /// </summary>
    private float GetJobDuration(int index)
    {
        if (JobDurations == null || JobDurations.Count == 0)
            return 1f;
        return JobDurations[index % JobDurations.Count];
    }

    /// <summary>
    /// Executes lightweight effects for the current job.  Real systems
    /// should override or extend this behaviour.  For demonstration
    /// purposes, mood is adjusted depending on the activity.
    /// </summary>
    private void PerformJob(NPCJobType job)
    {
        switch (job)
        {
            case NPCJobType.Sleep:
                // Sleeping gradually restores mood
                character.ModifyMood(0.01f * Time.deltaTime);
                break;
            case NPCJobType.Eat:
                // Eating gives a quick boost
                character.ModifyMood(0.02f * Time.deltaTime);
                break;
            case NPCJobType.Work:
            case NPCJobType.Patrol:
            case NPCJobType.Train:
            case NPCJobType.Craft:
            case NPCJobType.Farm:
                // Work drains mood slowly
                character.ModifyMood(-0.005f * Time.deltaTime);
                break;
            case NPCJobType.Celebrate:
                // Celebrations improve mood
                character.ModifyMood(0.03f * Time.deltaTime);
                break;
            case NPCJobType.Complain:
            case NPCJobType.Defect:
            case NPCJobType.Mutiny:
                // Negative jobs decrease mood quickly
                character.ModifyMood(-0.02f * Time.deltaTime);
                break;
            default:
                // Idle or unhandled jobs cause slight mood decay
                character.ModifyMood(-0.001f * Time.deltaTime);
                break;
        }
    }
}