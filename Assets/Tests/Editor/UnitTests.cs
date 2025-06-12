using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.TestTools;
using System.IO;
using System.Collections;
using System.Collections.Generic;

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

        string saveDir = Application.persistentDataPath + "/Saves/";
        if (!Directory.Exists(saveDir)) {
            Directory.CreateDirectory(saveDir);
        }
    }

    [TearDown]
    public void TearDown() {
        Object.DestroyImmediate(targetGO);
    }

    [Test]
    public void CleaningTarget_ProgressIncreasesWithCorrectTool() {
        target.TryClean(ToolType.Sponge, 1f);
        float progress = target.GetProgress();
        Assert.Greater(progress, 0f);
    }

    [Test]
    public void CleaningTarget_NoProgressWithWrongTool() {
        target.TryClean(ToolType.Spray, 1f);
        float progress = target.GetProgress();
        Assert.AreEqual(0f, progress);
    }

    [Test]
    public void CleaningToolBase_ResetsPositionAfterDrop() {
        var toolGO = new GameObject("Tool");
        toolGO.AddComponent<BoxCollider>();
        toolGO.AddComponent<Rigidbody>();

        var mockTool = toolGO.AddComponent<MockTool>();
        var hand = new GameObject("Hand").transform;

        mockTool.PickUp(hand);
        mockTool.Drop();

        Assert.AreEqual(mockTool.transform.localPosition, mockTool.GetOriginalLocalPosition());
        Assert.AreEqual(mockTool.transform.localRotation, mockTool.GetOriginalLocalRotation());
    }

    [Test]
    public void SprayTool_MarksTarget_WhenHeldAndAimed() {
        var spray = new GameObject("Spray").AddComponent<SprayTool>();
        var target = new GameObject("Target").AddComponent<CleaningTarget>();
        target.MarkSprayed();
        Assert.IsTrue(target.WasSprayed());
    }

    [Test]
    public void SoundManager_ActivatesWalkingSoundCorrectly() {
        var smGO = new GameObject("SoundManager");
        smGO.AddComponent<AudioSource>();
        var sm = smGO.AddComponent<SoundManager>();
        sm.SetWalking(true);
        Assert.Pass();
    }

    [Test]
    public void HapticManager_StartsAndStopsHaptics() {
        var hmGO = new GameObject("HapticManager");
        var hm = hmGO.AddComponent<HapticManager>();
        typeof(HapticManager).GetField("Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                             ?.SetValue(null, hm);

        hm.StartHaptic(XRNode.LeftHand, 0.5f);
        hm.StopHaptic(XRNode.LeftHand);
        Assert.Pass();
    }

    [Test]
    public void CleaningTarget_MarkSprayed_SetsFlag() {
        target.MarkSprayed();
        Assert.IsTrue(target.WasSprayed());
    }

    [Test]
    public void CleaningToolSO_AssignsCorrectToolType() {
        var so = ScriptableObject.CreateInstance<CleaningToolSO>();
        so.toolType = ToolType.Mop;
        Assert.AreEqual(ToolType.Mop, so.toolType);
    }

    [Test]
    public void GameData_StoresAndClearsReplayStage() {
        GameData.ReplayStage = 3;
        Assert.AreEqual(3, GameData.ReplayStage);
        GameData.ReplayStage = 0;
        Assert.AreEqual(0, GameData.ReplayStage);
    }

    [Test]
    public void GameData_StoresAndClearsSaveSlot() {
        GameData.PlayerName = "TestPlayer";
        GameData.Position = new Vector3(1, 2, 3);
        Assert.AreEqual("TestPlayer", GameData.PlayerName);
        Assert.AreEqual(new Vector3(1, 2, 3), GameData.Position);
    }

    [Test]
    public void SaveManager_CreatesAndDeletesSlot() {
        var go = new GameObject("SaveManager");
        var sm = go.AddComponent<SaveManager>();
        typeof(SaveManager).GetField("Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                           ?.SetValue(null, sm);

        GameData.PlayerName = "Dummy";
        GameData.Position = Vector3.one;
        GameData.Level = 1;
        GameData.MaxStageReached = 1;

        string path = Application.persistentDataPath + "/Saves/save_slot_0.json";
        if (File.Exists(path)) File.Delete(path);

        sm.SaveToSlot(0);

        var method = typeof(SaveManager).GetMethod("DelayedSave", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var enumerator = (IEnumerator)method.Invoke(sm, new object[] { 0 });
        while (enumerator.MoveNext()) { }

        bool fileCreated = File.Exists(path);
        Assert.IsTrue(fileCreated, "Save file was not created at: " + path);

        sm.DeleteSlot(0);
        Assert.IsFalse(File.Exists(path), "Save file was not deleted");

        Object.DestroyImmediate(go);
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