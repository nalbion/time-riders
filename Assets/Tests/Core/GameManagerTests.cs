using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

/// <summary>
/// Basic play mode tests for GameManager core logic.
/// </summary>
public class GameManagerTests
{
    [Test]
    public void GameManager_InitializesCorrectly()
    {
        var go = new GameObject("GameManager");
        var gm = go.AddComponent<GameManager>();
        Assert.IsNotNull(gm);
        Assert.AreEqual(GameManager.GameState.MainMenu, gm.CurrentState);
    }

    // Add more tests for race start, finish, and reset as needed
}
