# The measurements: the quads of the highest level, in flight

Part of [KSP Diag - Quad Seams](../README.md): [the protocol of the quads in flight](the-protocol-flight.md),
as played so far, on Kerbin and on Earth in Real Solar System.

KSP 1.12.5 with Harmony, ModuleManager and KSP Community Fixes 1.41.1, this mod,
[KSP Diag - Floating Origin](https://github.com/lhervier/KSP-Diag-FloatingOrigin) and
[KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer), the terrain detail set to *High*; on Earth,
[Real Solar System](https://github.com/KSP-RO/RealSolarSystem) 20.1.3.0 as released and what it requires
as well (Kopernicus 248, Modular Flight Integrator, KSPTextureLoader, the RSS textures). On each body, one
flight of `Quad-Rocket` from the launchpad, the same craft and the same steps, played by
[the script of the protocol](the-protocol-flight.md#played-by-a-script), `run-flight.py`, and read by
`analyse-flight.py`. The logs, what the script printed, what each *Log* answered, the two files of the
*Logs* and what `analyse-flight.py` printed are in [the runs](../diag/README.md#the-quads-of-the-highest-level-in-flight).

## On Kerbin

From the launchpad of the Space Center; the highest subdivision level of Kerbin is 10.

**The flight.** 72 *Logs* over 71 seconds of game time. The first one on the launchpad, the engine lit
and the clamp still holding; the craft then rose to 957 m and reached 653 m/s, and the last *Log* was
taken 30 m above the sea, just before the craft hit it. The world moved 2,365 times in all: once as the
scene opened, none while the craft was slower than 100 m/s (the first eleven *Logs*), then every frame
from about 235 m/s on, the speed at which the *Krakensbane* column of the file stops reading 0. 356
quads of level 10 were written, most of them in several *Logs*.

For the quads of level 10, in millimetres:

| | Comparisons | Median | 90th percentile | Largest |
|---|---:|---:|---:|---:|
| the same quad, from one *Log* to the next, under 100 m/s | 1,364 | 0.000 | 0.000 | 0.000 |
| the same quad, from one *Log* to the next, over 100 m/s | 9,912 | 0.050 | 0.226 | 0.699 |
| the step between siblings, in the same *Log* | 11,021 | 0.007 | 0.033 | 0.418 |
| **the step between cousins, in the same *Log*** | 9,103 | 0.013 | **12.021** | **22.454** |
| — both written from the first *Log* on | 5,080 | 0.006 | 0.027 | 0.403 |
| — one of the two at least built during the flight | 4,023 | **4.520** | **16.930** | **22.454** |

Of the 9,103 shared edges between cousins, 2,940 have a step of more than 1 mm, 1,882 of more than 5 mm,
and 1,120 of more than 10 mm. The cousins split in two: those both written from the first *Log* on, built
as the scene opened, around the launchpad, and the others, of which one at least was built during the
flight, as the craft went.

## On Earth, in Real Solar System

From the launchpad of Cape Canaveral; the highest subdivision level of Earth is 11.

**The flight.** 55 *Logs* over 52 seconds of game time: the same steps, but a lower and shorter flight
than on Kerbin, up to 565 m and 488 m/s, the last *Log* taken 30 m above the sea. The world moved 1,392
times in all: once as the scene opened, none while the craft was slower than 100 m/s (the first eleven
*Logs*), then every frame from about 238 m/s on. 224 quads of level 11 were written.

For the quads of level 11, in millimetres:

| | Comparisons | Median | 90th percentile | Largest |
|---|---:|---:|---:|---:|
| the same quad, from one *Log* to the next, under 100 m/s | 1,800 | 0.000 | 0.000 | 0.000 |
| the same quad, from one *Log* to the next, over 100 m/s | 8,732 | 0.160 | 0.740 | 2.172 |
| the step between siblings, in the same *Log* | 9,863 | 0.137 | 0.286 | 1.376 |
| **the step between cousins, in the same *Log*** | 8,203 | 0.144 | 0.618 | **288.716** |
| — both written from the first *Log* on | 7,425 | 0.132 | 0.331 | 1.592 |
| — one of the two at least built during the flight | 778 | **57.458** | **264.204** | **288.716** |

The quads of Earth are larger than those of Kerbin, and the flight shorter: far fewer quads were built
during it, 778 shared edges against 4,023 on Kerbin, and most cousins were still the ones built as the
scene opened.

## Under 100 m/s

On both bodies, every vertex of every quad was at the same distance from the centre of the body, to the
thousandth of a millimetre, from one *Log* to the next while the craft was slower than 100 m/s; but the
world did not move once in that time, so that line says nothing of what a move does to a quad at low
speed.

**→ What it shows: [What the measurements show: the quads of the highest level, in flight](what-the-measurements-show-flight.md)**
