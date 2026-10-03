"""Plays "The protocol" of KSP Diag - Quad Seams: Revert to Launch until the seam shows, then two screenshots.

It drives KSP through KSP-MCPServer, a mod that answers HTTP requests on 127.0.0.1, and needs nothing but
Python 3: no AI, no package to install. Start KSP with KSP-MCPServer and KSP Diag - Quad Seams installed, wait
for the main menu, then run:

    python run-revert.py --folder <your sandbox game> --craft VAB/<craft>.craft --out out

It launches the craft onto the launchpad, then reverts to launch until the largest gap is one the protocol can
see: the finer quad above the coarser one, its vertex out of the sea, the step at least --min-step
millimetres. Each revert waits for the seams to be built, the line of log no longer changing. It then switches the drawing to the yellow line only, pulls the camera back beyond the foot of
the line, a little above it, looking back towards the craft, takes a screenshot, switches the drawing to everything and takes
a second one. It writes every reading to readings.json, and leaves KSP running so that the view can be
adjusted by hand (unless --quit is given).
"""
import argparse
import json
import math
import os
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


def settled_reading():
    """Waits for the seams to be built: the same line of log for four seconds in a row. The mod writes a line
    only when the seams change, so in a scene at rest the last one may well be the one after the shift of the
    origin that comes with every load."""
    last = None
    start = time.time()
    while time.time() - start < 60:
        reading = call("quadseams_read")
        if reading.get("largest"):
            if last is not None and reading["lastLog"] == last["lastLog"]:
                return reading
            last = reading
        time.sleep(4)
    return last


def visible(largest, min_step):
    return largest["finerAbove"] and not largest["underSea"] and largest["verticalMm"] >= min_step


def main():
    global URL
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--folder", required=True, help="the sandbox game to launch from")
    parser.add_argument("--craft", required=True, help="the craft, as SPH/<name>.craft or VAB/<name>.craft")
    parser.add_argument("--min-step", type=float, default=200.0, help="the smallest step worth looking at, in mm")
    parser.add_argument("--max-reverts", type=int, default=30, help="how many reverts at most")
    parser.add_argument("--beyond", type=float, default=50.0,
                        help="how far beyond the foot of the yellow line the camera stands, in metres")
    parser.add_argument("--height", type=float, default=20.0,
                        help="how high above the foot of the yellow line the camera stands, in metres")
    parser.add_argument("--out", default="out", help="where readings.json and the screenshots go")
    parser.add_argument("--port", type=int, default=8770, help="the port of KSP-MCPServer")
    parser.add_argument("--quit", action="store_true", help="quit KSP at the end")
    options = parser.parse_args()
    URL = "http://127.0.0.1:%d/mcp/" % options.port
    out = os.path.abspath(options.out)
    os.makedirs(out, exist_ok=True)

    call("open_game", folder=options.folder)
    call("launch_vessel", craft=options.craft, site="LaunchPad")
    call("quadseams_move_window", x=0, y=40)
    readings = []
    chosen = None
    for revert in range(options.max_reverts + 1):
        if revert > 0:
            call("revert_to_launch")
        reading = settled_reading()
        reading["revert"] = revert
        readings.append(reading)
        largest = reading.get("largest")
        if largest is None:
            log("revert %d: no seam measured" % revert)
            continue
        log("revert %d: step %.1f mm (%s), %.1f km away, %s" % (
            revert, largest["verticalMm"], "finer above" if largest["finerAbove"] else "finer below",
            largest["distanceKm"], "under the sea" if largest["underSea"] else "on land"))
        if visible(largest, options.min_step):
            chosen = reading
            break
    with open(os.path.join(out, "readings.json"), "w", newline="") as f:
        json.dump(readings, f, indent=1)
    if chosen is None:
        raise SystemExit("no seam worth looking at after %d reverts" % options.max_reverts)

    largest = chosen["largest"]
    call("quadseams_set_display", display="marker")
    # The camera turns round the craft and looks along its heading: standing beyond the foot of the line and
    # looking back at the craft, it looks the other way. Its pitch puts it --height metres above the foot of
    # the line, the ground dropping away with the curve of the body over that distance.
    vessel = call("get_state")["vessel"]
    radius = call("get_terrain", latitude=largest["latitude"], longitude=largest["longitude"])["radius"]
    distance = largest["distanceKm"] * 1000.0 + options.beyond
    rise = largest["altitude"] + options.height - vessel["altitude"] + distance ** 2 / (2.0 * radius)
    pitch = math.degrees(math.atan2(rise, distance))
    call("set_camera", heading=(largest["headingFromVessel"] + 180.0) % 360.0, pitch=pitch, distance=distance)
    log("camera %.0f m from the craft, pitch %.2f degrees" % (distance, pitch))
    call("wait", seconds=3)
    call("screenshot", path=os.path.join(out, "revert%d-largest-gap.png" % chosen["revert"]), return_image=False)
    call("quadseams_set_display", display="everything")
    call("wait", seconds=1)
    call("screenshot", path=os.path.join(out, "revert%d-seams.png" % chosen["revert"]), return_image=False)
    log("done: revert %d, screenshots in %s" % (chosen["revert"], out))
    if options.quit:
        call("quit_game")


if __name__ == "__main__":
    main()
