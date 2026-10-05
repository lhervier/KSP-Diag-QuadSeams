"""Reads the two CSV files of the Logs of KSP Diag - Terrain Quads and prints what moved, and by how much.

    python analyse-flight.py <logs-....csv> <quads-....csv> [--json summary.json]

Two measures, for the quads of the highest subdivision level, in millimetres, as median, 90th percentile and
largest:

- how much the distance from a vertex to the centre of the body changes for one quad, from one Log to the
  next one it appears in (a quad keeps its name for as long as it lives, and takes it again when it is built
  again at the same place); split by the speed of the craft at the second Log, under or over 100 m/s;
- the step between two quads of the highest level that share an edge, in the same Log: siblings (split from
  the same parent, so built together) and cousins (any other pair), the cousins split again into the pairs
  both written from the first Log on (built as the scene opened) and the others (one of the two at least
  built during the flight).

The CSV files give no neighbours, only names and distances: two edges are taken as one shared edge when the
distances along them rise and fall the same way to within --profile millimetres between each pair of
vertices, and are apart by the same amount all along, to within 1 mm. Edges along which the ground rises and
falls by less than --relief metres in all are left out, flat ground matching any other flat ground.
"""
import argparse
import csv
import json
import statistics
from collections import defaultdict

SIDE = 15                  # vertices along a side of a quad
SLOW = 100.0               # m/s: under it, stock places the quads of the highest level again at each shift


def read_logs(path):
    """Log number -> surface speed of the craft, in m/s (None when there was no vessel)."""
    speeds = {}
    with open(path, newline="") as f:
        for row in csv.DictReader(f, delimiter=";"):
            speed = row["Surface speed (m/s)"]
            speeds[int(row["Log"])] = float(speed) if speed else None
    return speeds


def read_quads(path):
    """Quad name -> {Log number: distances}, and quad name -> level."""
    quads = defaultdict(dict)
    levels = {}
    with open(path, newline="") as f:
        reader = csv.reader(f, delimiter=";")
        next(reader)
        for row in reader:
            quads[row[1]][int(row[0])] = [float(x) for x in row[4:]]
            levels[row[1]] = int(row[2])
    return quads, levels


def stats(values):
    if not values:
        return None
    values = sorted(values)
    return {"count": len(values), "median": statistics.median(values),
            "p90": values[int(len(values) * 0.9)], "max": values[-1]}


def edges(distances):
    """The four edges of a quad, each as the distances of its vertices in order."""
    grid = [distances[z * SIDE:(z + 1) * SIDE] for z in range(SIDE)]
    return [grid[0], grid[-1], [row[0] for row in grid], [row[-1] for row in grid]]


def profile(edge, step):
    """How the distances rise and fall along an edge, rounded to step millimetres: the same for both quads
    of a shared edge, whatever step lies between them."""
    return tuple(round((b - a) / step) for a, b in zip(edge, edge[1:]))


def same_quad_changes(quads, levels, speeds, level):
    """The largest change of a vertex of each quad of level, between two Logs it appears in one after the
    other, sorted by the speed at the second one."""
    slow, fast = [], []
    for name, logs in quads.items():
        if levels[name] != level:
            continue
        numbers = sorted(logs)
        for a, b in zip(numbers, numbers[1:]):
            change = max(abs(y - x) for x, y in zip(logs[a], logs[b]))
            speed = speeds.get(b)
            (slow if speed is not None and speed < SLOW else fast).append(change)
    return slow, fast


def steps(quads, levels, level, profile_mm, relief_m):
    """The step across each shared edge between two quads of level, per Log: (siblings, cousins both
    written from the first Log on, other cousins)."""
    siblings, cousins_first, cousins_flight = [], [], []
    first = {name: min(logs) for name, logs in quads.items()}
    opening = min(first.values())
    numbers = sorted({n for logs in quads.values() for n in logs})
    for number in numbers:
        present = [(name, logs[number]) for name, logs in quads.items()
                   if number in logs and levels[name] == level]
        index = defaultdict(list)
        for name, distances in present:
            for edge in edges(distances):
                if sum(abs(b - a) for a, b in zip(edge, edge[1:])) >= relief_m * 1000.0:
                    index[profile(edge, profile_mm)].append((name, edge))
        seen = set()
        for name, distances in present:
            for edge in edges(distances):
                # The neighbour runs along the same edge the other way round as often as not.
                reverse = edge[::-1]
                for candidate in (edge, reverse):
                    for other, other_edge in index.get(profile(candidate, profile_mm), []):
                        pair = (min(name, other), max(name, other))
                        if other == name or pair in seen:
                            continue
                        apart = [y - x for x, y in zip(candidate, other_edge)]
                        if max(apart) - min(apart) > 1.0:
                            continue
                        seen.add(pair)
                        step = abs(statistics.mean(apart))
                        if name[:-1] == other[:-1]:
                            siblings.append(step)
                        elif first[name] == opening and first[other] == opening:
                            cousins_first.append(step)
                        else:
                            cousins_flight.append(step)
    return siblings, cousins_first, cousins_flight


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("logs", help="the logs-....csv file")
    parser.add_argument("quads", help="the quads-....csv file")
    parser.add_argument("--profile", type=float, default=20.0, help="mm to which an edge's profile is rounded")
    parser.add_argument("--relief", type=float, default=1.0, help="m of relief an edge needs to be compared")
    parser.add_argument("--json", help="also write the figures to this file")
    options = parser.parse_args()

    speeds = read_logs(options.logs)
    quads, levels = read_quads(options.quads)
    level = max(levels.values())
    slow, fast = same_quad_changes(quads, levels, speeds, level)
    siblings, cousins_first, cousins_flight = steps(quads, levels, level, options.profile, options.relief)
    summary = {
        "logs": len(speeds), "highestLevel": level,
        "quadsOfHighestLevel": sum(1 for n in levels if levels[n] == level),
        "sameQuadUnder100": stats(slow), "sameQuadOver100": stats(fast),
        "siblingSteps": stats(siblings), "cousinSteps": stats(cousins_first + cousins_flight),
        "cousinStepsFromFirstLog": stats(cousins_first), "cousinStepsBuiltInFlight": stats(cousins_flight),
    }
    print("%d Logs, %d quads of level %d" % (summary["logs"], summary["quadsOfHighestLevel"], level))
    for label, key in (("same quad, next Log, under 100 m/s", "sameQuadUnder100"),
                       ("same quad, next Log, over 100 m/s", "sameQuadOver100"),
                       ("step between siblings", "siblingSteps"),
                       ("step between cousins", "cousinSteps"),
                       ("  both there from the first Log", "cousinStepsFromFirstLog"),
                       ("  one at least built in flight", "cousinStepsBuiltInFlight")):
        s = summary[key]
        print("%-38s" % label + ("none" if s is None else
              "%6d   median %8.3f   p90 %8.3f   max %8.3f mm" % (s["count"], s["median"], s["p90"], s["max"])))
    if options.json:
        with open(options.json, "w", newline="") as f:
            json.dump(summary, f, indent=1)


if __name__ == "__main__":
    main()
