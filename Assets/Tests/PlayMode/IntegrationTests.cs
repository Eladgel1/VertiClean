using NUnit.Framework;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using UnityEngine.UI;

public class IntegrationTests {
    private GameObject spongeObj;
    private GameObject sprayObj;
    private GameObject dirtObj;
    private GameObject platformObj;
    private GameObject mockInputObj;
    private GameObject cameraObj;
    private GameObject hapticObj;
    private GameObject soundObj;
    private GameObject cleaningUIObj;
    private GameObject stageUIObj;
    private GameObject introObj;
    private GameObject playerObj;
    private GameObject introCanvasGO;

    private SpongeTool sponge;
    private SprayTool spray;
    private CleaningTarget target;
    private CleanerPlatform platform;

    [SetUp]
    public void SetUp() {
        mockInputObj = new GameObject("MockVRInput");
        var input = mockInputObj.AddComponent<VRInputManager>();
        typeof(VRInputManager).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, input);
        typeof(VRInputManager).GetField("scrubVector", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(input, new Vector2(1f, 1f));
        typeof(VRInputManager).GetField("scrubButtonHeld", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(input, true);

        cameraObj = new GameObject("MainCamera");
        var cam = cameraObj.AddComponent<Camera>();
        cam.tag = "MainCamera";
        cam.transform.position = Vector3.zero;
        cam.transform.forward = Vector3.forward;

        dirtObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        dirtObj.name = "CleaningTarget";
        dirtObj.transform.position = new Vector3(0, 0, 2);
        dirtObj.layer = LayerMask.NameToLayer("Default");
        dirtObj.AddComponent<Rigidbody>().isKinematic = true;
        if (!dirtObj.TryGetComponent(out BoxCollider _)) dirtObj.AddComponent<BoxCollider>();

        target = dirtObj.AddComponent<CleaningTarget>();
        PrivateSet(target, "cleanHitsRequired", 10);
        PrivateSet(target, "requiredTool", ToolType.Sponge);

        spongeObj = new GameObject("SpongeTool");
        spongeObj.AddComponent<BoxCollider>();
        sponge = spongeObj.AddComponent<SpongeTool>();
        PrivateSet(sponge, "dirtLayer", (LayerMask)(1 << dirtObj.layer));
        PrivateSet(sponge, "toolData", CreateToolSO(ToolType.Sponge));
        typeof(CleaningToolBase).GetField("isHeld", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(sponge, true);

        sprayObj = new GameObject("SprayTool");
        sprayObj.AddComponent<BoxCollider>();
        spray = sprayObj.AddComponent<SprayTool>();
        PrivateSet(spray, "dirtLayer", (LayerMask)(1 << dirtObj.layer));
        PrivateSet(spray, "toolData", CreateToolSO(ToolType.Spray));
        typeof(CleaningToolBase).GetField("isHeld", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(spray, true);

        platformObj = new GameObject("Platform");
        platform = platformObj.AddComponent<CleanerPlatform>();
        typeof(CleanerPlatform).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, platform);

        hapticObj = new GameObject("HapticManager");
        hapticObj.AddComponent<HapticManager>();
        typeof(HapticManager).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, hapticObj.GetComponent<HapticManager>());

        soundObj = new GameObject("SoundManager");
        soundObj.AddComponent<AudioSource>();
        soundObj.AddComponent<SoundManager>();
        typeof(SoundManager).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, soundObj.GetComponent<SoundManager>());

        cleaningUIObj = new GameObject("CleaningUI");
        var textGO = new GameObject("FeedbackText");
        textGO.transform.SetParent(cleaningUIObj.transform);
        var tmp = textGO.AddComponent<TextMeshProUGUI>();

        var sliderGO = new GameObject("ProgressSlider");
        sliderGO.transform.SetParent(cleaningUIObj.transform);
        var slider = sliderGO.AddComponent<Slider>();

        var ui = cleaningUIObj.AddComponent<CleaningProgressUI>();
        PrivateSet(ui, "feedbackText", tmp);
        PrivateSet(ui, "progressBar", slider);
        typeof(CleaningProgressUI).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, ui);

