using Hertzole.GoldPlayer;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player: MonoBehaviour {
    // -- constants --
    /// the item layer
    static int sItemLayer = -1;

    /// the held item layer
    static int sHeldItemLayer = -1;

    // -- tuning --
    /// the maximum grab range
    [SerializeField] float mGrabRange = 10.0f;

    /// the radius of the grab sphere
    [SerializeField] float mGrabSlack = 0.5f;

    /// the speed the held item rotates every frame
    [SerializeField] float mGrabRotateSpeed = 4.0f;

    /// the maximum rotation for the held item
    [SerializeField] float mGrabMaxRotation = 95.0f;

    // -- nodes --
    /// the player's viewpoint
    Transform mView;

    /// the player's hand
    Transform mHand;

    // -- inputs --
    /// all the player's inputs
    InputActionAsset mInputs;

    /// the grab input
    InputAction mGrab;

    /// the grab rotate left input
    InputAction mGrabRotateLeft;

    /// the grab rotate right input
    InputAction mGrabRotateRight;

    // -- props --
    /// the held item, if any
    Rigidbody mHeld;

    /// the angle of the held item
    float mHeldAngle = 0.0f;

    /// a shared buffer for raycast hits
    readonly RaycastHit[] mHits = new RaycastHit[1];

    // -- lifecycle --
    void Awake() {
        // get node dependencies
        var t = transform;
        mView = t.Find("Head/View");
        mHand = t.Find("Hand");

        // bind inputs
        var inputs = GetComponent<GoldPlayerInputSystem>().InputAsset;
        mInputs = inputs;
        mGrab = inputs.FindAction("Grab");
        mGrabRotateLeft = inputs.FindAction("Grab-RotateLeft");
        mGrabRotateRight = inputs.FindAction("Grab-RotateRight");

        // set statics
        if (sItemLayer == -1) {
            sItemLayer = LayerMask.NameToLayer("Item");
            sHeldItemLayer = LayerMask.NameToLayer("HeldItem");
        }
    }

    void OnEnable() {
        mInputs.Enable();
    }

    void Update() {
        if (mGrab.WasPressedThisFrame()) {
            TryGrabItem();
        }

        if (mHeld != null) {
            if (mGrabRotateLeft.IsPressed()) {
                RotateItem(isLeft: true);
            }

            if (mGrabRotateRight.IsPressed()) {
                RotateItem(isLeft: false);
            }

            if (mGrab.WasReleasedThisFrame()) {
                ReleaseItem();
            }
        }
    }

    void FixedUpdate() {
        if (mHeld != null) {
            HoldItem();
        }
    }

    void OnDisable() {
        mInputs.Disable();
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
            1 << sItemLayer
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
        mHeld = item;
        mHeldAngle = 0.0f;

        // disable physics while holding the item
        item.isKinematic = true;

        // disable collisions w/ player
        item.gameObject.layer = sHeldItemLayer;

        // reorient it
        item.transform.up = Vector3.up;

        // clear its physics state
        item.velocity = Vector3.zero;
        item.angularVelocity = Vector3.zero;
    }

    /// rotate the item
    void RotateItem(bool isLeft) {
        mHeldAngle += isLeft ? mGrabRotateSpeed : -mGrabRotateSpeed;
        mHeldAngle = Mathf.Clamp(mHeldAngle, -mGrabMaxRotation, mGrabMaxRotation);
    }

    /// hold the item in place
    void HoldItem() {
        // build rotation from hand position
        var forward = mHand.forward;
        var rotation = Quaternion.LookRotation(
            forward,
            Quaternion.AngleAxis(mHeldAngle, forward) * mHand.up
        );

        // move and rotate item towards hand
        var ti = mHeld.transform;
        ti.position = Vector3.Lerp(ti.position, mHand.position, 0.1f);
        ti.rotation = Quaternion.Lerp(ti.rotation, rotation, 0.1f);
    }

    /// release the held item
    void ReleaseItem() {
        // reset the item
        mHeld.isKinematic = false;
        mHeld.gameObject.layer = sItemLayer;

        // release it from the hand
        mHeld = null;
        mHeldAngle = 0.0f;
    }
}
