# What the measurements show: the quads of the highest level, in flight

Part of [KSP Diag - Quad Seams](../README.md): what [the measurements of the quads in flight](the-measurements-flight.md)
say so far, on Kerbin, one flight of 72 *Logs*.

## Quads built at different moments do not meet

Two quads of the highest level that share an edge should put its vertices at the same distance from the
centre of the body. Built at the same moment, they do: two quads split from the same quad agree to
0.42 mm at most, and so do two quads built as the scene opened, around the launchpad, 0.40 mm at most.
Built at different moments, as the craft flies on, they do not: across an edge between a quad built
during the flight and its neighbour, the ground steps up or down by 4.5 mm in the median, by 16.9 mm or
less nine times out of ten, and by up to 22.5 mm. Each quad lands at its own height, and the terrain is
no longer one surface where two of them meet.

## Once built, a quad stays where it is

While the craft flew faster than 100 m/s, a quad written in two *Logs* one after the other came back with
every vertex at the same distance from the centre of the body, to 0.70 mm at most, through 2,365 moves of
the world, most of them made every frame. Whatever height a quad was built at, it keeps it as long as it
lives. Under a rover driving on the ground, the same moves of the world move the ground by some 25 mm on
Kerbin
([KSP Diag - Terrain Height, driving on while the world moves](https://github.com/lhervier/KSP-Diag-TerrainHeight/blob/master/docs/the-measurements-driving.md));
here, in flight, they did not.

## What the flight does not say

- **What a move of the world does at low speed.** The world did not move while the craft was slower than
  100 m/s, so the first eleven *Logs* compare quads across no move at all.
- **A gap beside the edge.** The *Log* writes distances from the centre of the body: a step up or down
  shows, a gap or an overlap along the ground does not.
- **Other bodies, other flights.** One flight, on Kerbin, where a float's step at the radius of the body
  is 62.5 mm.

Why each quad lands at its own height is explained by
[Terrain Precision Fix](https://github.com/lhervier/KSP-TerrainPrecisionFix), in
[The culprit: the ground](https://github.com/lhervier/KSP-TerrainPrecisionFix/blob/master/docs/the-culprit-ground.md).
