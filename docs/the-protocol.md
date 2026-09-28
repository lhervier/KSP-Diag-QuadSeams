# The protocol

Part of [Terrain Precision Fix Diag 4](../README.md): two cases, step by step. What the drawing and the
log mean is in [What it shows](what-it-shows.md).

The two cases are the same steps on two installs: Real Solar System, whose Earth is the largest body a
player is likely to land on, and stock KSP.

## Before you start

- **Screenshots**: set `SCREENSHOT_SUPERSIZE = 4` in `settings.cfg`, KSP closed. A screenshot (F1) is
  then taken at four times the size of the window. A gap of a metre, some thirty kilometres away, is a
  couple of pixels in such a screenshot and less than one on screen.
- **Fine zoom keys**: set `FLT_CAMERA_ZOOM_SENS = 0.05` in `settings.cfg`, KSP closed (it is `0.5` by
  default on a fresh install). The zoom keys, + and − on the keypad, change the distance of the camera
  a little at every frame they are held down, by an amount this setting scales: at `0.05`, they are slow
  enough to stop the camera within a few hundred metres of a point thirty kilometres away. The mouse
  wheel does not depend on it.
- **A craft on the launchpad**: any craft will do. Launch it from the VAB and leave it on the pad.
  Every reload in the steps below is *Revert to Launch*, from the pause menu (Escape).

## Why reload until the finer quad is above

The two edges of a seam are not joined by anything: where they do not meet, the terrain has an opening
along the seam, and what lies behind the terrain shows through it as a thin dark line. Whether you can
see that opening depends on where you look from:

- when the edge **further from the camera is the higher one**, the opening faces the camera, and the
  dark line shows best;
- when the edge **nearer the camera is the higher one**, it hides most of the opening.

The size of the step matters too: the larger it is, the more easily the line shows.

The camera of a flight turns around the craft, and the seam is far from it. The easy way to see the
seam from close is to pull the camera back beyond it and look back towards the craft: the coarser quad
is then the nearer one, and the opening shows best when the finer quad is **above** the coarser one.
The log says which it is for the largest gap. This is a reading of the screenshots of
[the two cases](what-the-measurements-show.md#where-to-look-from), and only a hint: a line can show with
the finer quad below, clearly when the camera stands close, and a crack can show elsewhere along the
seam.

The gap changes from one load to the next, and so does its direction: reloading until the log says
"above", with a step large enough to show, can take many tries. The first load after starting the
game, the craft taken back from the Space Center, gave exactly the same gap both times it was tried:
count from the first revert.

## The steps

1. Revert to Launch. Wait for the log line that comes a second or two after the reload, the first one
   without the `after an origin shift:` prefix: the seams are all built by then.
2. If it says the finer quad is **below** the coarser one, go back to step 1. If the yellow line stands
   in the sea, go back to step 1 too: the surface of the sea is drawn over the terrain and hides the seam.
3. Press F8 once, then once more, until the message at the top of the screen says `largest gap only`.
4. Bring the camera close to the foot of the yellow line. The seam cannot be driven to, since it moves
   away with the craft, but the camera can be pulled back from the craft far enough to stand next to
   it. Turn the camera until the line stands between it and the craft, then pull it back to a little
   more than the distance the log gives for the largest gap, so that it stands just beyond the line,
   and keep it low. The mouse wheel changes the distance by steps of about a tenth, kilometres at that
   range; the zoom keys, slowed down as in *Before you start*, are the ones for the last few hundred
   metres. The closer the camera, the larger the gap on screen. Alt with the mouse wheel also narrows
   the field of view.
5. Look at the foot of the yellow line, and take a screenshot (F1). Then, without touching the camera,
   press F8 until the message says `seams and largest gap`, and take a second one: it shows whether a
   dark line of the first screenshot runs along a seam, or along a line inside a quad.

## Case 1: Real Solar System

Real Solar System as released, on KSP 1.12, with KSP Community Fixes. The launchpad is at Cape
Canaveral. The steps above.

**→ Measured in [case 1](the-measurements.md#case-1-real-solar-system)**

## Case 2: stock KSP

KSP 1.12 with KSP Community Fixes. The launchpad is at the Space Center, on Kerbin. The steps above.

**→ Measured in [case 2](the-measurements.md#case-2-stock-ksp)**
