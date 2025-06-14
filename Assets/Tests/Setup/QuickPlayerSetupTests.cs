using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

/// <summary>
/// Play mode tests for QuickPlayerSetup logic.
/// </summary>
public class QuickPlayerSetupTests
{
    [UnityTest]
    public IEnumerator QuickPlayerSetup_CreatesPlayerObject()
    {
        var go = new GameObject("QuickPlayerSetup");
        var qps = go.AddComponent<QuickPlayerSetup>();
        // Call the public method to create a player
        qps.CreateBasicPlayer();
        yield return null;
        var player = GameObject.Find("Player1");
        Assert.IsNotNull(player, "Player1 should be created");
    }
}
