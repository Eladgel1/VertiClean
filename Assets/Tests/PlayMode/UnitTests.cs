using NUnit.Framework;
using UnityEngine;

public class UnitTests {
    private GameObject targetGO;
    private CleaningTarget target;

    [SetUp]
    public void SetUp() {
        targetGO = new GameObject("Target");
        target = targetGO.AddComponent<CleaningTarget>();

        SetPrivate(target, "sprayed", true);
        SetPrivate(target, "cleanHitsRequired", 10);
        SetPrivate(target, "requiredTool", ToolType.Sponge);
    }

    [TearDown]
    public void TearDown() {
        Object.DestroyImmediate(targetGO);
    }

    [Test]
    public void CleaningTarget_ProgressIncreasesWithCorrectTool() {
        var go = new GameObject();
        var target = go.AddComponent<CleaningTarget>();
        SetPrivate(target, "cleanHitsRequired", 10);
        SetPrivate(target, "sprayed", true);
        SetPrivate(target, "requiredTool", ToolType.Sponge);

        target.TryClean(ToolType.Sponge, 1f);
        float progress = target.GetProgress();

        Assert.Greater(progress, 0f);
    }

    [Test]
    public void CleaningTarget_NoProgressWithWrongTool() {
        var go = new GameObject();
        var target = go.AddComponent<CleaningTarget>();
        SetPrivate(target, "cleanHitsRequired", 10);
        SetPrivate(target, "sprayed", true);
        SetPrivate(target, "requiredTool", ToolType.Sponge);

        target.TryClean(ToolType.Spray, 1f);
        float progress = target.GetProgress();

        Assert.AreEqual(0f, progress);
    }

    [Test]
    public void CleaningToolBase_ResetsPositionAfterDrop() {
        var go = new GameObject("Tool");
        go.AddComponent<BoxCollider>();
        var mockTool = go.AddComponent<MockTool>();
        var hand = new GameObject("Hand").transform;

        mockTool.PickUp(hand);
        mockTool.Drop();

        Assert.AreEqual(mockTool.transform.localPosition, mockTool.GetOriginalLocalPosition());
        Assert.AreEqual(mockTool.transform.localRotation, mockTool.GetOriginalLocalRotation());
    }

    private void SetPrivate<T>(object obj, string fieldName, T value) {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(obj, value);
    }

    private class MockTool : CleaningToolBase {
        public override void UseTool(RaycastHit hit) { }
        public Vector3 GetOriginalLocalPosition() => originalLocalPosition;
        public Quaternion GetOriginalLocalRotation() => originalRotation;
    }
}
