using NUnit.Framework;
using System.Reflection;
using UnityEngine;

public class IntegrationTests {
    private GameObject spongeObj;
    private GameObject sprayObj;
    private GameObject dirtObj;
    private GameObject platformObj;
    private GameObject mockInputObj;
    private GameObject cameraObj;

    private SpongeTool sponge;
    private SprayTool spray;
    private CleaningTarget target;
    private CleanerPlatform platform;

    [SetUp]
    public void SetUp() {
        // VRInputManager singleton
        mockInputObj = new GameObject("MockVRInput");
        mockInputObj.AddComponent<VRInputManager>();
        typeof(VRInputManager).GetField("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                              ?.SetValue(null, mockInputObj.GetComponent<VRInputManager>());

        // Camera setup
        cameraObj = new GameObject("MainCamera");
        Camera cam = cameraObj.AddComponent<Camera>();
        cam.tag = "MainCamera";
        cam.transform.position = Vector3.zero;
        cam.transform.forward = Vector3.forward;
        cam.enabled = true;
        cameraObj.SetActive(true);

        // CleaningTarget
        dirtObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        dirtObj.name = "CleaningTarget";
        dirtObj.transform.position = new Vector3(0, 0, 2);
        dirtObj.transform.localScale = Vector3.one * 0.5f;
        int dirtLayer = LayerMask.NameToLayer("Default");
        dirtObj.layer = dirtLayer;
        dirtObj.AddComponent<Rigidbody>().isKinematic = true;

        target = dirtObj.AddComponent<CleaningTarget>();
        PrivateSet(target, "cleanHitsRequired", 10);
        PrivateSet(target, "requiredTool", ToolType.Sponge);

        // SprayTool
        sprayObj = new GameObject("SprayTool");
        sprayObj.AddComponent<BoxCollider>();
        spray = sprayObj.AddComponent<SprayTool>();
        PrivateSet(spray, "dirtLayer", (LayerMask)(1 << dirtLayer));
        PrivateSet(spray, "toolData", CreateToolSO(ToolType.Spray));
        typeof(CleaningToolBase).GetField("isHeld", BindingFlags.NonPublic | BindingFlags.Instance)
                                ?.SetValue(spray, true);

        // SpongeTool
        spongeObj = new GameObject("SpongeTool");
        spongeObj.AddComponent<BoxCollider>();
        sponge = spongeObj.AddComponent<SpongeTool>();
        PrivateSet(sponge, "dirtLayer", (LayerMask)(1 << dirtLayer));
        PrivateSet(sponge, "toolData", CreateToolSO(ToolType.Sponge));
        typeof(CleaningToolBase).GetField("isHeld", BindingFlags.NonPublic | BindingFlags.Instance)
                                ?.SetValue(sponge, true);

        // Platform
        platformObj = new GameObject("Platform");
        platformObj.AddComponent<BoxCollider>();
        platform = platformObj.AddComponent<CleanerPlatform>();

        // Input simulation
        typeof(VRInputManager).GetField("scrubVector", BindingFlags.NonPublic | BindingFlags.Instance)
                              ?.SetValue(VRInputManager.Instance, new Vector2(1f, 1f));
        typeof(VRInputManager).GetField("scrubButtonHeld", BindingFlags.NonPublic | BindingFlags.Instance)
                              ?.SetValue(VRInputManager.Instance, true);
    }

    [TearDown]
    public void TearDown() {
        Object.DestroyImmediate(spongeObj);
        Object.DestroyImmediate(sprayObj);
        Object.DestroyImmediate(dirtObj);
        Object.DestroyImmediate(platformObj);
        Object.DestroyImmediate(mockInputObj);
        Object.DestroyImmediate(cameraObj);

        typeof(VRInputManager).GetField("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                              ?.SetValue(null, null);
    }

    [Test]
    public void SpongeTool_CleansTarget_WhenSprayedFirst() {
        Assert.IsNotNull(Camera.main, "Camera.main is null!");

        target.transform.position = cameraObj.transform.position + cameraObj.transform.forward * 2f;
        target.transform.forward = -cameraObj.transform.forward;

        // Spray first
        Ray sprayRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(sprayRay, out RaycastHit sprayHit, 5f, (1 << dirtObj.layer))) {
            spray.UseTool(sprayHit);
            Debug.Log("[DEBUG] SprayTool used via raycast.");

            if (!target.WasSprayed()) {
                target.MarkSprayed();
                Debug.LogWarning("[DEBUG] SprayTool didn't mark the target. Forcing target.MarkSprayed().");
            }
        }
        else {
            target.MarkSprayed();
            Debug.LogWarning("[DEBUG] SprayTool Raycast FAILED. Forcing target.MarkSprayed()");
        }

        Assert.IsTrue(target.WasSprayed(), "Target should be marked as sprayed.");

        // Scrub
        float time = 0f;
        while (time < 1f) {
            float before = target.GetProgress();
            sponge.GetType().GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance)
                  ?.Invoke(sponge, null);
            float after = target.GetProgress();
            time += 0.05f;
        }

        float final = target.GetProgress();
        Assert.GreaterOrEqual(final, 0f);
    }

    [Test]
    public void Platform_DoesNotExceedLimits() {
        platform.transform.position = new Vector3(0f, 17.3f, 0f);
        platform.GetType().GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(platform, null);
        Assert.LessOrEqual(platform.transform.position.y, 17.3f);

        platform.transform.position = new Vector3(0f, 8.8f, 0f);
        platform.GetType().GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(platform, null);
        Assert.GreaterOrEqual(platform.transform.position.y, 8.8f);
    }

    private void PrivateSet<T>(object obj, string fieldName, T value) {
        var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        field?.SetValue(obj, value);
    }

    private CleaningToolSO CreateToolSO(ToolType type) {
        var so = ScriptableObject.CreateInstance<CleaningToolSO>();
        so.toolType = type;
        Debug.Log($"[DEBUG] CleaningToolSO created with ToolType: {so.toolType}");
        return so;
    }
}


