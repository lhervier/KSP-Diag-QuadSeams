using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace com.github.lhervier.ksp.diag.quadseams
{
    /// <summary>
    /// Seam viewer. In flight, draws over the terrain of the active vessel's body, wherever a quad of the
    /// highest subdivision level lies against a coarser quad, the triangles both quads have along the
    /// side they share: green for the finer quad, red for the coarser one, filled and outlined. A yellow
    /// vertical line rises from the shared vertex where the two quads are furthest apart. The drawing
    /// shows through the terrain; the buttons of a small window choose between all of it, the yellow line
    /// only, and nothing, and Mod+F6 shows or hides that window. Each time the seams change, the log gets a
    /// line saying how far apart the two quads put the vertices they share. The Log button of the window
    /// writes to two CSV files the distance from the centre of the body to every vertex of every quad of the
    /// highest level, and of the coarser quads against them (see <see cref="VertexLog"/>).
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class KSPDiagQuadSeams : MonoBehaviour
    {
        private const string LOG_PREFIX = "[KSPDiagQuadSeams] ";

        // Unity's own shader for its debug lines: unlike the unlit shaders of the game, it lets a script
        // turn the depth test off.
        private const string SHADER_NAME = "Hidden/Internal-Colored";

        // How often the quads are looked at again, in seconds of real time. The drawing itself follows
        // the quads every frame.
        private const float REFRESH_PERIOD = 0.25f;

        // At most one line of log per this many seconds of real time, unless the origin of the world
        // has just been shifted.
        private const float LOG_PERIOD = 1f;

        // Change in the largest gap, in metres, below which a new line of log is not worth it.
        private const double LOG_GAP_CHANGE = 0.00001;

        private static readonly Color FINE_FILL = new Color(0f, 1f, 0f, 0.25f);
        private static readonly Color FINE_LINE = new Color(0f, 0.45f, 0f, 1f);
        private static readonly Color COARSE_FILL = new Color(1f, 0f, 0f, 0.25f);
        private static readonly Color COARSE_LINE = new Color(0.5f, 0f, 0f, 1f);
        private static readonly Color MARKER = new Color(1f, 0.9f, 0f, 1f);

        // Drawn after everything else, the lines after the fills so that no fill covers them, and the
        // marker last.
        private const int FILL_QUEUE = 4000;
        private const int LINE_QUEUE = 4001;
        private const int MARKER_QUEUE = 4002;

        private Material fineFill;
        private Material fineLine;
        private Material coarseFill;
        private Material coarseLine;
        private Material markerLine;

        // Mod+F6 shows or hides the window, the same key for every KSP Diag.
        private static readonly KeyBinding WINDOW_KEY = new KeyBinding(KeyCode.F6);
        private const int WINDOW_ID = 0x47485004;

        /// <summary>What is drawn. Measuring and logging go on whatever is chosen.</summary>
        internal enum Display
        {
            Everything,
            MarkerOnly,
            Nothing,
        }

        // Static, so that the choices survive a scene change or a reload.
        private static Display display = Display.Everything;
        private static bool windowVisible = true;

        /// <summary>Whether the window shows, as Mod+F6 toggles it; the measures go on either way.</summary>
        internal static bool WindowVisible
        {
            get { return windowVisible; }
            set { windowVisible = value; }
        }

        private static Rect windowRect = new Rect(60f, 60f, 520f, 0f);

        // The last line of log, shown in the window too, and whether it followed a shift of the origin.
        private string lastLine = "";
        private bool lastAfterShift;

        private CelestialBody body;
        private PQS sphere;
        private readonly WorstMarker marker = new WorstMarker();

        // The seam with the largest gap, at the last refresh; null when no vertex was matched.
        private Seam worst;
        private readonly Dictionary<PQ, EdgeOverlay> fineOverlays = new Dictionary<PQ, EdgeOverlay>();
        private readonly Dictionary<PQ, EdgeOverlay> coarseOverlays = new Dictionary<PQ, EdgeOverlay>();
        private readonly List<Seam> seams = new List<Seam>();
        private float nextRefresh;

        // Scratch buffers of a refresh.
        private readonly List<PQ> leaves = new List<PQ>();
        private readonly Dictionary<PQ, int> fineSides = new Dictionary<PQ, int>();
        private readonly Dictionary<PQ, int> coarseSides = new Dictionary<PQ, int>();

        // What the last line of log said, to write a new one only when it changes.
        private int loggedSeams = -1;
        private int loggedFine = -1;
        private int loggedCoarse = -1;
        private double loggedMaxGap = -1.0;
        private float nextLog;
        private bool originShifted;

        // Shifts of the origin since the last Log, and what the window says of that Log.
        private int shiftsSinceLog;
        private string lastVertexLog = "";

        private void Awake()
        {
            Shader shader = Shader.Find(SHADER_NAME);
            if (shader == null)
            {
                Debug.LogError(LOG_PREFIX + "shader " + SHADER_NAME + " not found: nothing will be drawn");
                enabled = false;
                return;
            }
            fineFill = MakeMaterial(shader, FINE_FILL, FILL_QUEUE);
            fineLine = MakeMaterial(shader, FINE_LINE, LINE_QUEUE);
            coarseFill = MakeMaterial(shader, COARSE_FILL, FILL_QUEUE);
            coarseLine = MakeMaterial(shader, COARSE_LINE, LINE_QUEUE);
            markerLine = MakeMaterial(shader, MARKER, MARKER_QUEUE);

            // Shader.Find answers with whatever carries the name: say what was found, and whether the depth
            // test really is a property of it, since without one the terrain hides the drawing.
            Debug.Log(LOG_PREFIX + "shader " + shader.name + ", depth test settable: "
                + fineFill.HasProperty("_ZTest"));

            // An instance method: EventData refuses a static handler.
            GameEvents.onFloatingOriginShift.Add(OnOriginShift);
        }

        private void OnDestroy()
        {
            GameEvents.onFloatingOriginShift.Remove(OnOriginShift);
            Clear();
            marker.Destroy();

            // None of them exists when the shader was not found.
            foreach (Material material in new[] { fineFill, fineLine, coarseFill, coarseLine, markerLine })
            {
                if (material != null)
                {
                    Destroy(material);
                }
            }
        }

        /// <summary>A material of <paramref name="color"/> that ignores the depth of what is already
        /// drawn, drawn at <paramref name="queue"/>.</summary>
        private static Material MakeMaterial(Shader shader, Color color, int queue)
        {
            Material material = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
            material.SetColor("_Color", color);
            material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            material.SetInt("_Cull", (int)CullMode.Off);
            material.SetInt("_ZWrite", 0);
            material.SetInt("_ZTest", (int)CompareFunction.Always);
            material.renderQueue = queue;
            return material;
        }

        private void OnOriginShift(Vector3d offset, Vector3d nonFrame)
        {
            // The quads have just been moved: measure them again at once, and say so whatever it shows.
            originShifted = true;
            nextRefresh = 0f;
            shiftsSinceLog++;
        }

        private void Update()
        {
            if (GameSettings.MODIFIER_KEY.GetKey() && WINDOW_KEY.GetKeyDown())
            {
                windowVisible = !windowVisible;
            }

            if (Time.unscaledTime < nextRefresh)
            {
                return;
            }
            nextRefresh = Time.unscaledTime + REFRESH_PERIOD;
            Refresh();
        }

        private void OnGUI()
        {
            if (!windowVisible)
            {
                return;
            }
            GUI.skin = HighLogic.Skin;
            windowRect = GUILayout.Window(WINDOW_ID, windowRect, DrawWindow, "KSP Diag - Quad Seams");
        }

        private void DrawWindow(int id)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            foreach (Display choice in new[] { Display.Everything, Display.MarkerOnly, Display.Nothing })
            {
                // The choice in force is the one that cannot be pressed.
                GUI.enabled = choice != display;
                if (GUILayout.Button(Describe(choice)))
                {
                    SetDisplay(choice);
                }
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            GUILayout.Label(lastLine);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Log", GUILayout.ExpandWidth(false)))
            {
                try
                {
                    LogVertices();
                }
                catch (InvalidOperationException e)
                {
                    lastVertexLog = "no Log: " + e.Message;
                }
            }
            GUILayout.Label(lastVertexLog);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        private static string Describe(Display choice)
        {
            return choice == Display.Everything ? "seams and largest gap"
                : choice == Display.MarkerOnly ? "largest gap only"
                : "nothing";
        }

        /// <summary>Chooses what is drawn, as the buttons of the window do.</summary>
        internal void SetDisplay(Display choice)
        {
            display = choice;
            Debug.Log(LOG_PREFIX + "display: " + Describe(choice));
        }

        /// <summary>What is drawn.</summary>
        internal Display Current
        {
            get { return display; }
        }

        /// <summary>Where the window is on the screen, and how big.</summary>
        internal Rect WindowRect
        {
            get { return windowRect; }
            set { windowRect = value; }
        }

        /// <summary>
        /// Writes a Log of the quads as they are now, as the Log button does: every quad of the highest
        /// level, and every coarser quad against one of them. Returns the number of the Log, how many quads
        /// it holds and the two files it went to, ready to be written as JSON; throws when the active
        /// vessel's body has no terrain.
        /// </summary>
        internal Dictionary<string, object> LogVertices()
        {
            // Measured again first, so that the Log holds the quads of this very frame.
            Refresh();
            if (sphere == null || sphere.quads == null)
            {
                throw new InvalidOperationException("the body of the active vessel has no terrain");
            }

            // The leaves of the highest level whether they show or not, then the coarse side of each seam;
            // a set keeps a coarse quad met by several seams to one line.
            HashSet<PQ> quads = new HashSet<PQ>();
            foreach (PQ leaf in leaves)
            {
                if (leaf.subdivision == sphere.maxLevel)
                {
                    quads.Add(leaf);
                }
            }
            foreach (Seam seam in seams)
            {
                quads.Add(seam.Coarse);
            }

            int shifts = shiftsSinceLog;
            int log = VertexLog.Write(body, quads, shifts);
            shiftsSinceLog = 0;
            lastVertexLog = "Log " + log + ": " + quads.Count + " quad(s), " + shifts + " origin shift(s) since the previous one";
            Debug.Log(LOG_PREFIX + lastVertexLog + ", written to " + VertexLog.LogsPath + " and " + VertexLog.QuadsPath);
            return new Dictionary<string, object>
            {
                { "log", log },
                { "quads", quads.Count },
                { "originShifts", shifts },
                { "logsFile", VertexLog.LogsPath },
                { "quadsFile", VertexLog.QuadsPath }
            };
        }

        /// <summary>
        /// The seams as last measured, ready to be written as JSON: their count, the last line of log and
        /// whether it followed a shift of the origin, and for the largest gap where its vertex lies
        /// (latitude, longitude, height above sea level, whether the sea covers it), which quad stands
        /// above the other and by how much, and where it lies from the active vessel (distance, heading).
        /// </summary>
        internal Dictionary<string, object> Reading()
        {
            Dictionary<string, object> reading = new Dictionary<string, object>
            {
                { "seams", seams.Count },
                { "lastLog", lastLine },
                { "lastLogAfterOriginShift", lastAfterShift }
            };
            if (worst == null || body == null)
            {
                return reading;
            }
            Vector3d position = worst.WorstPosition;
            Vector3d up = (position - body.position).normalized;
            double vertical = Vector3d.Dot(worst.WorstGap, up);
            double altitude = body.GetAltitude(position);
            double latitude = body.GetLatitude(position);
            double longitude = body.GetLongitude(position);
            Dictionary<string, object> largest = new Dictionary<string, object>
            {
                { "latitude", latitude },
                { "longitude", longitude },
                { "altitude", altitude },
                { "underSea", body.ocean && altitude < 0.0 },
                { "gapMm", worst.MaxGap * 1000.0 },
                { "verticalMm", Math.Abs(vertical) * 1000.0 },
                { "horizontalMm", (worst.WorstGap - up * vertical).magnitude * 1000.0 },
                { "finerAbove", vertical >= 0.0 }
            };
            Vessel active = FlightGlobals.ActiveVessel;
            if (active != null)
            {
                largest["distanceKm"] = (position - (Vector3d)active.transform.position).magnitude / 1000.0;
                largest["headingFromVessel"] = Bearing(active.latitude, active.longitude, latitude, longitude);
            }
            reading["largest"] = largest;
            return reading;
        }

        /// <summary>The initial bearing, in degrees from north towards east, of the great circle from one
        /// point to another.</summary>
        private static double Bearing(double lat1, double lon1, double lat2, double lon2)
        {
            double f1 = lat1 * Math.PI / 180.0;
            double f2 = lat2 * Math.PI / 180.0;
            double dl = (lon2 - lon1) * Math.PI / 180.0;
            double y = Math.Sin(dl) * Math.Cos(f2);
            double x = Math.Cos(f1) * Math.Sin(f2) - Math.Sin(f1) * Math.Cos(f2) * Math.Cos(dl);
            return (Math.Atan2(y, x) * 180.0 / Math.PI + 360.0) % 360.0;
        }

        private void LateUpdate()
        {
            if (display == Display.Everything)
            {
                foreach (EdgeOverlay overlay in fineOverlays.Values)
                {
                    overlay.Draw(fineFill, fineLine);
                }
                foreach (EdgeOverlay overlay in coarseOverlays.Values)
                {
                    overlay.Draw(coarseFill, coarseLine);
                }
            }
            if (display != Display.Nothing)
            {
                marker.Draw(markerLine, body);
            }
        }

        /// <summary>
        /// Finds the seams of the active vessel's body as they are now, brings the drawing in line with
        /// them, measures them, and logs the result when it has changed.
        /// </summary>
        private void Refresh()
        {
            body = FlightGlobals.currentMainBody;
            PQS current = body != null ? body.pqsController : null;
            if (current != sphere)
            {
                Clear();
                sphere = current;
                if (sphere != null)
                {
                    Debug.Log(LOG_PREFIX + "body " + body.bodyName + ", radius " + sphere.radius.ToString("F0")
                        + " m, highest subdivision level " + sphere.maxLevel + ", " + QuadGrid.SideVertices
                        + " vertices by side");
                }
            }
            if (sphere == null || sphere.quads == null)
            {
                return;
            }

            FindSeams();
            UpdateOverlays(fineOverlays, fineSides);
            UpdateOverlays(coarseOverlays, coarseSides);
            worst = null;
            foreach (Seam seam in seams)
            {
                seam.Measure();
                if (seam.WorstVertex >= 0 && (worst == null || seam.MaxGap > worst.MaxGap))
                {
                    worst = seam;
                }
            }
            marker.PointAt(worst?.Fine, worst != null ? worst.WorstVertex : 0);
            LogIfChanged();
        }

        /// <summary>
        /// Fills <see cref="seams"/>, <see cref="fineSides"/> and <see cref="coarseSides"/> with the
        /// seams of <see cref="sphere"/> as they are now.
        /// </summary>
        private void FindSeams()
        {
            seams.Clear();
            fineSides.Clear();
            coarseSides.Clear();

            leaves.Clear();
            foreach (PQ root in sphere.quads)
            {
                CollectLeaves(root);
            }

            int level = sphere.maxLevel;
            foreach (PQ fine in leaves)
            {
                if (fine.subdivision != level || !fine.isVisible)
                {
                    continue;
                }
                foreach (Side side in QuadGrid.SIDES)
                {
                    // The same test as PQ.GetEdgeState, the one that makes stock stitch this side.
                    PQ coarse = QuadGrid.Neighbour(fine, side);
                    if (coarse == null || coarse.subdivision >= fine.subdivision)
                    {
                        continue;
                    }
                    Side? facing = QuadGrid.FacingSide(coarse, fine);
                    if (facing == null || coarse.isSubdivided || !coarse.isVisible)
                    {
                        continue;
                    }
                    seams.Add(new Seam(fine, side, coarse, facing.Value));
                    AddSide(fineSides, fine, side);
                    AddSide(coarseSides, coarse, facing.Value);
                }
            }
        }

        /// <summary>Adds to <see cref="leaves"/> the quads under <paramref name="quad"/> that are not
        /// split into smaller ones, <paramref name="quad"/> included.</summary>
        private void CollectLeaves(PQ quad)
        {
            if (quad == null)
            {
                return;
            }
            if (!quad.isSubdivided || quad.subNodes == null)
            {
                leaves.Add(quad);
                return;
            }
            foreach (PQ child in quad.subNodes)
            {
                CollectLeaves(child);
            }
        }

        private static void AddSide(Dictionary<PQ, int> sides, PQ quad, Side side)
        {
            sides.TryGetValue(quad, out int mask);
            sides[quad] = mask | (int)side;
        }

        /// <summary>
        /// Makes <paramref name="overlays"/> hold one overlay per quad of <paramref name="sides"/>,
        /// drawing the sides it maps that quad to, and nothing else.
        /// </summary>
        private static void UpdateOverlays(Dictionary<PQ, EdgeOverlay> overlays, Dictionary<PQ, int> sides)
        {
            List<PQ> gone = null;
            foreach (KeyValuePair<PQ, EdgeOverlay> entry in overlays)
            {
                if (entry.Key == null || !sides.ContainsKey(entry.Key))
                {
                    (gone ?? (gone = new List<PQ>())).Add(entry.Key);
                }
            }
            if (gone != null)
            {
                foreach (PQ quad in gone)
                {
                    overlays[quad].Destroy();
                    overlays.Remove(quad);
                }
            }

            foreach (KeyValuePair<PQ, int> entry in sides)
            {
                if (!overlays.TryGetValue(entry.Key, out EdgeOverlay overlay))
                {
                    overlay = new EdgeOverlay(entry.Key);
                    overlays.Add(entry.Key, overlay);
                }
                overlay.Refresh(entry.Value);
            }
        }

        /// <summary>Drops every overlay and seam.</summary>
        private void Clear()
        {
            foreach (EdgeOverlay overlay in fineOverlays.Values)
            {
                overlay.Destroy();
            }
            foreach (EdgeOverlay overlay in coarseOverlays.Values)
            {
                overlay.Destroy();
            }
            fineOverlays.Clear();
            coarseOverlays.Clear();
            seams.Clear();
            worst = null;
            marker.PointAt(null, 0);
            loggedSeams = -1;
        }

        /// <summary>
        /// Writes one line of log about the seams, when they have changed since the last one, or when the
        /// origin of the world has just been shifted.
        /// </summary>
        private void LogIfChanged()
        {
            int matched = 0;
            int unmatched = 0;
            double sum = 0.0;
            foreach (Seam seam in seams)
            {
                matched += seam.Matched;
                unmatched += seam.Unmatched;
                sum += seam.SumGap;
            }
            double maxGap = worst != null ? worst.MaxGap : 0.0;

            bool changed = seams.Count != loggedSeams
                || fineOverlays.Count != loggedFine
                || coarseOverlays.Count != loggedCoarse
                || System.Math.Abs(maxGap - loggedMaxGap) > LOG_GAP_CHANGE;
            if (!originShifted && (!changed || Time.unscaledTime < nextLog))
            {
                return;
            }

            string line = (originShifted ? "after an origin shift: " : "")
                + seams.Count + " seam(s) between " + fineOverlays.Count + " quad(s) of level " + sphere.maxLevel
                + " and " + coarseOverlays.Count + " coarser one(s); " + matched + " shared vertices";
            if (matched > 0)
            {
                line += ", gap mean " + FormatMm(sum / matched) + ", max " + FormatMm(maxGap);
            }
            if (unmatched > 0)
            {
                line += "; " + unmatched + " vertices with no counterpart";
            }
            if (worst != null)
            {
                line += "; largest on " + worst.Fine.name + " (" + worst.FineSide + ", level "
                    + worst.Fine.subdivision + ") against " + worst.Coarse.name + " (" + worst.CoarseSide
                    + ", level " + worst.Coarse.subdivision + ")";

                // Split along the vertical of the body at that vertex: a vertical gap is a step in the
                // ground, a horizontal one a crack or an overlap.
                Vector3d up = (worst.WorstPosition - body.position).normalized;
                double vertical = Vector3d.Dot(worst.WorstGap, up);
                double horizontal = (worst.WorstGap - up * vertical).magnitude;
                line += ", where the finer quad is " + FormatMm(System.Math.Abs(vertical))
                    + (vertical >= 0.0 ? " above" : " below") + " the coarser one and " + FormatMm(horizontal)
                    + " beside it";

                Vessel active = FlightGlobals.ActiveVessel;
                if (active != null)
                {
                    double distance = (worst.WorstPosition - (Vector3d)active.transform.position).magnitude;
                    line += ", " + (distance / 1000.0).ToString("F1") + " km from the active vessel";
                }
            }
            Debug.Log(LOG_PREFIX + line);
            lastLine = line;
            lastAfterShift = originShifted;

            loggedSeams = seams.Count;
            loggedFine = fineOverlays.Count;
            loggedCoarse = coarseOverlays.Count;
            loggedMaxGap = maxGap;
            nextLog = Time.unscaledTime + LOG_PERIOD;
            originShifted = false;
        }

        private static string FormatMm(double metres)
        {
            return (metres * 1000.0).ToString("F3") + " mm";
        }
    }
}
