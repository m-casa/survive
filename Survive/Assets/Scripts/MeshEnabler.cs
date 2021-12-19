using UnityEngine;

[ExecuteInEditMode]
public class MeshEnabler : MonoBehaviour
{
    [SerializeField] private GameObject[] meshes;
    [SerializeField] private bool enableMesh = true;

    private bool meshEnabled = true;
    
    void Update()
    {
        if (enableMesh && !meshEnabled)
        {
            foreach (GameObject mesh in meshes)
            {
                mesh.GetComponent<MeshRenderer>().enabled = true;
            }

            meshEnabled = true;
        }

        if (!enableMesh && meshEnabled)
        {
            foreach (GameObject mesh in meshes)
            {
                mesh.GetComponent<MeshRenderer>().enabled = false;
            }

            meshEnabled = false;
        }
    }
}
