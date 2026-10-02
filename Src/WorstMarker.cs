using UnityEngine;

namespace com.github.lhervier.ksp.diag.quadseams
{
    /// <summary>
    /// A vertical line that rises from one vertex of one quad, to point the camera at it from far away.
    /// It follows the quad wherever the game moves it.
    /// </summary>
    internal sealed class WorstMarker
    {
        /// <summary>Height of the line above the vertex, in metres.</summary>
        public const float LENGTH = 1000f;

        private readonly Mesh mesh;
        private PQ quad;
        private Vector3 vertex;

        /// <summary>Creates the marker, pointing at nothing.</summary>
        public WorstMarker()
        {
            mesh = new Mesh { name = "KSPDiagQuadSeams worst vertex" };
            mesh.vertices = new[] { Vector3.zero, Vector3.up * LENGTH };
            // The colour comes from the material; the shader multiplies it by this one.
            mesh.colors32 = new[] { new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 255) };
            mesh.SetIndices(new[] { 0, 1 }, MeshTopology.Lines, 0);
            mesh.RecalculateBounds();
        }

        /// <summary>Points the marker at vertex <paramref name="index"/> of <paramref name="target"/>, or at
        /// nothing when <paramref name="target"/> is null.</summary>
        public void PointAt(PQ target, int index)
        {
            quad = target;
            if (quad != null)
            {
                // A copy: the quad may be built again elsewhere before the next refresh.
                vertex = quad.verts[index];
            }
        }

        /// <summary>Queues the marker for this frame, along the vertical of <paramref name="body"/> at the
        /// vertex, unless it points at nothing the game still draws.</summary>
        public void Draw(Material material, CelestialBody body)
        {
            if (quad == null || !quad.isVisible || quad.isSubdivided || body == null)
            {
                return;
            }
            Vector3 world = quad.transform.TransformPoint(vertex);
            Vector3 up = ((Vector3d)world - body.position).normalized;
            Matrix4x4 matrix = Matrix4x4.TRS(world, Quaternion.FromToRotation(Vector3.up, up), Vector3.one);
            Graphics.DrawMesh(mesh, matrix, material, quad.gameObject.layer);
        }

        /// <summary>Releases the mesh. The marker cannot be drawn afterwards.</summary>
        public void Destroy()
        {
            Object.Destroy(mesh);
        }
    }
}
