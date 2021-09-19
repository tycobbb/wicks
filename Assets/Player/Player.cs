using UnityEngine;

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

    // -- nodes --
    /// the player's viewpoint
    Transform mView;

    /// the player's hand
    Transform mHand;

    // -- props --
    /// the grab input
    Grab mGrab;

    /// the held item, if any
    Rigidbody mHeld;

    /// a shared buffer for raycast hits
    readonly RaycastHit[] mHits = new RaycastHit[1];

    // -- lifecycle --
    void Awake() {
        // get node dependencies
        var t = transform;
        mView = t.Find("Head/View");
        mHand = t.Find("Head/View/Hand");
        mGrab = GetComponent<Grab>();

        // set statics
        if (sItemLayer == -1) {
            sItemLayer = LayerMask.NameToLayer("Item");
            sHeldItemLayer = LayerMask.NameToLayer("HeldItem");
        }
    }

    void Update() {
        if (mGrab.WasPressedThisFrame()) {
            TryGrabItem();
        }

        if (mHeld != null && mGrab.WasReleasedThisFrame()) {
            ReleaseItem();
        }
    }

    void FixedUpdate() {
        if (mHeld != null) {
            HoldItem();
        }
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

        // disable physics while holding the item
        mHeld.isKinematic = true;

        // disable collisions w/ player
        SetHeldItemLayer(sHeldItemLayer);

        // reorient it
        mHeld.transform.up = Vector3.up;

        // clear its physics state
        mHeld.velocity = Vector3.zero;
        mHeld.angularVelocity = Vector3.zero;
    }

    /// hold the item in place
    void HoldItem() {
        // get hand direction
        var up = mHand.up;
        var right = mHand.right;
        var forward = mHand.forward;

        // build position from hand & grab
        var pos = mHand.position;
        var gd = mGrab.Offset;
        pos += forward * gd.y;
        pos += right * gd.x;

        // build rotation from hand & grab
        var rot = Quaternion.LookRotation(
            forward,
            Quaternion.AngleAxis(mGrab.Angle, forward) * up
        );

        // move and rotate item towards hand
        var ti = mHeld.transform;
        ti.position = Vector3.Lerp(ti.position, pos, 0.1f);
        ti.rotation = Quaternion.Lerp(ti.rotation, rot, 0.1f);
    }

    /// release the held item
    void ReleaseItem() {
        // reset the item
        mHeld.isKinematic = false;
        SetHeldItemLayer(sItemLayer);

        // release it from the hand
        mHeld = null;
    }

    // -- queries --
    void SetHeldItemLayer(int layer) {
        var c = mHeld.GetComponentInChildren<Collider>();
        c.gameObject.layer = layer;
    }
}
