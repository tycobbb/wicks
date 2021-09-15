using Godot;

public class Room: Spatial {
    // -- tuning --
    [Export] float mWind = 100.0f;

    // -- nodes --
    /// the wind-blocking door
    RigidBody mDoor;

    // -- lifecycle --
    public override void _Ready() {
        base._Ready();

        // capture node dependencies
        mDoor = GetNode<RigidBody>("Door");
    }

    public override void _PhysicsProcess(float delta) {
        base._PhysicsProcess(delta);

        // add forces
        AddWind();
    }

    // -- commands --
    void AddWind() {
        // add wind towards the room's right wall
        var dir = -GlobalTransform.basis.x;

        // apply force to the door
        mDoor.AddForce(dir * mWind, Vector3.Zero);
    }
}
