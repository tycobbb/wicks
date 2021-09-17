using UnityEngine;

public class House: MonoBehaviour {
    // -- tuning --
    [SerializeField] float mWind = 1.0f;

    // -- nodes --
    Transform mRoot;

    /// the house's front door
    Rigidbody mDoor;

    // -- lifecycle --
    void Awake() {
        // capture node dependencies
        mRoot = GetComponent<Transform>();
        mDoor = GameObject.Find("House-door").GetComponent<Rigidbody>();
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
