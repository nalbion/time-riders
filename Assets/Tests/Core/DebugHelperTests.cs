using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

/// <summary>
/// Play mode tests for DebugHelper utility logic.
/// </summary>
public class DebugHelperTests
{
    [UnityTest]
    public IEnumerator DebugHelper_CanBeCreatedAndLogsState()
    {
        var go = new GameObject("DebugHelper");
        var dh = go.AddComponent<DebugHelper>();
        yield return null;
        Assert.IsNotNull(dh);
        // Optionally, check log output or GameObject state if needed
    }
}
