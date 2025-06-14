using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

/// <summary>
/// Play mode tests for SceneSetup logic (terrain/environment setup).
/// </summary>
public class SceneSetupTests
{
    [Test]
    public void SceneSetup_CreatesTerrain()
    {
        var go = new GameObject("SceneSetup");
        var setup = go.AddComponent<SceneSetup>();
        setup.CreateBasicTerrain();
        var terrain = GameObject.FindObjectOfType<Terrain>();
        Assert.IsNotNull(terrain, "Terrain should be created by SceneSetup");
    }
}
