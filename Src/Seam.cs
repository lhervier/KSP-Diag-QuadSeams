using UnityEngine;

namespace com.github.lhervier.ksp.terrainprecisionfixdiag4
{
    /// <summary>
    /// One side of a quad of the highest subdivision level that lies against a coarser quad, and how far
    /// apart the two quads put the vertices they are supposed to share.
    /// </summary>
    internal sealed class Seam
    {
        /// <summary>The quad of the highest level.</summary>
        public PQ Fine { get; }

        /// <summary>The side of <see cref="Fine"/> along which the seam runs.</summary>
        public Side FineSide { get; }

        /// <summary>The coarser quad against it.</summary>
        public PQ Coarse { get; }

        /// <summary>The side of <see cref="Coarse"/> that faces <see cref="Fine"/>.</summary>
        public Side CoarseSide { get; }

        /// <summary>Number of vertices of the fine side that were matched with a vertex of the coarse
        /// side, at the last <see cref="Measure"/>.</summary>
        public int Matched { get; private set; }

        /// <summary>Number of vertices of the fine side that should have had a counterpart on the coarse
        /// side and had none close enough, at the last <see cref="Measure"/>.</summary>
        public int Unmatched { get; private set; }

        /// <summary>Largest distance, in metres, between a matched vertex and its counterpart, at the last
        /// <see cref="Measure"/>.</summary>
        public double MaxGap { get; private set; }

        /// <summary>Sum of the distances, in metres, of the matched vertices, at the last
        /// <see cref="Measure"/>.</summary>
        public double SumGap { get; private set; }

        /// <summary>Index, in the vertices of <see cref="Fine"/>, of the matched vertex with the largest
        /// gap, at the last <see cref="Measure"/>; -1 when none was matched.</summary>
        public int WorstVertex { get; private set; }

        /// <summary>World position of <see cref="WorstVertex"/>, at the last <see cref="Measure"/>.</summary>
        public Vector3d WorstPosition { get; private set; }

        /// <summary>From its counterpart to <see cref="WorstVertex"/>, in world axes, at the last
        /// <see cref="Measure"/>.</summary>
        public Vector3d WorstGap { get; private set; }

        // Scratch buffer for the world positions of the coarse side.
        private static Vector3d[] coarseWorld = new Vector3d[0];

        public Seam(PQ fine, Side fineSide, PQ coarse, Side coarseSide)
        {
            Fine = fine;
            FineSide = fineSide;
            Coarse = coarse;
            CoarseSide = coarseSide;
        }

        /// <summary>
        /// Measures, in the world, the distance between each vertex of the fine side that the game keeps in
        /// its triangles and the vertex of the coarse side it should land on.
        /// </summary>
        public void Measure()
        {
            Matched = 0;
            Unmatched = 0;
            MaxGap = 0.0;
            SumGap = 0.0;
            WorstVertex = -1;

            int count = QuadGrid.SideVertices;
            if (coarseWorld.Length != count)
            {
                coarseWorld = new Vector3d[count];
            }

            // Both sides taken through the matrices the quads are drawn with, but computed in double: the
            // distance is then the one between the two places the game puts the vertices, without the
            // rounding a float world position would add to it.
            Matrix4x4 coarseMatrix = Coarse.transform.localToWorldMatrix;
            Vector3[] coarseVerts = Coarse.verts;
            for (int i = 0; i < count; i++)
            {
                coarseWorld[i] = ToWorld(coarseMatrix, coarseVerts[QuadGrid.EdgeVertex(CoarseSide, i)]);
            }

            // Along a stitched side, the finer quad keeps one vertex in two (the even ones) and runs its
            // edge on them. The coarse side is twice as long with the same number of vertices, so its
            // vertices are twice as far apart: a fine vertex further than a quarter of that spacing from
            // every coarse vertex has no counterpart at all, which would mean this reading of the grid is
            // wrong.
            double coarseSpacing = (coarseWorld[1] - coarseWorld[0]).magnitude;
            double tolerance = coarseSpacing / 4.0;

            Matrix4x4 fineMatrix = Fine.transform.localToWorldMatrix;
            Vector3[] fineVerts = Fine.verts;
            for (int i = 0; i < count; i += 2)
            {
                int vertex = QuadGrid.EdgeVertex(FineSide, i);
                Vector3d fine = ToWorld(fineMatrix, fineVerts[vertex]);
                double nearest = double.MaxValue;
                Vector3d gap = Vector3d.zero;
                for (int j = 0; j < count; j++)
                {
                    Vector3d candidate = fine - coarseWorld[j];
                    double d = candidate.magnitude;
                    if (d < nearest)
                    {
                        nearest = d;
                        gap = candidate;
                    }
                }
                if (nearest > tolerance)
                {
                    Unmatched++;
                    continue;
                }
                Matched++;
                SumGap += nearest;
                if (WorstVertex < 0 || nearest > MaxGap)
                {
                    MaxGap = nearest;
                    WorstVertex = vertex;
                    WorstPosition = fine;
                    WorstGap = gap;
                }
            }
        }

        /// <summary>Where <paramref name="matrix"/> puts <paramref name="local"/>, computed in
        /// double.</summary>
        private static Vector3d ToWorld(Matrix4x4 matrix, Vector3 local)
        {
            double x = local.x;
            double y = local.y;
            double z = local.z;
            return new Vector3d(
                matrix.m00 * x + matrix.m01 * y + matrix.m02 * z + matrix.m03,
                matrix.m10 * x + matrix.m11 * y + matrix.m12 * z + matrix.m13,
                matrix.m20 * x + matrix.m21 * y + matrix.m22 * z + matrix.m23
            );
        }
    }
}
