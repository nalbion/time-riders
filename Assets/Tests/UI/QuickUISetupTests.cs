using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Play mode tests for QuickUISetup logic.
/// </summary>
public class QuickUISetupTests
{
    [Test]
    public void QuickUISetup_CreatesStartButton()
    {
        var go = new GameObject("QuickUISetup");
        var quickUI = go.AddComponent<QuickUISetup>();
        quickUI.CreateStartButton(go.transform);
        var button = GameObject.Find("StartButton");
        Assert.IsNotNull(button, "StartButton should be created");
        Assert.IsNotNull(button.GetComponent<Button>(), "StartButton should have a Button component");
    }
}
