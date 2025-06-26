using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// This script manually triggers the Tunneling Vignette effect.
/// It should be referenced by the Player script and called when moving or rotating.
/// </summary>
public class PlayerTunnelingVignetteProvider : MonoBehaviour, ITunnelingVignetteProvider {
    [SerializeField] private TunnelingVignetteController vignetteController;
    [SerializeField] private VignetteParameters parameters;

    private bool isVignetteActive = false;

    public VignetteParameters vignetteParameters => parameters;

    public void StartVignette() {
        if (vignetteController != null && !isVignetteActive) {
            vignetteController.BeginTunnelingVignette(this);
            isVignetteActive = true;
        }
    }

    public void StopVignette() {
        if (vignetteController != null && isVignetteActive) {
            vignetteController.EndTunnelingVignette(this);
            isVignetteActive = false;
        }
    }
}
