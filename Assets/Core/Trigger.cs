using System;
using UnityEngine;

public class Trigger: MonoBehaviour {
    // -- config --
    [SerializeField] Action<Collider> mOnEnter;

    // -- events --
    void OnTriggerEnter(Collider other) {
        mOnEnter?.Invoke(other);
    }

    void OnTriggerStay(Collider other) {
    }

    void OnTriggerExit(Collider other) {
    }
}
