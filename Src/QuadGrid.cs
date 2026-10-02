namespace com.github.lhervier.ksp.diag.quadseams
{
    /// <summary>
    /// The four sides of a quad. Each value is the bit stock uses for that side in
    /// <see cref="PQS.EdgeState"/>, so a set of sides is a mask the game itself understands.
    /// </summary>
    internal enum Side
    {
        North = 1,
        South = 2,
        East = 4,
        West = 8,
    }

    /// <summary>
    /// Where the sides of a quad are in the grid its mesh is built on: which neighbour lies across each
    /// side, which cells run along it, and which vertices it is made of. The grid is the one every quad
    /// shares (<see cref="PQS.cacheSideVertCount"/> vertices by side, <see cref="PQS.cacheRes"/> cells).
    /// </summary>
    internal static class QuadGrid
    {
        /// <summary>The four sides, in the order they are looked at.</summary>
        public static readonly Side[] SIDES = { Side.North, Side.South, Side.East, Side.West };

        /// <summary>Number of cells along a side of a quad.</summary>
        public static int Cells => PQS.cacheRes;

        /// <summary>Number of vertices along a side of a quad.</summary>
        public static int SideVertices => PQS.cacheSideVertCount;

        /// <summary>The quad stock links to <paramref name="quad"/> across <paramref name="side"/>, or
        /// null.</summary>
        public static PQ Neighbour(PQ quad, Side side)
        {
            switch (side)
            {
                case Side.North: return quad.north;
                case Side.South: return quad.south;
                case Side.East: return quad.east;
                default: return quad.west;
            }
        }

        /// <summary>
        /// The side of <paramref name="coarse"/> that faces <paramref name="fine"/>, a quad of a higher
        /// subdivision level that lies against it; or null when none does.
        /// </summary>
        public static Side? FacingSide(PQ coarse, PQ fine)
        {
            // A coarse quad does not link to the finer quads against it, but to the quad of its own level
            // they were split from: the side we want is the one that leads to an ancestor of the fine
            // quad. Comparing links rather than directions also holds across the edge of a cube face,
            // where the two quads are not turned the same way.
            foreach (Side side in SIDES)
            {
                PQ across = Neighbour(coarse, side);
                if (across == null)
                {
                    continue;
                }
                for (PQ ancestor = fine; ancestor != null; ancestor = ancestor.parent)
                {
                    if (ancestor == across)
                    {
                        return side;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Offset, in a triangle index list of <see cref="PQS.cacheIndices"/>, of the
        /// <paramref name="i"/>-th cell along <paramref name="side"/>. A cell holds two triangles, that
        /// is six indices.
        /// </summary>
        public static int EdgeCell(Side side, int i)
        {
            // Cells are laid out row by row, as in PQS.ti(x, z). The sides are named after the part of the
            // grid PQS.CreateIndexState rewires for them: North is the row z = 0, South the last row,
            // East the column x = 0, West the last column.
            int last = Cells - 1;
            switch (side)
            {
                case Side.North: return PQS.ti(i, 0);
                case Side.South: return PQS.ti(i, last);
                case Side.East: return PQS.ti(0, i);
                default: return PQS.ti(last, i);
            }
        }

        /// <summary>Index, in the vertices of a quad, of the <paramref name="i"/>-th vertex along
        /// <paramref name="side"/>, from 0 to <see cref="Cells"/>.</summary>
        public static int EdgeVertex(Side side, int i)
        {
            int last = SideVertices - 1;
            switch (side)
            {
                case Side.North: return PQS.vi(i, 0);
                case Side.South: return PQS.vi(i, last);
                case Side.East: return PQS.vi(0, i);
                default: return PQS.vi(last, i);
            }
        }

        /// <summary>
        /// The triangle index list the mesh of <paramref name="quad"/> is currently drawn with.
        /// </summary>
        public static int[] Triangles(PQ quad)
        {
            // Right after a build, stock gives the mesh the list without any stitching and marks the quad
            // Reset (PQS.BuildQuad); the stitched list only comes with the next visibility update.
            int state = (int)quad.edgeState;
            return PQS.cacheIndices[state < 0 ? 0 : state];
        }
    }
}
