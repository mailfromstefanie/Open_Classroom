# Recovery Decision — 2026-09-10

## Status

Stef rejected the current end-of-evening Unity scene as a usable baseline.

Observed by Stef in the real Unity project after the 9/10 September work:
- the tablet/canvas layout is visually badly disturbed;
- multiple / all relevant buttons no longer behave correctly;
- part of the problem is visual/layout-specific and is not practical to repair reliably through more remote automated editing;
- therefore the correct next action is **recovery, not repair**.

This user-observed result overrides earlier editor/Play Mode smoke notes that described the Content-tab integration as successful.

## Decision

**All Unity scene/UI/runtime work from this evening is to be discarded by restoring Stef's full backup from the beginning of the evening.**

The beginning-of-evening backup becomes the candidate baseline after Stef verifies it visually and functionally.

Do not attempt to merge pieces from the currently broken scene into the backup during recovery.

Do not try to "fix forward" the current scene.

## What this means for tonight's work

The following work may remain useful as design/reference knowledge in GitHub, but must **not** be treated as accepted Unity baseline after recovery unless Stef later rebuilds and tests it again:

- Entrance Text V1.1 title/body UI additions made tonight;
- Persistent Poster scene/runtime integration made tonight;
- the new `eDit` / `Panel (Content)` tab at index 8;
- moving the Entrance Text editor into the Content panel;
- poster editor placement in the Content panel;
- related button/tab wiring and scene layout changes;
- any other scene/runtime/UI modifications made after the selected beginning-of-evening backup.

Documentation about the intended Entrance Text and Poster architecture may be reused later as reference only.

## Exact next session

1. Restore Stef's full backup from the beginning of the evening over the working Unity project.
2. Open the restored project in Unity and allow it to compile fully.
3. Do **not** add or repair features yet.
4. Verify visually that the old tablet/canvas layout is back.
5. Verify that the pre-existing buttons work again.
6. Confirm Presentation, VideoTXL 2.5.1, e-reader/library, reset systems and other previously working systems are still present.
7. Stef decides whether this restored state is accepted as the new baseline.
8. **STOP.**

Only after Stef explicitly accepts the restored backup:
- rebuild desired UI pieces mostly by hand;
- use Nova/ChatGPT for visual/manual guidance;
- use Codex only for very small, clearly bounded technical tasks;
- make one change at a time;
- test immediately after each change.

## Hard rules for Codex after recovery

- Do not rebuild the full Content tab automatically.
- Do not redesign or reposition large parts of the Canvas.
- Do not batch multiple UI changes.
- Do not infer visual intent from hierarchy alone.
- Do not touch working Presentation, VideoTXL, e-reader, library, reset or unrelated tablet systems without explicit instruction.
- For UI tasks, inspect first and wait for Stef to specify the exact small change.
- A successful compile or Play Mode smoke test is not enough to call a visual/UI result accepted; Stef's visual check is required.

## GitHub truth

GitHub keeps the technical history of what was attempted tonight so useful ideas are not lost.

However:

**Attempted/implemented tonight != accepted baseline.**

After backup recovery, the restored and user-verified Unity project is the authoritative scene truth.
