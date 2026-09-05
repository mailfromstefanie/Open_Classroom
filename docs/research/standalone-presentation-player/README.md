# Standalone Presentation Player Research

This folder is reserved for architecture/research material supporting the standalone StefanieInVR Presentation prefab.

## Upload target

Upload the completed Work report here, preferably with this filename:

`Standalone_VRChat_Presentation_Player_Architecture_Decision_2026-09-05.docx`

The report is research evidence, not automatic project truth. Accepted implementation decisions are recorded separately in:

- `/PRESENTATION_ARCHITECTURE_DECISION.md`
- `/PRESENTATION_INTEGRATION_PLAN.md`
- `/CURRENT_WORK.md`

## Current interpretation rule

When the report and later real Unity/VRChat tests differ, the tested project result wins.

Important current V1 choice:

- standalone Presentation Core;
- one own `VRCUnityVideoPlayer`;
- sync only presentation mode + slot + slide + revision;
- playback/seek/pause happens locally on every client;
- paused hold is the default V1 behavior;
- snapshot-and-stop is only a later optional performance experiment, not a requirement;
- VideoTXL is an optional typed integration for Stef's Classroom, not a Core dependency.
