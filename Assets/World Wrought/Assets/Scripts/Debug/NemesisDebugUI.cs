using UnityEngine;
using System.Text;

public class NemesisDebugUI : MonoBehaviour
{
    public Character TargetCharacter;
    private Vector2 scroll;

    private void OnGUI()
    {
        if (TargetCharacter == null)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 60));
            GUILayout.Label("Nemesis Debug UI: assign TargetCharacter in Inspector");
            GUILayout.EndArea();
            return;
        }

        var nem = TargetCharacter.GetComponent<NemesisSystem>();
        if (nem == null)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 60));
            GUILayout.Label("No NemesisSystem on TargetCharacter");
            GUILayout.EndArea();
            return;
        }

        GUILayout.BeginArea(new Rect(10, 10, 400, 400), GUI.skin.box);
        GUILayout.Label($"Nemeses for: {TargetCharacter.CharacterName}");
        scroll = GUILayout.BeginScrollView(scroll, GUILayout.Width(380), GUILayout.Height(340));

        foreach (var kvp in nem.Nemeses)
        {
            var c = kvp.Key;
            var hostility = kvp.Value;
            if (c == null) continue;
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{c.CharacterName} ({c.FamilyName})");
            GUILayout.FlexibleSpace();
            GUILayout.Label(hostility.ToString("F2"));
            GUILayout.EndHorizontal();

            // Show profile details if available
            if (nem.Profiles != null && nem.Profiles.ContainsKey(c))
            {
                var p = nem.Profiles[c];
                GUILayout.Label($"  Rank: {p.Rank}  Personality: {p.Personality}");
                if (p.History != null && p.History.Count > 0)
                {
                    foreach (var h in p.History)
                    {
                        GUILayout.Label($"    - {h}");
                    }
                }
            }
        }

        GUILayout.EndScrollView();
        if (GUILayout.Button("Clear Nemeses"))
        {
            // Clear list (for debug only)
            var keys = new System.Collections.Generic.List<Character>(nem.Nemeses.Keys);
            foreach (var k in keys) nem.ReduceHostility(k, 1f);
        }
        GUILayout.EndArea();
    }
}
