# KSP Diag - Terrain Quads

**⚠️ Work in progress.** This is an active investigation, not a finished mod. The code and this page can still change, and several questions are still open.

A viewing and measuring instrument for KSP 1.12, for the quads the terrain is built of. In flight, it
draws over the terrain the seam where the quads of the highest subdivision level meet coarser quads:
the triangles each of the two quads has along the edge they share, green for the finer quad, red for
the coarser one, and a yellow line where the two are furthest apart. The drawing shows through the
hills, so the whole seam can be followed from far away. It also writes to the log how far apart the
two quads put the vertices they share, and, at the press of a button, where every vertex of every quad
around the craft is, so that a quad can be followed from one press to the next and compared with its
neighbours.

What goes wrong along that seam, and between quads built at different moments of a flight, is
explained by [Terrain Precision Fix](https://github.com/lhervier/KSP-TerrainPrecisionFix), in
[The seam between subdivision levels](https://github.com/lhervier/KSP-TerrainPrecisionFix/blob/master/docs/non-regression/the-seam-between-subdivision-levels.md)
and [Checking the culprit: in flight](https://github.com/lhervier/KSP-TerrainPrecisionFix/blob/master/docs/checking-the-culprit-flight.md).
This mod only shows it.

**How this was made.** Written with Claude, Anthropic's AI assistant, and reviewed line by line by a
human — me. I am saying so up front, because contributions made with an AI deserve a closer look than
others, and because some people would rather stop reading here. That look is easy to give here: this
mod changes nothing in the game, it only reads the vertices and triangles the game already holds for
each quad, and the source is a few hundred lines with no dependency of any kind.

## What it shows

Around the active craft, the terrain is made of quads of the highest level; further away, of coarser
ones. Where the two meet, the finer quad leaves out every other vertex of its edge, and the ones it
keeps should land on the vertices of the coarser edge. The mod draws the triangles of both quads along
that seam, marks with a yellow line the shared vertex where they are furthest apart, and logs the gaps:
their mean, the largest one, and how much of it is a step up or down and how much a gap beside. The
buttons of a small window switch between the whole drawing, the yellow line alone, and nothing; its *Log*
button writes, to two files, the distance from the centre of the body to every vertex of every quad
around the craft.

![The drawing seen from 10 km above a landed capsule: a green band inside, a red band outside, all around the zone of the highest level](imgs/drawing-kerbin.png)

*Stock KSP 1.12.5 with KSP Community Fixes: a capsule landed a few kilometres from the Space Center,
the camera pulled back 10 km from it.*

**→ Full chapter: [What it shows](docs/what-it-shows.md)**

## The situations

Each situation below comes with its protocol, its measurements and what they show. Every protocol is
played by a script through [KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer), on Real Solar
System and on stock KSP.

## The seam between subdivision levels

A craft on the launchpad, reverted to launch until the log says the finer quad is above the coarser one
at the largest gap; then two screenshots from just past the yellow line, one with the line only, one
with the triangles. The largest gap of a load went from 0.76 m to 2.82 m on Earth, over 79 loads, and
from 118 mm to 312 mm on Kerbin, over seven: the seam is open, and shows as a crack on both, though it
takes looking for.

**→ [The protocol](docs/the-protocol-seam.md) · [The measurements](docs/the-measurements-seam.md) · [What they show](docs/what-the-measurements-show-seam.md)**

## The quads of the highest level, in flight

A rocket launched from the launchpad and left to fly until it falls into the sea, with a *Log* about
once a second, so that each quad of the highest level can be compared with its neighbours. Two
neighbouring quads built at different moments step by up to 22.5 mm on Kerbin and 289 mm on Earth where
they meet; two built together, by 0.42 mm and 1.6 mm at most.

**→ [The protocol](docs/the-protocol-flight.md) · [The measurements](docs/the-measurements-flight.md) · [What they show](docs/what-the-measurements-show-flight.md)**

## Get it

Either way you end up with the same `GameData/KSPDiagTerrainQuads/` folder.

**Download it** — from the assets of the
[latest release](https://github.com/lhervier/KSP-Diag-TerrainQuads/releases/latest).

**Or compile it** — clone this repository, set `KSPDIR` to your KSP install folder and run
`build.bat`. It needs the .NET SDK, takes a few seconds, reads the KSP assemblies straight from your
install, and puts the DLL in `GameData/KSPDiagTerrainQuads/` inside the repository. It does
not install anything. Worth doing if you would rather not run a binary you have no source for while
reporting a measurement.

## Install

Drop `GameData/KSPDiagTerrainQuads` into the `GameData` of KSP, so that you end up with
`GameData/KSPDiagTerrainQuads/KSPDiagTerrainQuads.dll`. It runs on a stock install:
no Harmony, no ModuleManager, no dependency of any kind.

The drawing is there in every flight, the window's *nothing* hides it, and `Alt+F6` hides the window. The mod reads the terrain
and writes nothing but its lines in `KSP.log`, and the two files of its *Log* button in its own
`PluginData` folder; your saves are never touched. Removing the folder removes
the mod.

## License

MIT
