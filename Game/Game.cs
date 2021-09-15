using Godot;

public class Game: Node {
    // -- lifecycle --
    public override void _Process(float delta) {
        base._Process(delta);

        // process commands
        if (Input.IsActionJustPressed("restart")) {
            Restart();
        }
    }

    // -- commands --
    /// restart the game
    void Restart() {
        GetTree().ReloadCurrentScene();
    }
}
