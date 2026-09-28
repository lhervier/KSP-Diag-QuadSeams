using System.Collections.Generic;
using UnityEngine;

namespace com.github.lhervier.ksp.terrainprecisionfixdiag4
{
    /// <summary>
    /// The drawing of some sides of one quad: the triangles its mesh has along those sides, filled, and
    /// their edges. It is built in the frame of the quad, from the quad's own vertices and the very
    /// triangle list the game draws the quad with, so it lands where the terrain is drawn, wherever the
    /// quad is moved to.
    /// </summary>
    internal sealed class EdgeOverlay
    {
        // Sub-meshes of the drawing.
        private const int FILL = 0;
        private const int LINES = 1;

        // Scratch buffers shared by every rebuild, which all happen on the main thread.
        private static readonly List<int> FILL_INDICES = new List<int>();
        private static readonly List<int> LINE_INDICES = new List<int>();
        private static bool[] cellTaken = new bool[0];
        private static Color32[] white = new Color32[0];

        /// <summary>The quad this overlay draws.</summary>
        public PQ Quad { get; }

        private readonly Mesh mesh;

        // What the drawing was built from. Stock keeps a quad object alive and builds it again elsewhere
        // when the terrain changes, so the same quad can come back with other vertices or another
        // triangle list.
        private int builtSides = -1;
        private int[] builtTriangles;
        private Vector3[] builtVerts;
        private Vector3d builtPosition;
        private Vector3 builtFirstVertex;
        private Vector3 builtLastVertex;

        /// <summary>Creates the overlay of <paramref name="quad"/>, with nothing drawn yet.</summary>
        public EdgeOverlay(PQ quad)
        {
            Quad = quad;
            mesh = new Mesh { name = "TerrainPrecisionFixDiag4 edge of " + quad.name };
            mesh.MarkDynamic();
        }

        /// <summary>
        /// Makes the drawing match the triangles <see cref="Quad"/> has along <paramref name="sides"/>
        /// right now, a mask of <see cref="Side"/> values. Does nothing when it already does.
        /// </summary>
        public void Refresh(int sides)
        {
            Vector3[] verts = Quad.verts;
            int[] triangles = QuadGrid.Triangles(Quad);
            if (sides == builtSides
                && triangles == builtTriangles
                && verts == builtVerts
                && Quad.positionPlanetRelative == builtPosition
                && verts[0].Equals(builtFirstVertex)
                && verts[verts.Length - 1].Equals(builtLastVertex))
            {
                return;
            }

            int cells = QuadGrid.Cells;
            if (cellTaken.Length != cells * cells)
            {
                cellTaken = new bool[cells * cells];
            }
            System.Array.Clear(cellTaken, 0, cellTaken.Length);
            FILL_INDICES.Clear();
            LINE_INDICES.Clear();

            foreach (Side side in QuadGrid.SIDES)
            {
                if ((sides & (int)side) == 0)
                {
                    continue;
                }
                for (int i = 0; i < cells; i++)
                {
                    // A corner cell runs along two sides: draw it once.
                    int cell = QuadGrid.EdgeCell(side, i);
                    if (cellTaken[cell / 6])
                    {
                        continue;
                    }
                    cellTaken[cell / 6] = true;
                    AddTriangle(triangles, cell);
                    AddTriangle(triangles, cell + 3);
                }
            }

            // The quad's whole vertex array, even though only the edge uses it: the triangle lists index
            // into it as they are.
            if (white.Length != verts.Length)
            {
                white = new Color32[verts.Length];
                for (int i = 0; i < white.Length; i++)
                {
                    white[i] = new Color32(255, 255, 255, 255);
                }
            }
            mesh.Clear();
            mesh.subMeshCount = 2;
            mesh.vertices = verts;
            // The colour comes from the material; the shader multiplies it by this one.
            mesh.colors32 = white;
            mesh.SetIndices(FILL_INDICES.ToArray(), MeshTopology.Triangles, FILL);
            mesh.SetIndices(LINE_INDICES.ToArray(), MeshTopology.Lines, LINES);
            mesh.RecalculateBounds();

            builtSides = sides;
            builtTriangles = triangles;
            builtVerts = verts;
            builtPosition = Quad.positionPlanetRelative;
            builtFirstVertex = verts[0];
            builtLastVertex = verts[verts.Length - 1];
        }

        /// <summary>Adds the triangle that starts at <paramref name="at"/> in <paramref name="triangles"/>
        /// to the drawing, unless it is one stock has flattened to nothing.</summary>
        private static void AddTriangle(int[] triangles, int at)
        {
            int a = triangles[at];
            int b = triangles[at + 1];
            int c = triangles[at + 2];

            // Where stock stitches a side, it drops a triangle by pointing its three corners at vertex 0.
            if (a == b || b == c || c == a)
            {
                return;
            }
            FILL_INDICES.Add(a);
            FILL_INDICES.Add(b);
            FILL_INDICES.Add(c);
            LINE_INDICES.Add(a);
            LINE_INDICES.Add(b);
            LINE_INDICES.Add(b);
            LINE_INDICES.Add(c);
            LINE_INDICES.Add(c);
            LINE_INDICES.Add(a);
        }

        /// <summary>
        /// Queues the drawing for this frame, where the quad is now, unless the game does not draw the
        /// quad itself any more.
        /// </summary>
        public void Draw(Material fill, Material lines)
        {
            if (Quad == null || !Quad.isVisible || Quad.isSubdivided || builtSides <= 0)
            {
                return;
            }

            // Same matrix and same layer as the quad's own renderer: the same cameras draw both, at the
            // same place.
            Matrix4x4 matrix = Quad.transform.localToWorldMatrix;
            int layer = Quad.gameObject.layer;
            Graphics.DrawMesh(mesh, matrix, fill, layer, null, FILL);
            Graphics.DrawMesh(mesh, matrix, lines, layer, null, LINES);
        }

        /// <summary>Releases the mesh. The overlay cannot be drawn afterwards.</summary>
        public void Destroy()
        {
            Object.Destroy(mesh);
        }
    }
}
