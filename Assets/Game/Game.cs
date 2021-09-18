using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Game: MonoBehaviour {
    // -- commands --
    /// reset the current scene
    void Reset() {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    // -- events --
    /// catch the reset input event
    public void OnReset(InputAction.CallbackContext ctx) {
        Reset();
    }
}
