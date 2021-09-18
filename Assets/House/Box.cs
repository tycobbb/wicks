using System.Collections;
using UnityEngine;

/// a box for spawning candles
public class Box: MonoBehaviour {
    // -- config --
    /// the candle prototype
    [SerializeField] GameObject mCandle = null;

    /// the number of candles to create
    [SerializeField] int mCount = 420;

    /// the number of candles to spawn per frame
    [SerializeField] int mSpawnRate = 5;

    // -- nodes --
    /// the root transform
    Transform mRoot;

    // -- lifecycle --
    void Awake() {
        // capture node dependencies
        mRoot = transform;
    }

    void Start() {
        // spawn a bunch of candles
        StartCoroutine(SpawnCandles());
    }

    // -- commands --
    /// spawn candles over a few frames
    IEnumerator SpawnCandles() {
        // make a bunch of candles
        for (var i = 1; i <= mCount; i++) {
            var obj = Instantiate(mCandle,  mRoot);
            obj.name = $"Candle{i}";
            obj.transform.localPosition = Vector3.zero;
            obj.GetComponent<Candle>().IsLit = false;

            if (i % mSpawnRate == 0) {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
