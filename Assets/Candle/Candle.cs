using UnityEngine;

/// a candle that can be lit
public class Candle: MonoBehaviour {
    // -- nodes --
    /// the candle's lit flame
    Transform mFlame;

    /// the candle's flame
    Transform mFlameEffect;

    // -- lifecycle --
    void Awake() {
        var t = transform;

        // get node dependencies
        mFlame = t.Find("Wick/Flame");
        mFlameEffect = t.Find("Wick/Flame/Effect");
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
    /// apply wind force to the candle
    public void AddWind(Vector3 wind) {
        Debug.Log($"blowing on {name} @ {Time.time}");
    }

    /// light the candle
    void Light() {
        IsLit = true;
    }

    /// ensures the flame is upright
    void RotateFlame() {
        mFlameEffect.forward = Vector3.up;
    }

    // -- events --
    void OnTriggerEnter(Collider other) {
        if (!IsLit) {
            return;
        }

        var candle = other.GetComponent<Candle>();
        if (candle != null && !candle.IsLit) {
            candle.Light();
        }
    }
}
