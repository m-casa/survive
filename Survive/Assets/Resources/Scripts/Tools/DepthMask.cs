using UnityEngine;

public class DepthMask : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] meshArray;
    [SerializeField] private Material depthMask;

    void Awake()
    {
        foreach (MeshRenderer mesh in meshArray)
        {
            var mats = mesh.sharedMaterials;

            for (int i = 0; i < mesh.sharedMaterials.Length; i++)
            {
                mats[i] = depthMask;
            }

            mesh.sharedMaterials = mats;
        }
    }
}