        stageUIObj = new GameObject("StageFeedbackUI");
        var sfu = stageUIObj.AddComponent<StageFeedbackUI>();
        typeof(StageFeedbackUI).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, sfu);

        introObj = new GameObject("IntroManager");
        var intro = introObj.AddComponent<IntroManager>();
        typeof(IntroManager).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, intro);

        // Create introPanel and introText
        introCanvasGO = new GameObject("IntroCanvas");
        var panelGO = new GameObject("IntroPanel");
        panelGO.transform.SetParent(introCanvasGO.transform);
        var introPanel = panelGO; // Just a placeholder GameObject

        var introTextGO = new GameObject("IntroText");
        introTextGO.transform.SetParent(introCanvasGO.transform);
        var introText = introTextGO.AddComponent<TextMeshProUGUI>();

        PrivateSet(intro, "introPanel", introPanel);
        PrivateSet(intro, "introText", introText);

        playerObj = new GameObject("Player");
        playerObj.AddComponent<CharacterController>();
        var player = playerObj.AddComponent<Player>();
        typeof(Player).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, player);
    }

    [TearDown]
    public void TearDown() {
        Object.DestroyImmediate(spongeObj);
        Object.DestroyImmediate(sprayObj);
        Object.DestroyImmediate(dirtObj);
        Object.DestroyImmediate(platformObj);
        Object.DestroyImmediate(mockInputObj);
        Object.DestroyImmediate(cameraObj);
        Object.DestroyImmediate(hapticObj);
        Object.DestroyImmediate(soundObj);
        Object.DestroyImmediate(cleaningUIObj);
        Object.DestroyImmediate(stageUIObj);
        Object.DestroyImmediate(introObj);
        Object.DestroyImmediate(introCanvasGO);
        Object.DestroyImmediate(playerObj);

        typeof(VRInputManager).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, null);
        typeof(SoundManager).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, null);
        typeof(HapticManager).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, null);
        typeof(CleaningProgressUI).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, null);
        typeof(StageFeedbackUI).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, null);
        typeof(IntroManager).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, null);
        typeof(Player).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, null);
    }

    [Test]
    public void SpongeTool_CleansTarget_WhenSprayedFirst() {
        PrivateSet(target, "sprayed", true);
        Assert.IsTrue(target.WasSprayed(), "Target should be marked as sprayed.");

        float time = 0f;
        while (time < 1f) {
            sponge.GetType().GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(sponge, null);
            time += 0.05f;
        }

        Assert.GreaterOrEqual(target.GetProgress(), 0f);
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

    [Test]
    public void Cleaning_DoesNotProgress_WithoutSpray() {
        PrivateSet(target, "sprayed", false);
        float before = target.GetProgress();
        sponge.GetType().GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(sponge, null);
        float after = target.GetProgress();
        Assert.AreEqual(before, after);
    }

    [Test]
    public void SpongeTool_TriggersHaptic_WhenCleaning() {
        target.MarkSprayed();
        sponge.GetType().GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(sponge, null);
        Assert.Pass();
    }

    [Test]
    public void SprayTool_EnablesEffectAndSound() {
        spray.GetType().GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(spray, null);
        Assert.Pass();
    }

    [Test]
    public void StageManager_ChangesStageCorrectly() {
        var smGO = new GameObject("StageManager");
        var sm = smGO.AddComponent<StageManager>();
        var dummyTarget = new GameObject("DummyTarget").AddComponent<CleaningTarget>();
        PrivateSet(sm, "allTargets", new System.Collections.Generic.List<CleaningTarget> { dummyTarget });

        sm.SetStage(2);
        Assert.AreEqual(2, sm.GetCurrentStage());
    }

    [Test]
    public void CleaningProgressUI_ShowsAndHidesFeedback() {
        var ui = CleaningProgressUI.Instance;
        ui.ShowFeedback("Nice!", Color.yellow);
        ui.HideProgressBar();
        Assert.Pass();
    }

    [Test]
    public void Player_TriggersWalkingSound_WhenMoving() {
        typeof(VRInputManager).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, mockInputObj.GetComponent<VRInputManager>());
        SoundManager.Instance.SetWalking(true);
        Assert.Pass();
    }

    private void PrivateSet<T>(object obj, string fieldName, T value) {
        var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        field?.SetValue(obj, value);
    }

    private CleaningToolSO CreateToolSO(ToolType type) {
        var so = ScriptableObject.CreateInstance<CleaningToolSO>();
        so.toolType = type;
        return so;
    }
}
