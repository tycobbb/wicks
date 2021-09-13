using Godot;

public class Player: KinematicBody {
    // -- config --
    /// the player's forward movement speed
    [Export] float mForward = 1.0f;

    /// the player's backwards movement speed
    [Export] float mBackwards = 1.0f;

    /// the player's strafing speed
    [Export] float mStrafe = 1.0f;

    /// the player's downwards gravity
    [Export] float mGravity = 1.0f;

    /// the player's mouse sensitivity
    [Export] float mMouseSensitivity = 0.002f;

    // -- props --
    /// the player's current velocity
    Vector3 mVelocity;

    // -- nodes --
    /// the player's root node
    Spatial mRoot => this;

    /// the player's physics body
    KinematicBody mBody => this;

    /// the attached camera node
    Spatial mLook => GetNode<Spatial>("Player-look");

    // -- lifecycle --
    public override void _Ready() {
        base._Ready();

        // capture mouse input (i.e. hide cursor)
        Input.SetMouseMode(Input.MouseMode.Captured);
    }

    public override void _PhysicsProcess(float delta) {
        base._PhysicsProcess(delta);

        // update velocity
        Fall(delta);
        Move();

        // update body
        mVelocity = mBody.MoveAndSlide(mVelocity, Vector3.Up);
    }

    public override void _UnhandledInput(InputEvent evt) {
        base._UnhandledInput(evt);

        // if this is a mouse event
        if (evt is InputEventMouseMotion e && Input.GetMouseMode() == Input.MouseMode.Captured) {
            Look(e.Relative);
        }
    }

    // -- commands --
    /// add gravity
    void Fall(float delta) {
        mVelocity.y -= mGravity * delta;
    }

    /// update velocity from input
    void Move() {
        // gather local velocity from input
        var vl = new Vector3();

        if (Input.IsActionPressed("move_forward")) {
            vl.z += mForward;
        }

        if (Input.IsActionPressed("move_backward")) {
            vl.z -= mBackwards;
        }

        if (Input.IsActionPressed("move_left")) {
            vl.x += mStrafe;
        }

        if (Input.IsActionPressed("move_right")) {
            vl.x -= mStrafe;
        }

        // update current velocity
        var b = mRoot.Transform.basis;
        var v = b.x * vl.x + b.z * vl.z;
        mVelocity.x = v.x;
        mVelocity.z = v.z;
    }

    // update look direction from input
    void Look(Vector2 input) {
        // rotate player horizontally
        mRoot.RotateY(-input.x * mMouseSensitivity);

        // rotate camera vertically
        mLook.RotateX(input.y * mMouseSensitivity);

        // clamp rotation
        var r = mLook.Rotation;
        r.x = Mathf.Clamp(r.x, -1.2f, 1.2f);
        mLook.Rotation = r;
    }
}
