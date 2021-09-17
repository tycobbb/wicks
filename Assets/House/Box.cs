using UnityEngine;

/// a box for spawning candles
public class Box: MonoBehaviour {
    // -- config --
    /// the candle prototype
    [SerializeField] GameObject mCandle;

    /// the number of candles to create
    [SerializeField] int mCount;

    // -- nodes --
    /// the root transform
    Transform mRoot;

    // -- lifecycle --
    void Awake() {
        // capture node dependencies
        mRoot = transform;
    }

    void Start() {
        // make a bunch of candles
        for (var i = 0; i < mCount; i++) {
            var obj = Instantiate(mCandle,  mRoot);
            obj.name = $"Candle{i}";
            obj.transform.localPosition = Vector3.zero;
            obj.GetComponent<Candle>().IsLit = false;
        }
    }
}
