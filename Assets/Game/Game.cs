using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Game: MonoBehaviour {
    // -- commands --
    void Reset() {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    // -- events --
    public void OnReset(InputAction.CallbackContext ctx) {
        Reset();
    }
}
