using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    #region ------ Class Variables ------
    [Header("Meta XR Hand Grab Interactors")]
    [SerializeField] HandGrabInteractor interactorLeft;
    [SerializeField] HandGrabInteractor interactorRight;

    [SerializeField] Rigidbody rightRigidBody;
    #endregion

    #region ------ Unity Lifecycle Functions ------
    void Start()
    {
        if (interactorLeft == null) Debug.LogError("[PlayerGrab] Missing interactorLeft");
        if (interactorLeft == null) Debug.LogError("[PlayerGrab] Missing interactorRight");
    }


    private void OnEnable()
    {
        CatchVRGameManager.OnSpawnApple += AutoGrabObject;
        CatchVRGameManager.OnHoveringBasket += ReleaseObjects;
    }


    private void OnDisable()
    {
        CatchVRGameManager.OnSpawnApple += AutoGrabObject;
        CatchVRGameManager.OnHoveringBasket += ReleaseObjects;
    }
    #endregion

    #region ------ Private Methods ------
    /// <summary>
    /// Sets the next interactable object that each hand can auto-grab.
    /// </summary>
    /// <param name="interactable">The interactable to target.</param>
    private void AutoGrabObject(HandGrabInteractable interactable)
    {
        interactorLeft.ForceSelect(interactable);
        interactorRight.ForceSelect(interactable);
    }


    /// <summary>
    /// Releases all objects in both hands. Asssumes that only one
    /// catchable object is in play.
    /// </summary>
    private void ReleaseObjects()
    {
        interactorLeft.ForceRelease();
        interactorRight.ForceRelease();
    }

    private void Clear()
    {
        interactorLeft.ForceSelect(null);
        interactorRight.ForceSelect(null);
    }
    #endregion
}
