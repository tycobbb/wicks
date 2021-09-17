using UnityEngine;

/// a candle that can be lit
public class Candle: MonoBehaviour {
    // -- nodes --
    /// the candle's point light
    Transform mFlame;

    /// the candle's flame
    Transform mFlameEffect;

    // -- lifecycle --
    void Awake() {
        var t = transform;

        // capture node dependencies
        mFlame = t.Find("Flame");
        mFlameEffect = t.Find("Flame/Effect");
    }

    void Update() {
        // rotate the flame
        if (mFlame) {
            RotateFlame();
        }
    }

    // -- tools --
    /// if the candle is lit or not (hot prop)
    public bool IsLit {
        get => mFlame.IsActive();
        set => mFlame.SetActive(value);
    }

    // -- commands --
    /// ensures the flame is upright
    void RotateFlame() {
        mFlameEffect.forward = Vector3.up;
    }
}
