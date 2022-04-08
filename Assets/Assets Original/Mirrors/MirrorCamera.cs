// Attach this to a camera.
// Inverts the view of the camera so everything rendered by it is flipped

using UnityEngine;
using System.Collections;

public class MirrorCamera : MonoBehaviour
{
    Camera camera;

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    void OnPreCull()
    {
        camera.ResetWorldToCameraMatrix();
        camera.ResetProjectionMatrix();
        camera.projectionMatrix = camera.projectionMatrix * Matrix4x4.Scale(new Vector3(-1, 1, 1));
    }

    // Set it to true so we can watch the flipped Objects
    void OnPreRender()
    {
        GL.SetRevertBackfacing(true);
    }

    // Set it to false again because we dont want to affect all other cammeras.
    void OnPostRender()
    {
        GL.SetRevertBackfacing(false);
    }
}

