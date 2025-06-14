using NUnit.Framework;
using UnityEngine.TestTools;
using System.Collections;

public class MinimalTest
{
    [UnityTest]
    public IEnumerator TestSomething()
    {
        yield return null;
        Assert.IsTrue(true);
    }
}
