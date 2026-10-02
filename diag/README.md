# The runs

Part of [KSP Diag - Quad Seams](../README.md): the `KSP.log` of every session of
[the protocol](../docs/the-protocol.md). What their readings say is in
[The measurements](../docs/the-measurements.md) and
[What the measurements show](../docs/what-the-measurements-show.md).

The protocol needs no save of its own: any craft on the launchpad will do.

## Case 1: Real Solar System

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1 and this mod, plus
[Real Solar System](https://github.com/KSP-RO/RealSolarSystem) 20.1.3.0 as released and what it
requires (Kopernicus, Modular Flight Integrator, KSPTextureLoader, the RSS textures), and MechJeb. A
craft on the launchpad at Cape Canaveral, reverted to launch again and again.

- [`runs/revert-earth-rss-1-readings.txt`](runs/revert-earth-rss-1-readings.txt) — the first session,
  loads 1 to 11. Its `KSP.log` was overwritten when the game was started again: this file holds the
  last line this mod wrote after each load, read from that log before it was lost.
- [`runs/revert-earth-rss-2.log`](runs/revert-earth-rss-2.log) — the second session, loads 12 to 15,
  whole. Load 15 is the one of the two screenshots with the crack along the seam.

## Case 2: stock KSP

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1 and this mod. A craft launched from
the VAB onto the launchpad at the Space Center, on Kerbin.

- [`runs/launch-kerbin-1.log`](runs/launch-kerbin-1.log) — one session: the launch, the only load,
  and the two screenshots with the crack along the seam.
