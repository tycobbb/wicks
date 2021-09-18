using UnityEngine;

/// a house that protects u against the elements
public class House: MonoBehaviour {
    // -- tuning --
    [SerializeField] float mWind = 1.0f;

    // -- nodes --
    /// the root transform
    Transform mRoot;

    /// the house's front door
    Rigidbody mDoor;

    // -- lifecycle --
    void Awake() {
        var t = transform;

        // get node dependencies
        mRoot = t;
        mDoor = t.Find("Door").GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        AddWind();
    }

    // -- commands --
    void AddWind() {
        // add wind towards the room's back wall
        var dir = -mRoot.forward;

        // apply force to the door
        mDoor.AddForce(dir * mWind);
    }
}
