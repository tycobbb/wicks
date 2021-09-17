using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class Pixelate: MonoBehaviour {
    // -- constants --
    static readonly int sAspectRatioId = Shader.PropertyToID("_AspectRatio");

    // -- config --
    /// the pixelate material
    [SerializeField] Material mMaterial;

    // -- lifecycle --
    void OnRenderImage(RenderTexture src, RenderTexture dst) {
        var aspect = new Vector2(1.0f, 1.0f);
        if (Screen.height > Screen.width) {
            aspect.x = (float)Screen.width / Screen.height;
        } else {
            aspect.y = (float)Screen.height / Screen.width;
        }

        mMaterial.SetVector(sAspectRatioId, aspect);
        Graphics.Blit(src, dst, mMaterial);
    }
}
