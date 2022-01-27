using Mirror;
using UnityEngine;

public class NetworkAnimations : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleAnimationClipUpdated))]
    private string currentAnimationClip;

    [SerializeField] private ClassicCharacter character;

    public void SetAnimationClip(string newAnimationClip)
    {
        currentAnimationClip = newAnimationClip;
    }

    private void HandleAnimationClipUpdated(string oldAnimationClip, string newAnimationClip)
    {
        // Don't update the local player's animation
        //  since they're updating it themselves
        if (!isLocalPlayer)
            character.SyncAnimationClip(newAnimationClip);
    }
}
