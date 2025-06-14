using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

/// <summary>
/// Basic play mode tests for GameManager core logic.
/// </summary>
public class GameManagerTests
{
    [UnityTest]
    public IEnumerator GameManager_InitializesCorrectly()
    {
        var go = new GameObject("GameManager");
        var gm = go.AddComponent<GameManager>();
        yield return null; // Wait one frame for initialization
        Assert.IsNotNull(gm);
        Assert.AreEqual(GameState.MainMenu, gm.CurrentState);
    }

    // Add more tests for race start, finish, and reset as needed
}
