# What the measurements show

Part of [KSP Diag - Quad Seams](../README.md): what [the measurements](the-measurements.md) say so
far, on Earth under Real Solar System (case 1, seventeen loads) and on Kerbin (case 2, seven loads).

## The seam is open

The vertices the two quads of a seam are supposed to share are not at the same place. On Earth, over
seventeen loads, the largest gap of a load went from 0.85 m to 2.52 m, and the mean over all shared
vertices from 0.31 m to 1.64 m. On Kerbin, over seven loads, the largest gap went from 118 mm to 312 mm,
and the mean from 46 mm to 225 mm: much smaller, as the precision of a float is finer on a smaller
body, but there as well. The gap changes from one load to the next, and so does its direction: at the
largest gap, the finer quad was above the coarser one in eight loads of seventeen on Earth, and in one
of seven on Kerbin.

It can be seen, but it takes looking for. Each case has one pair of screenshots, taken at the load the
script stopped at: on Kerbin, a straight dark line runs through the foot of the yellow line; on Earth,
a much fainter one. In both, the screenshot taken right after, with the triangles drawn, puts it on the
edge between the coarser quad and the finer one.

## Where to look from

The seam is looked at from beyond it, the camera on the side of the coarser quad, at the first load
where the finer quad was above at the largest gap — 131 mm above on Kerbin, 0.95 m on Earth — the
camera some 50 m beyond the foot of the yellow line and 20 m above it. Which side is higher is only a hint: how close the
camera stands, and how the seam runs across the view, matter at least as much. A crack can also show
elsewhere along the seam than at the largest gap: the log only describes that one vertex.
