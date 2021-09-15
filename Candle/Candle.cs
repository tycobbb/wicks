using Godot;

[Tool]
public class Candle: RigidBody {
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
}
