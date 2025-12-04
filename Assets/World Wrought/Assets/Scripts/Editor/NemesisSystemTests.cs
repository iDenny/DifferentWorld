using NUnit.Framework;
using UnityEngine;

public class NemesisSystemTests
{
    [Test]
    public void AddAndReduceNemesis()
    {
        var go = new GameObject("TestChar");
        var charComp = go.AddComponent<Character>();
        var nem = go.AddComponent<NemesisSystem>();

        var otherGo = new GameObject("Other");
        var otherChar = otherGo.AddComponent<Character>();

        nem.AddNemesis(otherChar, 0.6f);
        Assert.IsTrue(nem.IsNemesis(otherChar));
        Assert.AreEqual(0.6f, nem.GetHostility(otherChar));

        nem.ReduceHostility(otherChar, 0.3f);
        Assert.AreEqual(0.3f, nem.GetHostility(otherChar));

        nem.ReduceHostility(otherChar, 0.5f);
        Assert.IsFalse(nem.IsNemesis(otherChar));
    }

    [Test]
    public void RecordInteractionCreatesProfile()
    {
        var go = new GameObject("TestChar2");
        var charComp = go.AddComponent<Character>();
        var nem = go.AddComponent<NemesisSystem>();

        var otherGo = new GameObject("Other2");
        var otherChar = otherGo.AddComponent<Character>();

        nem.RecordInteraction(otherChar, "Hit for 10", 0.1f);
        Assert.IsTrue(nem.IsNemesis(otherChar));
        Assert.IsTrue(nem.Profiles.ContainsKey(otherChar));
        Assert.IsNotEmpty(nem.Profiles[otherChar].History);
    }
}
