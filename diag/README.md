# The runs

Part of [KSP Diag - Quad Seams](../README.md): the `KSP.log` of every session of
[the protocol](../docs/the-protocol.md), and the script that plays it. What their readings say is in
[The measurements](../docs/the-measurements.md) and
[What the measurements show](../docs/what-the-measurements-show.md).

The protocol needs no save of its own: any craft on the launchpad will do. The one used is
`Diag3-Rocket`, from [KSP Diag - Floating Origin](https://github.com/lhervier/KSP-Diag-FloatingOrigin/tree/master/craft).

- [`automation/run-revert.py`](automation/run-revert.py) — the script that plays the protocol through
  [KSP-MCPServer](https://github.com/lhervier/KSP-MCPServer), with Python 3 alone; how to run it is at
  the top of the file, and in [Played by a script](../docs/the-protocol.md#played-by-a-script).

## Case 1: Real Solar System

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1, this mod and KSP-MCPServer, plus
[Real Solar System](https://github.com/KSP-RO/RealSolarSystem) 20.1.3.0 as released and what it
requires (Kopernicus 248, Modular Flight Integrator, KSPTextureLoader, the RSS textures). The craft on
the launchpad at Cape Canaveral, reverted to launch by
`run-revert.py --min-step 500 --ut 64800 --candidates 8 --max-reverts 80`.

- [`runs/revert-earth-rss-stock.log`](runs/revert-earth-rss-stock.log) — the session, 79 loads, the
  screenshots taken at 8 of them;
  what the script printed in [`runs/revert-earth-rss-stock-script.txt`](runs/revert-earth-rss-stock-script.txt),
  and every reading in [`runs/revert-earth-rss-stock-readings.json`](runs/revert-earth-rss-stock-readings.json).

## Case 2: stock KSP

KSP 1.12.5 with Harmony, ModuleManager, KSP Community Fixes 1.41.1, this mod and KSP-MCPServer. The craft
on the launchpad at the Space Center, on Kerbin, reverted to launch by
`run-revert.py --min-step 50 --ut 3600`.

- [`runs/revert-kerbin-stock.log`](runs/revert-kerbin-stock.log) — the session, seven loads; what the
  script printed in [`runs/revert-kerbin-stock-script.txt`](runs/revert-kerbin-stock-script.txt), and
  every reading in [`runs/revert-kerbin-stock-readings.json`](runs/revert-kerbin-stock-readings.json).
