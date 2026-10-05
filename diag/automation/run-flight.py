"""Plays the suborbital flight of KSP Diag - Quad Seams: a rocket launched from the launchpad, one Log a second.

It drives KSP through KSP-MCPServer, a mod that answers HTTP requests on 127.0.0.1, and needs nothing but
Python 3: no AI, no package to install. Start KSP with KSP-MCPServer and KSP Diag - Quad Seams installed, wait
for the main menu, then run:

    python run-flight.py --folder <your sandbox game> --craft VAB/Quad-Rocket.craft --out out

It does what the protocol asks of a player: it launches the craft onto the launchpad, turns on Infinite Fuel
and Infinite Electricity (Alt+F12, Cheats), turns SAS on and the throttle to full, activates the first stage
(the engines light, the launch clamps still hold the craft), takes a Log, activates the second stage (the
craft lifts off), then takes one Log a second and touches nothing else until the craft is no longer flying:
destroyed, landed or splashed down. Each Log is what the Log button of the mod writes, into its two CSV files;
the script copies both into --out at the end, with readings.json, what each Log answered. It leaves KSP
running unless --quit is given.

With --screenshots, it also takes the screenshots of the protocol into --out, the window of KSP Diag -
Floating Origin, which must then be installed, at the top left of the screen, and the one of this mod at the
bottom left. The moments: the craft on the launchpad before the first stage (00-launchpad), the first
Log, the engines lit and the clamps still holding (10-first-log), the first Log after which the origin of the
world moved more than once (20-shifts), and the last Log of the flight (30-last-log).
"""
import argparse
import json
import os
import shutil
import time
import urllib.request

URL = None


def call(tool, **args):
    """Calls one tool of KSP-MCPServer and returns its answer, decoded from JSON when it is JSON."""
    body = json.dumps({"jsonrpc": "2.0", "id": 1, "method": "tools/call",
                       "params": {"name": tool, "arguments": args}}).encode()
    request = urllib.request.Request(URL, body, {"Content-Type": "application/json"})
    with urllib.request.urlopen(request, timeout=900) as response:
        result = json.loads(response.read())["result"]
    text = result["content"][0].get("text", "") if result["content"] else ""
    if result.get("isError"):
        raise RuntimeError(tool + ": " + text)
    try:
        answer = json.loads(text)
    except ValueError:
        return text
    # The tools another mod adds answer inside "returned".
    if isinstance(answer, dict) and list(answer) == ["returned"]:
        return answer["returned"]
    return answer


def log(*parts):
    print(time.strftime("%H:%M:%S"), *parts, flush=True)


def still_flying(name):
    """Whether the craft called name is still the active vessel and not back on the ground; with the state of
    the game."""
    state = call("get_state")
    vessel = state.get("vessel")
    # PRELAUNCH counts as flying: the game may keep it for a moment after the clamps let go.
    flying = vessel is not None and vessel["name"] == name and vessel["situation"] not in ("LANDED", "SPLASHED")
    return flying, state


def take_log(readings, out, state):
    """Takes a Log, notes what it answered with the state of the craft, and writes readings.json again."""
    answer = call("quadseams_log")
    vessel = state.get("vessel") or {}
    answer["altitude"] = vessel.get("altitude")
    answer["surfaceSpeed"] = vessel.get("surfaceSpeed")
    readings.append(answer)
    with open(os.path.join(out, "readings.json"), "w", newline="") as f:
        json.dump(readings, f, indent=1)
    log("Log %d: %d quads, %d origin shift(s), altitude %s m, %s m/s" % (
        answer["log"], answer["quads"], answer["originShifts"],
        "%.0f" % vessel["altitude"] if vessel else "-", "%.0f" % vessel["surfaceSpeed"] if vessel else "-"))
    return answer


def shoot(out, name, screen_height):
    """Takes a screenshot of the game as it is drawn, both windows showing, into out/<name>.png."""
    # The window of this mod grows and shrinks with the lines it shows: set against the bottom of the screen
    # at its height of the moment.
    height = call("quadseams_move_window", x=0, y=0)["height"]
    call("quadseams_move_window", x=0, y=screen_height - height)
    # Moved from the next frame on.
    call("wait", seconds=0.2)
    call("screenshot", path=os.path.join(out, name + ".png"), return_image=False)
    log("screenshot " + name)


def main():
    global URL
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--folder", required=True, help="the sandbox game to launch from")
    parser.add_argument("--craft", required=True, help="the craft, as VAB/<name>.craft")
    parser.add_argument("--period", type=float, default=1.0, help="seconds between two Logs")
    parser.add_argument("--max-seconds", type=float, default=300.0,
                        help="how long the flight may last at most, in seconds of real time")
    parser.add_argument("--out", default="out", help="where readings.json and the two CSV files go")
    parser.add_argument("--port", type=int, default=8770, help="the port of KSP-MCPServer")
    parser.add_argument("--screenshots", action="store_true", help="also take the screenshots of the protocol")
    parser.add_argument("--screen-height", type=int, default=720, help="the height of the game's screen, in pixels")
    parser.add_argument("--quit", action="store_true", help="quit KSP at the end")
    parser.add_argument("--keep-running", action="store_true",
                        help="leave KSP running at the end, which it does unless --quit is given")
    options = parser.parse_args()
    URL = "http://127.0.0.1:%d/mcp/" % options.port
    out = os.path.abspath(options.out)
    os.makedirs(out, exist_ok=True)

    call("open_game", folder=options.folder)
    call("launch_vessel", craft=options.craft, site="LaunchPad")
    name = call("get_state")["vessel"]["name"]
    call("set_cheats", infinite_fuel=True, infinite_electricity=True)
    call("set_flight", throttle=1.0, sas=True)
    if options.screenshots:
        # Diag FloatingOrigin at the top left, below the bar of the game's time; this mod at the bottom left.
        call("floatingorigin_move_window", x=0, y=40)
        call("wait", seconds=2)
        shoot(out, "00-launchpad", options.screen_height)

    # The first stage lights the engines; the launch clamps hold the craft until the second.
    call("stage")
    call("wait", seconds=1)
    readings = []
    take_log(readings, out, call("get_state"))
    if options.screenshots:
        shoot(out, "10-first-log", options.screen_height)
    call("stage")
    log("lift-off")
    shifts_shot = False

    # One Log a second, on a schedule of its own: the time a Log takes does not push the next one back.
    start = time.monotonic()
    tick = 1
    while time.monotonic() - start < options.max_seconds:
        delay = start + tick * options.period - time.monotonic()
        if delay > 0:
            time.sleep(delay)
        tick += 1
        flying, state = still_flying(name)
        if not flying:
            vessel = state.get("vessel")
            log("the flight is over: " + ("no active vessel" if vessel is None
                                          else "%s, %s" % (vessel["name"], vessel["situation"])))
            break
        answer = take_log(readings, out, state)
        if options.screenshots:
            if not shifts_shot and answer["originShifts"] > 1:
                shoot(out, "20-shifts", options.screen_height)
                shifts_shot = True
            elif shifts_shot:
                # Taken again after every Log: the one left is the last Log of the flight.
                shoot(out, "30-last-log", options.screen_height)
    else:
        log("still flying after %.0f s: stopped" % options.max_seconds)

    # The two files of the Logs, kept next to readings.json.
    for key in ("logsFile", "quadsFile"):
        shutil.copy2(readings[-1][key], out)
    log("done: %d Logs, files in %s" % (len(readings), out))
    if options.quit:
        call("quit_game")


if __name__ == "__main__":
    main()
