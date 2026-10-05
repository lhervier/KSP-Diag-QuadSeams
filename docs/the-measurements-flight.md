# The measurements: the quads of the highest level, in flight

Part of [KSP Diag - Quad Seams](../README.md): [the protocol of the quads in flight](the-protocol-flight.md),
as played so far, on Kerbin.

KSP 1.12.5 with Harmony, ModuleManager and KSP Community Fixes 1.41.1, this mod,
[KSP Diag - Floating Origin](https://github.com/lhervier/KSP-Diag-FloatingOrigin) and
[KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer); the terrain detail set to *High*, where the
highest subdivision level of Kerbin is 10. One flight of `Quad-Rocket` from the launchpad of the Space
Center, played by [the script of the protocol](the-protocol-flight.md#played-by-a-script), `run-flight.py`,
and read by `analyse-flight.py`. The session is logged in
[`diag/runs/flight-kerbin-stock.log`](../diag/runs/flight-kerbin-stock.log); what the script printed is in
[`flight-kerbin-stock-script.txt`](../diag/runs/flight-kerbin-stock-script.txt), what each *Log* answered in
[`flight-kerbin-stock-readings.json`](../diag/runs/flight-kerbin-stock-readings.json), the two files of the
*Logs* in [`flight-kerbin-stock-logs.csv`](../diag/runs/flight-kerbin-stock-logs.csv) and
[`flight-kerbin-stock-quads.zip`](../diag/runs/flight-kerbin-stock-quads.zip), and what `analyse-flight.py`
printed in [`flight-kerbin-stock-analysis.txt`](../diag/runs/flight-kerbin-stock-analysis.txt).

## The flight

72 *Logs* over 71 seconds of game time. The first one on the launchpad, the engine lit and the clamp
still holding; the craft then rose to 957 m and reached 653 m/s, and the last *Log* was taken 30 m above
the sea, just before the craft hit it. The world moved 2,365 times in all: once as the scene opened,
none while the craft was slower than 100 m/s (the first eleven *Logs*), then every frame from about
235 m/s on, the speed at which the *Krakensbane* column of the file stops reading 0. 356 quads of level
10 were written, most of them in several *Logs*.

## The figures

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

Under 100 m/s, every vertex of every quad was at the same distance from the centre of the body, to the
thousandth of a millimetre, from one *Log* to the next; but the world did not move once in that time, so
that line says nothing of what a move does to a quad at low speed.
