using System;
using System.Collections.Generic;
using UnityEngine;
using com.github.lhervier.ksp.mcpserver;

namespace com.github.lhervier.ksp.diag.quadseams
{
    /// <summary>
    /// What KSP-MCPServer, when it is installed, offers of this mod as tools: the reading of the largest gap,
    /// the buttons that choose what is drawn, and the moving of the window. Each method works on the mod
    /// loaded in the flight scene. Nothing here is needed to play by hand, and this mod runs the same without
    /// KSP-MCPServer: only that server reads the attribute.
    /// </summary>
    internal static class McpTools
    {
        [McpTool("quadseams_read",
            "Reads KSP Diag - Quad Seams: how many seams, the last line it logged and whether that line followed " +
                "a shift of the origin, and for the largest gap the latitude, longitude, height above sea level " +
                "and underSea of its vertex, gapMm, verticalMm, horizontalMm, finerAbove (whether the finer quad " +
                "stands above the coarser one), distanceKm and headingFromVessel from the active vessel.")]
        internal static object Read()
        {
            return Mod().Reading();
        }

        [McpTool("quadseams_set_display",
            "Chooses what KSP Diag - Quad Seams draws, as the buttons of its window do: everything (the seams " +
                "and the largest gap), marker (the yellow line of the largest gap only) or nothing.")]
        internal static object SetDisplay(string display)
        {
            KSPDiagQuadSeams mod = Mod();
            switch ((display ?? "").ToLowerInvariant())
            {
                case "everything":
                    mod.SetDisplay(KSPDiagQuadSeams.Display.Everything);
                    break;
                case "marker":
                    mod.SetDisplay(KSPDiagQuadSeams.Display.MarkerOnly);
                    break;
                case "nothing":
                    mod.SetDisplay(KSPDiagQuadSeams.Display.Nothing);
                    break;
                default:
                    throw new ArgumentException("display: everything, marker or nothing");
            }
            return mod.Current.ToString();
        }

        [McpTool("quadseams_move_window",
            "Moves the window of KSP Diag - Quad Seams, as dragging it does: x and y in pixels from the top left " +
            "corner of the screen. Returns its position and size (x, y, width, height).")]
        internal static object MoveWindow(double x, double y)
        {
            KSPDiagQuadSeams mod = Mod();
            Rect rect = mod.WindowRect;
            rect.x = (float)x;
            rect.y = (float)y;
            mod.WindowRect = rect;
            return new Dictionary<string, object>
            {
                { "x", (double)rect.x },
                { "y", (double)rect.y },
                { "width", (double)rect.width },
                { "height", (double)rect.height }
            };
        }

        [McpTool("quadseams_show_window",
            "Shows or hides the window of KSP Diag - Quad Seams, as Mod+F6 does; what it measures goes on either " +
            "way. Returns whether it shows (visible).")]
        internal static object ShowWindow(bool visible)
        {
            KSPDiagQuadSeams.WindowVisible = visible;
            return new Dictionary<string, object> { { "visible", KSPDiagQuadSeams.WindowVisible } };
        }

        private static KSPDiagQuadSeams Mod()
        {
            KSPDiagQuadSeams mod = UnityEngine.Object.FindObjectOfType<KSPDiagQuadSeams>();
            if (mod == null)
            {
                throw new InvalidOperationException("KSP Diag - Quad Seams only runs in flight");
            }
            return mod;
        }
    }
}
