using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

/// <summary>
/// Play mode tests for PlayerController core logic.
/// </summary>
public class PlayerControllerTests
{
    [UnityTest]
    public IEnumerator PlayerController_InitializesWithDefaultValues()
    {
        var go = new GameObject("Player");
        var pc = go.AddComponent<PlayerController>();
        yield return null; // Wait one frame for initialization
        Assert.IsNotNull(pc);
        Assert.GreaterOrEqual(pc.MaxSpeed, 0f, "MaxSpeed should be non-negative");
        Assert.GreaterOrEqual(pc.Acceleration, 0f, "Acceleration should be non-negative");
    }

    // Add more tests for movement, input, and physics as needed
}
