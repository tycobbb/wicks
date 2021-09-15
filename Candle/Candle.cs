using Godot;

[Tool]
public class Candle: RigidBody {
    // -- constants --
    /// an upright rotation (for an emitter)
    static readonly Basis sFlameRot = new Basis {
        x = Vector3.Left,
        y = Vector3.Back,
        z = Vector3.Up,
    };

    // -- nodes --
    /// the candle's point light
    Light mLight;

    /// the candle's flame
    CPUParticles mFlame;

    // -- props --
    bool mIsLit = false;

    // -- lifecycle --
    public override void _Ready() {
        // capture node dependencies
        mLight = GetNode<Light>("Light");
        mFlame = GetNode<CPUParticles>("Flame");

        // set initial state
        SetFlame();
    }

    public override void _PhysicsProcess(float delta) {
        base._PhysicsProcess(delta);

        // rotate the flame
        if (mFlame.Visible) {
            RotateFlame();
        }
    }

    // -- tools --
    /// if the candle is lit or not (hot prop)
    [Export]
    public bool IsLit {
        get => mIsLit;
        set => SetLit(value);
    }

    // -- commands --
    /// sets the candle's lit state, lighting/extinguishing the flame
    void SetLit(bool isLit) {
        if (mIsLit != isLit) {
            mIsLit = isLit;
            SetFlame();
        }
    }

    /// syncs the flame with the current state
    void SetFlame() {
        if (mLight == null) {
            return;
        }

        if (mIsLit) {
            mLight.Show();
            mFlame.Show();
        } else {
            mLight.Hide();
            mFlame.Hide();
        }
    }

    /// ensures the flame is upright
    void RotateFlame() {
        var ft = mFlame.GlobalTransform;
        ft.basis = sFlameRot;
        mFlame.GlobalTransform = ft;
    }
}
