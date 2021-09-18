using System;
using Hertzole.GoldPlayer;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player: MonoBehaviour {
    // -- constants --
    /// a mask for the item layer
    static int sItemMask = -1;

    // -- tuning --
    /// the maximum grab range
    [SerializeField] float mGrabRange = 10.0f;

    /// the radius of the grab sphere
    [SerializeField] float mGrabSlack = 0.5f;

    // -- nodes --
    /// the grab action
    InputAction mGrab;

    /// the player's viewpoint
    Transform mView;

    /// the player's hand
    FixedJoint mHand;

    // -- props --
    /// a shared buffer for raycast hits
    readonly RaycastHit[] mHits = new RaycastHit[1];

    // -- lifecycle --
    void Awake() {
        var t = transform;
        var inputs = GetComponent<GoldPlayerInputSystem>().InputAsset;

        // get node dependencies
        mGrab = inputs.FindAction("grab");
        mView = t.Find("Head/View");
        mHand = t.Find("Hand").GetComponent<FixedJoint>();

        // set statics
        if (sItemMask == -1) {
            sItemMask = LayerMask.GetMask("Item");
        }
    }

    void OnEnable() {
        mGrab.Enable();
    }

    void Update() {
        if (mGrab.WasPressedThisFrame()) {
            TryGrabItem();
        }

        if (IsHoldingItem && mGrab.WasReleasedThisFrame()) {
            ReleaseItem();
        }
    }

    void OnDisable() {
        mGrab.Enable();
    }

    // -- commands--
    /// tries to grab the first item in view
    void TryGrabItem() {
        var nHits = Physics.SphereCastNonAlloc(
            mView.position,
            mGrabSlack,
            mView.forward,
            mHits,
            mGrabRange,
            sItemMask
        );

        if (nHits <= 0) {
            return;
        }

        var body = mHits[0].rigidbody;
        if (body != null) {
            GrabItem(body);
        }
    }

    /// grabs the item
    void GrabItem(Rigidbody item) {
        // move the item into the hand
        var ti = item.transform;
        ti.up = Vector3.up;
        ti.position = mHand.transform.position;

        // clear the item's physics state
        item.velocity = Vector3.zero;
        item.angularVelocity = Vector3.zero;

        // weld it to the hand
        mHand.SetActive(true);
        mHand.connectedBody = item;
    }

    /// release the held item
    void ReleaseItem() {
        // remove the item from the joint
        mHand.connectedBody = null;
        mHand.SetActive(false);
    }

    // -- queries --
    /// if the player is holding an item
    bool IsHoldingItem {
        get => mHand.IsActive();
    }
}
