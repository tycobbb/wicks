using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

/// a box for spawning candles
public class Box: MonoBehaviour {
    // -- config --
    /// the candle prototype
    [SerializeField] GameObject mCandle = null;

    /// the number of candles to create
    [SerializeField] int mSize = 10;

    /// the interval between candles in seconds
    [SerializeField] float mSpawnRate = 1.0f;

    // -- nodes --
    /// the root transform
    Transform mRoot;

    // -- props --
    /// the number of candles in the box
    int mCurr = 0;

    /// the total number of candles spawned
    int mTotal = 0;

    // -- lifecycle --
    void Awake() {
        // capture node dependencies
        mRoot = transform;
    }

    void Start() {
        StartCoroutine(SpawnCandles());
    }

    // -- commands --
    /// spawn a candle every so often as needed
    IEnumerator SpawnCandles() {
        while (true) {
            if (mCurr < mSize) {
                // get next candle id
                var next = mTotal + 1;

                // spawn a candle
                var obj = Instantiate(mCandle, mRoot);
                obj.name = $"Candle{next}";
                obj.transform.localPosition = Vector3.zero;
                obj.GetComponent<Candle>().IsLit = false;

                // update counts
                mTotal = next;
            }

            // wait to check again
            yield return new WaitForSeconds(mSpawnRate);
        }
    }

    // -- events --
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Candle")) {
            mCurr += 1;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Candle")) {
            mCurr -= 1;
        }
    }
}
