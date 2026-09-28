# What the measurements show

Part of [Terrain Precision Fix Diag 4](../README.md): what [the measurements](the-measurements.md) say so
far, on Earth under Real Solar System (case 1, fifteen loads) and on Kerbin (case 2, one load).

## The seam is open

The vertices the two quads of a seam are supposed to share are not at the same place. On Earth, over
fifteen loads, the largest gap of a load went from 1.01 m to 3.05 m, and the mean over all shared
vertices from 0.31 m to 2.14 m. On Kerbin, the one load measured left a largest gap of 102 mm, and a
mean of 37 mm: much smaller, as the precision of a float is finer on a smaller body, but there as well.
The gap changes from one load to the next, and so does its direction: on Earth, the finer quad was above
the coarser one at the largest gap in seven loads, below it in eight.

It can be seen, but it takes looking for. On Earth, two of seven screenshots show a crack: a faint one
along a seam in load 1, and a clear straight line in load 15. On Kerbin, the one screenshot taken shows
a clear straight line too. In both clear cases, the screenshot taken right after, with the triangles
drawn, puts the line exactly on the edge between the coarser quad and the finer one.

## Where to look from

The seam is looked at from beyond it, the camera on the side of the coarser quad. On Earth, where the
finer quad was above at the largest gap, one screenshot out of three shows a crack at the foot of the
yellow line, for steps a little under a metre; where it was below, one out of four shows a faint line,
for a step of two metres. On Kerbin, the finer quad was below, by 77 mm, and the crack is clear, the
camera close to the seam. Which side is higher is only a hint: how close the camera stands, and how the
seam runs across the view, matter at least as much. A crack can also show elsewhere along the seam than
at the largest gap: the log only describes that one vertex.
