using Hertzole.GoldPlayer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/// the player's grab action
public class Grab: MonoBehaviour {
    // -- tuning --
    /// the speed the grab rotates every frame
    [SerializeField] float mRotateSpeed = 4.0f;

    /// the maximum rotation
    [SerializeField] float mMaxRotation = 95.0f;

    /// the move speed
    [SerializeField] float mMoveSpeed = 0.1f;

    /// the maximum offset
    [SerializeField] Vector2 mMaxMoveOffset = new Vector2(0.6f, 1.0f);

    /// the look speed multiplier when moving
    [SerializeField] float mMoveLookSpeed = 0.5f;

    // -- nodes --
    /// the gold player camera
    PlayerCamera mCamera;

    /// all the player's inputs
    InputActionAsset mInputs;

    // -- inputs --
    /// the grab input
    InputAction mGrab;

    /// the move mode input
    InputAction mMoveMode;

    /// the move input
    InputAction mMove;

    /// the rotate left input
    InputAction mRotateLeft;

    /// the rotate right input
    InputAction mRotateRight;

    // -- props --
    /// the aggregate angle
    float mAngle = 0.0f;

    /// the aggregate move offset
    Vector2 mOffset;

    /// the frame the last move started
    float mMoveStartTime = 0.0f;

    /// the look sensitivity when not moving grab
    Vector2 mFreeLookSensitivity;

    // -- lifecycle --
    void Awake() {
        // get node dependencies
        mCamera = GetComponent<GoldPlayerController>().Camera;
        mInputs = GetComponent<GoldPlayerInputSystem>().InputAsset;

        // get actions
        mGrab = mInputs.FindAction("Grab");
        mMoveMode = mInputs.FindAction("Grab-MoveMode");
        mMove = mInputs.FindAction("Grab-Move");
        mRotateLeft = mInputs.FindAction("Grab-RotateLeft");
        mRotateRight = mInputs.FindAction("Grab-RotateRight");
    }

    void Start() {
        // get props
        mFreeLookSensitivity = mCamera.MouseSensitivity;
    }

    void OnEnable() {
        mInputs.Enable();
    }

    void Update() {
        // while grabbing
        if (mGrab.IsPressed()) {
            // check rotation inputs
            var isLeftPressed = mRotateLeft.IsPressed();
            var isRightPressed = mRotateRight.IsPressed();

            // if only one is pressed, rotate
            if (isLeftPressed != isRightPressed) {
                Rotate(isLeftPressed);
            }

            // if none are pressed and one was just released, finish rotate
            if (!isLeftPressed && mRotateRight.WasReleasedThisFrame() || !isRightPressed && mRotateLeft.WasReleasedThisFrame()) {
                FinishRotate();
            }

            // when move is pressed
            if (mMoveMode.WasPressedThisFrame()) {
                StartMove();
            }

            // while moving, aggregate the offset
            if (mGrab.IsPressed() && mMoveMode.IsPressed()) {
                Move();
            }

            // when move is released
            if (mMoveMode.WasReleasedThisFrame()) {
                FinishMove();
            }
        }

        // finish grab on release
        if (mGrab.WasReleasedThisFrame()) {
            FinishGrab();

            // and finish move if it was active
            if (mMoveMode.IsPressed() || mMoveMode.WasReleasedThisFrame()) {
                FinishMove();
            }
        }
    }

    void OnDisable() {
        mInputs.Disable();
    }

    // -- commands --
    /// rotate the item
    void Rotate(bool isLeft) {
        var delta = isLeft ? mRotateSpeed : -mRotateSpeed;
        mAngle = Mathf.Clamp(mAngle + delta, -mMaxRotation, mMaxRotation);
    }

    /// stop rotating the item
    void FinishRotate() {
        // snap to zero if close
        if (Mathf.Abs(mAngle) <= 25.0f) {
            mAngle = 0.0f;
        }
    }

    /// start move action
    void StartMove() {
        mMoveStartTime = Time.time;

        // lower look sensitivity
        mCamera.MouseSensitivity = mFreeLookSensitivity * mMoveLookSpeed;
    }

    /// aggregate the move
    void Move() {
        // aggregate move offset
        mOffset += mMove.ReadValue<Vector2>() * mMoveSpeed;

        // clamp offset to reasonable bounds
        var xMax = mMaxMoveOffset.x;
        var yMax = mMaxMoveOffset.y;
        mOffset.x = Mathf.Clamp(mOffset.x, -xMax, xMax);
        mOffset.y = Mathf.Clamp(mOffset.y, 0, yMax);
    }

    /// finish move action
    void FinishMove() {
        // if the move time was short, this was a tap/click
        if (Time.time - mMoveStartTime <= 0.25f) {
            // reset the offset
            mOffset = Vector2.zero;
        }

        // restore look sensitivity
        mCamera.MouseSensitivity = mFreeLookSensitivity;
    }

    /// finish grab action
    void FinishGrab() {
        mAngle = 0.0f;
        mOffset = Vector2.zero;
    }

    // -- queries --
    /// if the grab was pressed this frame
    public bool WasPressedThisFrame() {
        return mGrab.WasPressedThisFrame();
    }

    /// if the grab was released this frame
    public bool WasReleasedThisFrame() {
        return mGrab.WasReleasedThisFrame();
    }

    /// the current angle
    public float Angle {
        get => mAngle;
    }

    /// the current position offset
    public Vector2 Offset {
        get => mOffset;
    }
}
