using NUnit.Framework;
using UnityEngine;

[Category("Canvas")]
public class RootCanvasTests : TestBehaviourBase<UnityEngine.Canvas>
{
    // A simple nested canvas hierarchy
    // m_TestObject
    //     └ rootCanvasChild
    //              └ emptyChildGameObject
    //                          └ baseCanvas
    private UnityEngine.Canvas rootCanvasChild;
    private GameObject emptyChildGameObject;
    private UnityEngine.Canvas baseCanvas;

    [SetUp]
    public override void TestSetup()
    {
        base.TestSetup();

        var rootChildGO = new GameObject("root child");
        rootCanvasChild = rootChildGO.AddComponent<Canvas>();

        emptyChildGameObject = new GameObject("empty");

        var baseGO = new GameObject("base");
        baseCanvas = baseGO.AddComponent<Canvas>();

        baseCanvas.transform.SetParent(emptyChildGameObject.transform);
        emptyChildGameObject.transform.SetParent(rootChildGO.transform);
        rootChildGO.transform.SetParent(m_TestObject.transform);
    }

    [Test]
    public void IsRootCanvasTest()
    {
        Assert.IsFalse(baseCanvas.isRootCanvas);
        Assert.IsFalse(rootCanvasChild.isRootCanvas);
        Assert.IsTrue(m_TestObject.isRootCanvas);
    }

    [Test]
    public void CorrectRootCanvasReturned()
    {
        Assert.AreEqual(m_TestObject, m_TestObject.rootCanvas);
        Assert.AreEqual(m_TestObject, rootCanvasChild.rootCanvas);
        Assert.AreEqual(m_TestObject, baseCanvas.rootCanvas);
    }

    [Test]
    public void IsNotRootCanvasWhenDisabled()
    {
        baseCanvas.enabled = false;
        Assert.IsFalse(baseCanvas.isRootCanvas);
    }
}
