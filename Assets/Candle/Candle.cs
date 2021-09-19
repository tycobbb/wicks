using UnityEngine;

/// a candle that can be lit
public class Candle: MonoBehaviour {
    // -- constants --
    static int sWickLayer = -1;
    static int sFlameLayer = -1;

    // -- tuning --
    /// the wind speed that extinguishes the candle
    [SerializeField] float mWindThreshold = 1.0f;

    // -- nodes --
    /// the candle's wick
    GameObject mWick;

    /// the candle's lit flame
    Transform mFlame;

    /// the candle's flame
    Transform mFlameEffect;

    // -- props --
    /// the wind currently affecting the candle
    float mWind;

    // -- lifecycle --
    void Awake() {
        var t = transform;

        // get node dependencies
        mWick = t.Find("Wick").gameObject;
        mFlame = t.Find("Wick/Flame");
        mFlameEffect = t.Find("Wick/Flame/Effect");

        // set statics
        if (sWickLayer == -1) {
            sWickLayer = LayerMask.NameToLayer("Wick");
            sFlameLayer = LayerMask.NameToLayer("Flame");
        }
    }

    void Start() {
        // ensure the candle is configured properly if initially lit
        if (IsLit) {
            SetLit(true);
        }
    }

    void Update() {
        // rotate the flame
        if (mFlame) {
            mFlameEffect.forward = Vector3.up;
        }
    }

    // -- tools --
    /// if the candle is lit or not (hot prop)
    public bool IsLit {
        get => mFlame.IsActive();
        set => SetLit(value);
    }

    // -- commands --
    /// apply wind force to the candle
    public void AddWind(float wind) {
        mWind = wind;
        if (!IsLit) {
            return;
        }

        if (mWind > mWindThreshold) {
            SetLit(false);
        }
    }

    /// lights/extinguishes the candle
    void SetLit(bool isLit) {
        // flip layer so flames and wicks can collide
        mWick.layer = isLit ? sFlameLayer : sWickLayer;

        // turn on the light/particles
        mFlame.SetActive(isLit);
    }

    // -- events --
    void OnTriggerEnter(Collider other) {
        if (!IsLit) {
            return;
        }

        var candle = other.GetComponent<Candle>();
        if (candle != null && !candle.IsLit) {
            candle.IsLit = true;
        }
    }
}
