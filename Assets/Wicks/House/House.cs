using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

/// a house that protects u against the elements
public class House: MonoBehaviour {
    // -- constants --
    /// a mask for objects affected by wind
    static int sWindMask = -1;

    // -- tuning --
    /// the minimum wind force
    [SerializeField] [Range(0.0f, 10.0f)] float mWindMin = 0.0f;

    /// the maximum wind force
    [SerializeField] [Range(0.0f, 10.0f)] float mWindMax = 1.0f;

    /// the curve applied when seeding the initial wind distance
    [SerializeField] AnimationCurve mWindDist = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    /// the curve applied when seeding the initial wind speed
    [FormerlySerializedAs("mWindCurve")] [SerializeField]
    AnimationCurve mWindSpeed = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    /// the curve applied when adding force to objects as a function of distance from
    /// the wind source
    [FormerlySerializedAs("mWindDistCurve")] [SerializeField]
    AnimationCurve mWindFalloff = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);

    // -- nodes --
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
        // calc wind attrs
        var spd = Mathf.Lerp(
            mWindMin,
            mWindMax,
            mWindSpeed.Evaluate(Random.value)
        );

        var len = Mathf.Lerp(
            0.0f,
            mWindLeg1 + mWindLeg2,
            mWindDist.Evaluate(Random.value)
        );

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
            AddWind(1, spd);
            return;
        }

        // otherwise, add wind to everything hit
        AddWind(nHits, spd);

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
        AddWind(nHits, spd);
    }

    void AddWind(int nHits, float speed) {
        // calc start pos and max distance
        var z0 = mWindStart.position.z;
        var dMax = mWindLeg1 + mWindLeg2;

        // for each hit
        for (var i = 0; i < nHits; i++) {
            var hit = mHits[i];

            // get the rigidbody
            var body = hit.rigidbody;
            if (body == null) {
                continue;
            }

            // if door, push it open
            if (body == mDoor) {
                body.AddForce(speed * mWindStart.forward);
            }
            // if candle, try to blow it out
            else if (body.CompareTag("Candle")) {
                // determine wind speed as a fn of z distance from source
                var dz = body.transform.position.z - z0;
                var pct = Mathf.InverseLerp(0.0f, dMax, dz);
                var spd = speed * mWindFalloff.Evaluate(pct);

                // add the wind to the candle
                body.GetComponent<Candle>().AddWind(spd);
            }
        }
    }
}
