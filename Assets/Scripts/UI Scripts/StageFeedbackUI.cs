using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StageFeedbackUI : MonoBehaviour {
    public static StageFeedbackUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject canvasObject;
    [SerializeField] private GameObject panel; // only the container of buttons
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Rating Buttons")]
    [SerializeField] private Button veryLowButton;
    [SerializeField] private Button lowButton;
    [SerializeField] private Button mediumButton;
    [SerializeField] private Button highButton;
    [SerializeField] private Button veryHighButton;

    private Button[] ratingButtons;
    private int selectedIndex = 0;
    private bool awaitingRating = false;
    private bool awaitingContinue = false;

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        if (canvasObject != null)
            canvasObject.SetActive(false);

        veryLowButton?.onClick.AddListener(() => FinalizeRating(1));
        lowButton?.onClick.AddListener(() => FinalizeRating(2));
        mediumButton?.onClick.AddListener(() => FinalizeRating(3));
        highButton?.onClick.AddListener(() => FinalizeRating(4));
        veryHighButton?.onClick.AddListener(() => FinalizeRating(5));

        ratingButtons = new Button[] {
            veryLowButton, lowButton, mediumButton, highButton, veryHighButton
        };
    }

    public void ShowStageCompleteMessage(int stageNumber) {
        if (canvasObject != null) canvasObject.SetActive(true);
        if (panel != null) panel.SetActive(true);

        messageText.text =
            $"Congratulations!\nYou have completed stage {stageNumber}.\n" +
            "You may descend to the ground to proceed.\n\n" +
            "Please rate the overall experience of the current stage:";

        SoundManager.Instance?.PlayStageComplete();

        Player.Instance.DisableMovement();

        selectedIndex = 0;
        awaitingRating = true;
        SelectButton(selectedIndex);
    }
    private void Update() {
        if (awaitingRating) {
            Vector2 nav = VRInputManager.Instance.GetUINavigationDelta();

            if (nav.x > 0.5f) {
                selectedIndex = (selectedIndex + 1) % ratingButtons.Length;
                SelectButton(selectedIndex);
            }
            else if (nav.x < -0.5f) {
                selectedIndex = (selectedIndex - 1 + ratingButtons.Length) % ratingButtons.Length;
                SelectButton(selectedIndex);
            }
            else if (VRInputManager.Instance.GetUIClick()) {
                ratingButtons[selectedIndex].onClick.Invoke();
            }
        }
        else if (awaitingContinue && VRInputManager.Instance.GetContinuePressed()) {
            awaitingContinue = false;

            if (canvasObject != null)
                canvasObject.SetActive(false);

            Player.Instance.EnableMovement();
            StageManager.Instance.ReadyToDescend();
        }
    }

    private void SelectButton(int index) {
        if (ratingButtons[index] != null)
            ratingButtons[index].Select();
    }

    private void FinalizeRating(int score) {
        Debug.Log("User rated stage: " + score);

        messageText.text =
            "Thank you for your feedback!\nYou may proceed to the next stage.\n\nPress the Right Trigger Button to continue.";

        awaitingRating = false;

        // Disable entire panel containing the buttons
        if (panel != null)
            panel.SetActive(false);

        Invoke(nameof(EnableContinue), 0.3f); // prevent double-enter

        // END TRACKING SESSION AND SAVE RATING
        StatisticsManager.Instance.EndSession(score.ToString());

    }

    private void EnableContinue() {
        awaitingContinue = true;
    }
}