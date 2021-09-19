using Hertzole.GoldPlayer;
using UnityEngine;
using UnityEngine.InputSystem;

/// the player's grab action
public class Grab: MonoBehaviour {
    // -- tuning --
    /// the speed the grab rotates every frame
    [SerializeField] float mRotateSpeed = 4.0f;

    /// the maximum rotation
    [SerializeField] float mMaxRotation = 95.0f;

    /// the maximum offset
    [SerializeField] float mMaxOffset = 1.0f;

    // -- nodes --
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

    /// the default look input
    InputAction mLook;

    // -- props --
    /// the aggregate angle
    float mAngle = 0.0f;

    /// the aggregate move offset
    Vector2 mOffset;

    // -- lifecycle --
    void Awake() {
        // get node dependencies
        mInputs = GetComponent<GoldPlayerInputSystem>().InputAsset;

        // get actions
        mGrab = mInputs.FindAction("Grab");
        mMoveMode = mInputs.FindAction("Grab-MoveMode");
        mMove = mInputs.FindAction("Grab-Move");
        mRotateLeft = mInputs.FindAction("Grab-RotateLeft");
        mRotateRight = mInputs.FindAction("Grab-RotateRight");
        mLook = mInputs.FindAction("Look");
    }

    void OnEnable() {
        mInputs.Enable();
    }

    void Update() {
        // while grabbing
        if (mGrab.IsPressed()) {
            // aggregate the rotation
            if (mRotateLeft.IsPressed()) {
                Rotate(isLeft: true);
            }

            if (mRotateRight.IsPressed()) {
                Rotate(isLeft: false);
            }

            // when move is pressed, disable look
            if (mMoveMode.WasPressedThisFrame()) {
                StartMove();
            }

            // while moving, aggregate the offset
            if (mGrab.IsPressed() && mMoveMode.IsPressed()) {
                Move();
            }

            // when move is released, reset move state
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
        mAngle += isLeft ? mRotateSpeed : -mRotateSpeed;
        mAngle = Mathf.Clamp(mAngle, -mMaxRotation, mMaxRotation);
    }

    /// start move action
    void StartMove() {
        mLook.Disable();
    }

    /// aggregate the move
    void Move() {
        mOffset += mMove.ReadValue<Vector2>();;

        // clamp offset to reasonable bounds
        mOffset.x = Mathf.Clamp(mOffset.x, -mMaxOffset, mMaxOffset);
        mOffset.y = Mathf.Clamp(mOffset.y, 0, mMaxOffset);
    }

    /// finish move action
    void FinishMove() {
        mLook.Enable();
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
