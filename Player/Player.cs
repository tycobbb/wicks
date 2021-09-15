using Godot;

/// a player "controller"
public class Player: KinematicBody {
    // -- tuning --
    /// the player's forward movement speed
    [Export] float mForward = 1.0f;

    /// the player's backwards movement speed
    [Export] float mBackwards = 1.0f;

    /// the player's strafing speed
    [Export] float mStrafe = 1.0f;

    /// the player's downwards gravity
    [Export] float mGravity = 1.0f;

    /// The distance the player can grab objects from
    [Export] float mGrabDistance = 10.0f;

    /// the player's mouse sensitivity
    [Export] float mMouseSensitivity = 0.002f;

    // -- props --
    /// the player's current velocity
    Vector3 mVelocity;

    // -- nodes --
    /// the player's root node
    Spatial mRoot;

    /// the player's physics body
    KinematicBody mBody;

    /// the player's viewpoint
    RayCast mView;

    /// the player's hand
    Spatial mHand;

    // -- props --
    /// the held object, if any
    RigidBody mHeld;

    // -- lifecycle --
    public override void _Ready() {
        base._Ready();

        // capture node dependencies
        mRoot = this;
        mBody = this;
        mView = GetNode<RayCast>("View");
        mHand = GetNode<Spatial>("View/Hand");

        // capture mouse input (i.e. hide cursor)
        Input.SetMouseMode(Input.MouseMode.Captured);
    }

    public override void _PhysicsProcess(float delta) {
        base._PhysicsProcess(delta);

        DebugDraw.DrawSphere(mHand.GlobalTransform.origin, 0.1f, Colors.Green, 0.05f);

        // try to grab an object
        if (Input.IsActionJustPressed("grab")) {
            TryGrab();
        }

        // continue to hold an object if we have one
        if (mHeld != null) {
            Hold();

            // until the button is released
            if (Input.IsActionJustReleased("grab")) {
                Release();
            }
        }

        // update velocity
        Fall(delta);
        Move();

        // update body
        mVelocity = mBody.MoveAndSlide(mVelocity, Vector3.Up);
    }

    public override void _Input(InputEvent evt) {
        base._Input(evt);

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

    /// update look direction from input
    void Look(Vector2 input) {
        // rotate player horizontally
        mRoot.RotateY(-input.x * mMouseSensitivity);

        // rotate camera vertically
        mView.RotateX(input.y * mMouseSensitivity);

        // clamp rotation
        var r = mView.Rotation;
        r.x = Mathf.Clamp(r.x, -1.2f, 1.2f);
        mView.Rotation = r;
    }

    /// tries to grab the first object in view
    void TryGrab() {
        mView.Enabled = true;
        mView.CastTo = mView.Transform.basis.z * mGrabDistance;
        mView.ForceRaycastUpdate();

        var hit = mView.GetCollider();
        if (hit is Node n) {
            Grab(n);
        }

        mView.Enabled = false;
    }

    /// grabs the item
    void Grab(Node node) {
        mHeld = node.GetNode<RigidBody>("..");
        mHeld.Mode = RigidBody.ModeEnum.Kinematic;
    }

    /// holds the item in place
    void Hold() {
        mHeld.GlobalTranslate(mHand.GlobalTransform.origin - mHeld.GlobalTransform.origin);
        mHeld.LinearVelocity = Vector3.Zero;
        mHeld.AngularVelocity = Vector3.Zero;
    }

    /// release the held item
    void Release() {
        mHeld.Mode = RigidBody.ModeEnum.Rigid;
        mHeld = null;
    }
}
