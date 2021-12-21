using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ScreenDepthNormal : MonoBehaviour
{
    private Camera cam;
    private Material mat;

    // Update is called once per frame
    void Update()
    {
        if (cam == null)
        {
            cam = this.GetComponent<Camera>();
            cam.depthTextureMode = DepthTextureMode.DepthNormals;
        }

        if (mat == null)
        {
            // Assign shader "Hidden/ScreenDepthNormal" to material
            mat = new Material(Shader.Find("Hidden/ScreenDepthNormal"));

            // For rendering geometry depth only
            //mat = new Material(Shader.Find("Custom/DepthMask"));
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (mat != null)
        {
            // Render source to screen
            //Graphics.Blit(source, destination);

            // Render source to screen with shader
            Graphics.Blit(source, destination, mat);
        }
    }
}
