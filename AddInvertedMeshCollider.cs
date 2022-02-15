using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class AddInvertedMeshCollider : MonoBehaviour {
    public bool removeExistingColliders = true;

    public void CreateInvertedMeshCollider() {
        if (removeExistingColliders)
            RemoveExistingColliders();

        InvertMesh();

        var collider = gameObject.AddComponent<MeshCollider>();
        collider.convex = true;
    }

    private void RemoveExistingColliders() {
        Collider[] colliders = GetComponents<Collider>();
        for (int i = 0; i < colliders.Length; i++)
            DestroyImmediate(colliders[i]);
    }

    private void InvertMesh() {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.triangles = mesh.triangles.Reverse().ToArray();
    }
}
