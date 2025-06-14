using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

/// <summary>
/// Play mode tests for DebugHelper utility logic.
/// </summary>
public class DebugHelperTests
{
    [Test]
    public void DebugHelper_CanBeCreated()
    {
        var go = new GameObject("DebugHelper");
        var dh = go.AddComponent<DebugHelper>();
        Assert.IsNotNull(dh);
        // Optionally, check log output or GameObject state if needed
    }
}
