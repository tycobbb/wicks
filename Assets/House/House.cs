using UnityEngine;
using Random = UnityEngine.Random;

/// a house that protects u against the elements
public class House: MonoBehaviour {
    // -- constants --
    /// a mask for objects affected by wind
    static int sWindMask = -1;

    // -- nodes --
    /// the root transform
    Transform mRoot;

    /// the house's front door
    Rigidbody mDoor;

    /// the start pos of the wind
    Transform mWindStart;

    /// the pos at which the wind grows
    Transform mWindGrow;

    // -- props --
    /// the length of the wind's first leg
    float mWindLeg1;

    /// the length of the wind's second leg
    float mWindLeg2;

    /// the list of raycast hits
    readonly RaycastHit[] mHits = new RaycastHit[64];

    // -- lifecycle --
    void Awake() {
        // get node dependencies
        var t = transform;
        mRoot = t;
        mDoor = t.Find("Door").GetComponent<Rigidbody>();
        mWindStart = t.Find("Wind/Start");
        mWindGrow = t.Find("Wind/Grow");

        // calculate wind legs
        var sPos = mWindStart.position;
        var gPos = mWindGrow.position;
        var ePos = t.Find("Wind/End").position;
        mWindLeg1 = Vector3.Distance(sPos, gPos);
        mWindLeg2 = Vector3.Distance(gPos, ePos);

        // set statics
        if (sWindMask == -1) {
            sWindMask = LayerMask.GetMask("Wind", "Flame");
        }
    }

    void FixedUpdate() {
        AddWind();
    }

    // -- commands --
    void AddWind() {
        // calc wind length
        var len = (mWindLeg1 + mWindLeg2) * Random.value;

        // capture props common to both legs
        var dir = mWindStart.forward;

        // cast the first leg through the door
        var nHits = Physics.SphereCastNonAlloc(
            mWindStart.position,
            0.3f,
            dir,
            mHits,
            Mathf.Min(mWindLeg1, len),
            sWindMask
        );

        // if we hit the door, apply wind to it and nothing else
        if (nHits != 0 && mHits[0].rigidbody == mDoor) {
            AddWind(1);
            return;
        }

        // otherwise, add wind to everything hit
        AddWind(nHits);

        // get the length of the second leg
        len -= mWindLeg1;
        if (len <= 0.0f) {
            return;
        }

        // cast the second leg from the grow point
        nHits = Physics.SphereCastNonAlloc(
            mWindGrow.position,
            2.0f,
            dir,
            mHits,
            len,
            sWindMask
        );

        // add wind to the hits
        AddWind(nHits);
    }

    void AddWind(int nHits) {
        // for each hit
        for (var i = 0; i < nHits; i++) {
            var hit = mHits[i];

            // get the rigidbody
            var body = hit.rigidbody;
            if (body == null) {
                continue;
            }

            // if door, push it open
            if (body.CompareTag("Door")) {
                body.AddForce(1.0f * mWindStart.forward);
            }
            // if candle, try to blow it out
            else if (body.CompareTag("Candle")) {
                body.GetComponent<Candle>().AddWind(
                    1.0f * mWindStart.forward
                );
            }
        }
    }
}
