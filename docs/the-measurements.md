# The measurements

Part of [KSP Diag - Quad Seams](../README.md): the two cases of [the protocol](the-protocol.md),
as played so far.

Both cases were played by [the script of the protocol](the-protocol.md#played-by-a-script),
`run-revert.py`, through [KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer), installed next to
this mod. The craft is `Diag3-Rocket`, a small rocket that comes with
[KSP Diag - Floating Origin](https://github.com/lhervier/KSP-Diag-FloatingOrigin/tree/master/craft).
Each table gives the last line of the log after each load, the first load being the launch itself. A
load is worth looking at when the finer quad is above the coarser one at the largest gap, on land: the
script took its two screenshots there, at the size of the window, 1280 × 720, the game's interface
hidden. On Kerbin it stopped at the first such load. On Earth the crack does not show at every such load,
so it went on to the eighth (`--candidates 8`), and the pair shown is the one where the crack shows best.
The logs, what the script printed and every reading are in [the runs](../diag/README.md).

## Case 1: Real Solar System

Real Solar System 20.1.3.0 as released, with what it requires (Kopernicus 248, Modular Flight
Integrator, KSPTextureLoader, the RSS textures), on KSP 1.12.5 with Harmony, ModuleManager and KSP
Community Fixes 1.41.1; the craft on the launchpad at Cape Canaveral, launched at UT 64,800 s for
daylight, then reverted to launch 78 times: **79 loads**. On Earth the highest level is 11, and every
load ended with 60 seams between 49 quads of level 11 and 23 quads of level 10, 480 shared vertices.

Over the 79 loads, the largest gap of a load went from 0.76 m to 2.82 m, and the mean over all shared
vertices from 0.28 m to 2.08 m. At the largest gap, the finer quad was above the coarser one at 24 loads,
and on land at 8 of them, each time by more than 0.5 m. The two screenshots were taken at each of those
8 loads. In metres:

| Load | Gap, mean | Gap, max | Finer quad at the largest gap | Beside | From the craft | Largest gap |
|---:|---:|---:|---|---:|---:|---|
| 11 | 0.41 | 1.18 | 1.04 above | 0.56 | 39.7 km | on land |
| 12 | 0.65 | 1.46 | 1.26 above | 0.74 | 35.9 km | on land |
| 16 | 0.72 | 1.58 | 1.39 above | 0.75 | 38.5 km | on land |
| 19 | 0.62 | 1.42 | 1.24 above | 0.68 | 42.8 km | on land |
| 46 | 0.46 | 1.03 | 0.85 above | 0.60 | 36.0 km | on land |
| 53 | 0.60 | 1.49 | 1.30 above | 0.71 | 39.2 km | on land |
| 67 | 0.35 | 1.03 | 0.84 above | 0.59 | 36.0 km | on land |
| 79 | 0.31 | 0.89 | 0.78 above | 0.43 | 42.6 km | on land |

**79 loads, 8 worth looking at, one clear crack.** At load 12, a dark line runs through the foot of the
yellow line and along the seam; at loads 11, 16 and 19 it shows as a dashed line, at loads 46, 53 and 67
as a dotted one, and at load 79 it can barely be seen. The step is not what decides: load 16 had the
largest one, 1.39 m. An earlier session of 17 loads, which stopped at the first load worth looking at,
showed no crack that could be seen on its screenshot.

<details>
<summary>The 79 loads</summary>

| Load | Gap, mean | Gap, max | Finer quad at the largest gap | Beside | From the craft | Largest gap |
|---:|---:|---:|---|---:|---:|---|
| 1 | 0.50 | 1.28 | 1.12 above | 0.61 | 36.7 km | under the sea |
| 2 | 0.52 | 1.46 | 1.27 above | 0.71 | 38.8 km | under the sea |
| 3 | 0.57 | 1.34 | 1.17 below | 0.65 | 40.3 km | under the sea |
| 4 | 0.37 | 1.00 | 0.87 above | 0.49 | 37.7 km | under the sea |
| 5 | 0.39 | 1.07 | 0.93 below | 0.52 | 40.3 km | on land |
| 6 | 0.32 | 1.03 | 0.82 below | 0.63 | 35.5 km | on land |
| 7 | 1.03 | 1.83 | 1.61 below | 0.87 | 38.4 km | on land |
| 8 | 1.09 | 1.74 | 1.53 below | 0.83 | 40.2 km | on land |
| 9 | 1.91 | 2.66 | 2.33 below | 1.29 | 38.8 km | under the sea |
| 10 | 0.32 | 0.97 | 0.84 above | 0.47 | 33.1 km | under the sea |
| 11 | 0.41 | 1.18 | 1.04 above | 0.56 | 39.7 km | on land |
| 12 | 0.65 | 1.46 | 1.26 above | 0.74 | 35.9 km | on land |
| 13 | 1.24 | 2.00 | 1.75 below | 0.97 | 39.5 km | under the sea |
| 14 | 0.36 | 1.12 | 0.98 below | 0.54 | 37.9 km | on land |
| 15 | 0.41 | 1.08 | 0.91 below | 0.59 | 38.4 km | on land |
| 16 | 0.72 | 1.58 | 1.39 above | 0.75 | 38.5 km | on land |
| 17 | 0.41 | 1.27 | 1.05 below | 0.72 | 43.0 km | on land |
| 18 | 0.37 | 1.07 | 0.88 above | 0.62 | 41.5 km | under the sea |
| 19 | 0.62 | 1.42 | 1.24 above | 0.68 | 42.8 km | on land |
| 20 | 0.32 | 0.99 | 0.83 above | 0.54 | 41.5 km | under the sea |
| 21 | 0.30 | 0.86 | 0.75 below | 0.42 | 43.0 km | under the sea |
| 22 | 0.32 | 1.23 | 1.07 above | 0.60 | 40.0 km | under the sea |
| 23 | 0.33 | 1.02 | 0.87 below | 0.54 | 40.2 km | on land |
| 24 | 0.33 | 0.98 | 0.78 below | 0.60 | 38.8 km | on land |
| 25 | 0.42 | 1.26 | 1.06 below | 0.68 | 39.2 km | under the sea |
| 26 | 1.26 | 2.07 | 1.82 below | 0.99 | 43.3 km | on land |
| 27 | 0.35 | 1.04 | 0.87 above | 0.56 | 38.8 km | under the sea |
| 28 | 0.41 | 1.07 | 0.88 above | 0.61 | 35.3 km | under the sea |
| 29 | 0.54 | 1.30 | 1.13 below | 0.66 | 35.8 km | under the sea |
| 30 | 0.38 | 1.14 | 0.93 above | 0.67 | 37.4 km | under the sea |
| 31 | 1.11 | 1.90 | 1.67 below | 0.91 | 40.1 km | on land |
| 32 | 0.46 | 1.11 | 0.97 below | 0.53 | 40.9 km | on land |
| 33 | 1.63 | 2.48 | 2.17 below | 1.19 | 37.2 km | on land |
| 34 | 0.70 | 1.53 | 1.29 below | 0.81 | 37.2 km | under the sea |
| 35 | 0.57 | 1.38 | 1.19 below | 0.69 | 35.1 km | under the sea |
| 36 | 1.22 | 1.88 | 1.66 below | 0.89 | 35.9 km | under the sea |
| 37 | 1.12 | 1.93 | 1.65 below | 1.00 | 35.3 km | under the sea |
| 38 | 1.72 | 2.68 | 2.36 below | 1.28 | 40.3 km | under the sea |
| 39 | 0.34 | 0.96 | 0.81 below | 0.51 | 39.7 km | under the sea |
| 40 | 0.69 | 1.40 | 1.23 below | 0.68 | 38.1 km | under the sea |
| 41 | 0.40 | 1.21 | 1.04 above | 0.62 | 32.7 km | under the sea |
| 42 | 0.34 | 0.91 | 0.74 below | 0.53 | 34.3 km | under the sea |
| 43 | 0.52 | 1.35 | 1.18 below | 0.64 | 34.8 km | under the sea |
| 44 | 0.38 | 1.07 | 0.93 below | 0.52 | 38.0 km | under the sea |
| 45 | 0.72 | 1.64 | 1.44 below | 0.80 | 39.7 km | under the sea |
| 46 | 0.46 | 1.03 | 0.85 above | 0.60 | 36.0 km | on land |
| 47 | 0.28 | 0.81 | 0.62 below | 0.52 | 41.5 km | on land |
| 48 | 0.54 | 1.25 | 1.01 below | 0.73 | 38.8 km | on land |
| 49 | 0.49 | 1.14 | 1.00 below | 0.54 | 39.7 km | on land |
| 50 | 0.41 | 1.21 | 1.03 below | 0.63 | 38.8 km | under the sea |
| 51 | 0.57 | 1.32 | 1.11 above | 0.72 | 35.2 km | under the sea |
| 52 | 0.34 | 0.92 | 0.71 above | 0.58 | 33.2 km | under the sea |
| 53 | 0.60 | 1.49 | 1.30 above | 0.71 | 39.2 km | on land |
| 54 | 1.50 | 2.12 | 1.83 below | 1.06 | 35.7 km | under the sea |
| 55 | 0.74 | 1.60 | 1.34 below | 0.86 | 38.8 km | on land |
| 56 | 1.52 | 2.25 | 1.95 below | 1.13 | 34.6 km | under the sea |
| 57 | 0.87 | 1.67 | 1.46 below | 0.82 | 37.2 km | under the sea |
| 58 | 1.78 | 2.53 | 2.22 below | 1.20 | 35.5 km | on land |
| 59 | 0.52 | 1.29 | 1.07 below | 0.72 | 38.7 km | on land |
| 60 | 0.77 | 1.43 | 1.25 below | 0.69 | 40.1 km | on land |
| 61 | 0.82 | 1.58 | 1.37 above | 0.79 | 33.8 km | under the sea |
| 62 | 0.64 | 1.38 | 1.21 below | 0.66 | 36.7 km | under the sea |
| 63 | 2.08 | 2.82 | 2.47 below | 1.37 | 38.6 km | under the sea |
| 64 | 0.33 | 0.88 | 0.72 above | 0.51 | 37.5 km | under the sea |
| 65 | 0.29 | 0.76 | 0.66 below | 0.37 | 37.6 km | under the sea |
| 66 | 0.53 | 1.37 | 1.14 below | 0.76 | 37.3 km | under the sea |
| 67 | 0.35 | 1.03 | 0.84 above | 0.59 | 36.0 km | on land |
| 68 | 0.57 | 1.21 | 1.05 below | 0.61 | 37.7 km | under the sea |
| 69 | 0.36 | 1.01 | 0.86 below | 0.52 | 39.0 km | under the sea |
| 70 | 1.66 | 2.57 | 2.26 below | 1.22 | 40.9 km | under the sea |
| 71 | 0.58 | 1.43 | 1.23 below | 0.74 | 38.9 km | under the sea |
| 72 | 0.42 | 1.23 | 1.08 below | 0.59 | 34.5 km | on land |
| 73 | 0.98 | 1.68 | 1.47 below | 0.81 | 37.7 km | under the sea |
| 74 | 0.80 | 1.52 | 1.25 below | 0.85 | 32.5 km | on land |
| 75 | 0.28 | 0.90 | 0.75 below | 0.50 | 41.2 km | under the sea |
| 76 | 0.29 | 0.98 | 0.74 below | 0.63 | 39.2 km | on land |
| 77 | 0.36 | 1.14 | 0.99 above | 0.56 | 35.4 km | under the sea |
| 78 | 0.35 | 1.02 | 0.89 below | 0.49 | 42.7 km | under the sea |
| 79 | 0.31 | 0.89 | 0.78 above | 0.43 | 42.6 km | on land |

</details>

**Load 12, the finer quad 1.26 m above**, the camera 37.0 km from the craft, two screenshots from the
same camera: the yellow line only, then everything.

![Load 12: grassland under a blue sky, the yellow line standing in the middle, a thin dark line running from its foot down to the right](../imgs/earth-stock-largest-gap.png)

![The same place, the same camera, with the triangles: a red area on the left, a green band on the right, meeting along the dark line](../imgs/earth-stock-seams.png)

**→ What it shows: [The seam is open](what-the-measurements-show.md#the-seam-is-open), and
[Where to look from](what-the-measurements-show.md#where-to-look-from)**

## Case 2: stock KSP

KSP 1.12.5 with Harmony, ModuleManager and KSP Community Fixes 1.41.1; the craft on the launchpad at
the Space Center, on Kerbin, launched at UT 3,600 s for daylight, reverted to launch until a step of at
least 50 mm. On Kerbin the highest level is 10, and every load ended with 56 seams between 44 quads of
level 10 and 20 quads of level 9, 448 shared vertices.

In millimetres:

| Load | Gap, mean | Gap, max | Finer quad at the largest gap | Beside | From the craft | Largest gap |
|---:|---:|---:|---|---:|---:|---|
| 1 | 45.5 | 117.7 | 115.6 below | 22.3 | 6.7 km | under the sea |
| 2 | 62.6 | 141.2 | 134.6 below | 42.7 | 6.7 km | under the sea |
| 3 | 151.3 | 237.0 | 237.0 below | 1.3 | 8.1 km | on land |
| 4 | 224.5 | 312.1 | 310.2 below | 33.9 | 7.1 km | under the sea |
| 5 | 71.1 | 156.5 | 156.0 below | 11.4 | 6.6 km | on land |
| 6 | 67.9 | 168.6 | 167.2 below | 21.1 | 6.3 km | on land |
| 7 | 64.2 | 140.0 | 131.2 above | 48.9 | 8.1 km | on land |

**Load 7, the finer quad 131 mm above**, the camera 8.4 km from the craft, two screenshots from the same
camera: the yellow line only, then everything.

![Load 7: green hills, the yellow line standing in the middle](../imgs/kerbin-stock-largest-gap.png)

![The same place, the same camera, with the triangles: a green band on the left, a red one on the right, meeting at the foot of the line](../imgs/kerbin-stock-seams.png)

**→ What it shows: [The seam is open](what-the-measurements-show.md#the-seam-is-open), and
[Where to look from](what-the-measurements-show.md#where-to-look-from)**
