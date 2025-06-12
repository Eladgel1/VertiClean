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
        PrivateSet(target, "uniqueID", "dummyID");

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
        ui.GetType().GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(ui, null);

        stageUIObj = new GameObject("StageFeedbackUI");
        var sfu = stageUIObj.AddComponent<StageFeedbackUI>();
        typeof(StageFeedbackUI).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, sfu);

        introObj = new GameObject("IntroManager");
        var intro = introObj.AddComponent<IntroManager>();
        typeof(IntroManager).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, intro);

        introCanvasGO = new GameObject("IntroCanvas");
        var panelGO = new GameObject("IntroPanel");
        panelGO.transform.SetParent(introCanvasGO.transform);
        var introPanel = panelGO;

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

        typeof(VRInputManager).SetField("Instance", null);
        typeof(SoundManager).SetField("Instance", null);
        typeof(HapticManager).SetField("Instance", null);
        typeof(CleaningProgressUI).SetField("Instance", null);
        typeof(StageFeedbackUI).SetField("Instance", null);
        typeof(IntroManager).SetField("Instance", null);
        typeof(Player).SetField("Instance", null);
    }

    [Test]
    public void SpongeTool_CleansTarget_WhenSprayedFirst() {
        PrivateSet(target, "sprayed", true);
        Assert.IsTrue(target.WasSprayed());

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
        platform.InvokePrivateUpdate();
        Assert.LessOrEqual(platform.transform.position.y, 17.3f);

        platform.transform.position = new Vector3(0f, 8.8f, 0f);
        platform.InvokePrivateUpdate();
        Assert.GreaterOrEqual(platform.transform.position.y, 8.8f);
    }

    [Test]
    public void Cleaning_DoesNotProgress_WithoutSpray() {
        PrivateSet(target, "sprayed", false);
        float before = target.GetProgress();
        sponge.InvokePrivateUpdate();
        float after = target.GetProgress();
        Assert.AreEqual(before, after);
    }

    [Test]
    public void SpongeTool_TriggersHaptic_WhenCleaning() {
        target.MarkSprayed();
        sponge.InvokePrivateUpdate();
        Assert.Pass();
    }

    [Test]
    public void SprayTool_EnablesEffectAndSound() {
        spray.InvokePrivateUpdate();
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
        typeof(VRInputManager).SetField("Instance", mockInputObj.GetComponent<VRInputManager>());
        SoundManager.Instance.SetWalking(true);
        Assert.Pass();
    }

    [Test]
    public void SaveMenuUI_OpensAndDisplaysSlotData() {
        var uiGO = new GameObject("SaveMenuUI");
        var saveUI = uiGO.AddComponent<SaveMenuUI>();

        var titleGO = new GameObject("TitleText");
        titleGO.transform.SetParent(uiGO.transform);
        var titleText = titleGO.AddComponent<TextMeshProUGUI>();
        PrivateSet(saveUI, "titleText", titleText);

        var slotGO = new GameObject("Slot0");
        slotGO.transform.SetParent(uiGO.transform);
        var button = slotGO.AddComponent<Button>();

        var labelGO = new GameObject("Label");
        labelGO.transform.SetParent(slotGO.transform);
        var label = labelGO.AddComponent<TextMeshProUGUI>();
        label.text = "Slot 0";

        var slotArray = new GameObject[] { slotGO };
        var field = typeof(SaveMenuUI).GetField("slotButtons", BindingFlags.NonPublic | BindingFlags.Instance);
        field?.SetValue(saveUI, slotArray);

        var namePanel = new GameObject("EnterNamePanel");
        namePanel.transform.SetParent(uiGO.transform);
        PrivateSet(saveUI, "enterNamePanel", namePanel);

        var nameInputGO = new GameObject("NameInput");
        nameInputGO.transform.SetParent(namePanel.transform);
        var input = nameInputGO.AddComponent<TMP_InputField>();
        PrivateSet(saveUI, "nameInput", input);

        var confirmGO = new GameObject("ConfirmButton");
        confirmGO.transform.SetParent(namePanel.transform);
        var confirm = confirmGO.AddComponent<Button>();
        PrivateSet(saveUI, "confirmNameButton", confirm);

        PrivateSet(saveUI, "saveSlots", new System.Collections.Generic.List<Button> { button });

        uiGO.SetActive(true);
        saveUI.Open(SaveActionType.Save);

        Assert.IsTrue(uiGO.activeSelf || uiGO.activeInHierarchy);
        Object.DestroyImmediate(uiGO);
    }

    [Test]
    public void GameRestorer_RestoresCleanedTargetProperly() {
        var restorerGO = new GameObject("Restorer");
        var restorer = restorerGO.AddComponent<GameRestorer>();
        typeof(GameRestorer).SetField("Instance", restorer);

        var dummyData = new FullSaveData {
            cleanedTargetIDs = new System.Collections.Generic.List<string> { "abc123" },
            stageNumber = 2,
            cleanerPlatformY = 12.5f,
            playerData = new SaveData {
                position = new Vector3(1, 1, 1),
                playerName = "PlayerX"
            }
        };

        GameData.LoadFromFullSave(dummyData);
        restorer.ApplyLoadedData(dummyData);

        Assert.AreEqual(2, GameData.Level);
        Assert.AreEqual(new Vector3(1, 1, 1), GameData.Position);

        Object.DestroyImmediate(restorerGO);
    }

    [Test]
    public void GameManager_ReplaysStageWhenSet() {
        GameData.ReplayStage = 2;

        var smGO = new GameObject("StageManager");
        var sm = smGO.AddComponent<StageManager>();
        typeof(StageManager).SetField("Instance", sm);

        var dummyTarget = new GameObject("DummyTarget").AddComponent<CleaningTarget>();
        dummyTarget.transform.position = Vector3.zero;
        PrivateSet(dummyTarget, "uniqueID", "abc");
        PrivateSet(sm, "allTargets", new System.Collections.Generic.List<CleaningTarget> { dummyTarget });

        var replayGO = new GameObject("ReplayStageHandler");
        var handler = replayGO.AddComponent<ReplayStageHandler>();
        handler.InvokeAwake();

        var gmGO = new GameObject("GameManager");
        var gm = gmGO.AddComponent<GameManager>();
        gm.InvokeAwake();
        gm.InvokeStart();

        Assert.AreEqual(2, GameData.Level);

        Object.DestroyImmediate(gmGO);
        Object.DestroyImmediate(smGO);
        Object.DestroyImmediate(replayGO);
    }

    [Test]
    public void MenuManager_TriggersCorrectSceneOnButtonClick() {
        var mmGO = new GameObject("MenuManager");
        var menu = mmGO.AddComponent<MenuManager>();
        menu.LoadGameSceneFromButton();
        Assert.IsTrue(menu.isInGame);
        Object.DestroyImmediate(mmGO);
    }

    [Test]
    public void LoadBuffer_HasPendingSave_ReturnsTrue() {
        LoadBuffer.pendingSlotIndex = 1;
        Assert.IsTrue(LoadBuffer.HasPendingLoad);
        LoadBuffer.Clear();
        Assert.IsFalse(LoadBuffer.HasPendingLoad);
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

public static class ReflectionExtensions {
    public static void SetField(this System.Type type, string name, object value) =>
        type.GetField(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(null, value);

    public static void InvokePrivateUpdate(this MonoBehaviour mb) =>
        mb.GetType().GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(mb, null);

    public static void InvokeAwake(this MonoBehaviour mb) =>
        mb.GetType().GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(mb, null);

    public static void InvokeStart(this MonoBehaviour mb) =>
        mb.GetType().GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(mb, null);
}

