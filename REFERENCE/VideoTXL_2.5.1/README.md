# VideoTXL 2.5.1 Reference

This folder contains the exact VideoTXL package reference used for Stef's current Open Classroom integration work.

## Package

Path:

`REFERENCE/VideoTXL_2.5.1/com.texelsaur.video-2.5.1/`

Verified from `package.json`:

- package name: `com.texelsaur.video`
- display name: `TXL - VideoTXL`
- version: `2.5.1`
- CommonTXL dependency: `com.texelsaur.common ^2.0.0`

## Why this copy exists

This is a **read-only technical reference** for:

- inspecting the exact VideoTXL 2.5.1 code used by the Classroom;
- implementing and reviewing the optional VideoTXL Presentation adapter;
- checking `SyncPlayer.LocalPlaybackEnabled`;
- checking ScreenManager behavior and screen takeover/restoration;
- comparing future VideoTXL versions without guessing from current upstream main.

It is not the reusable Presentation Core.

The Presentation Core must remain independent of VideoTXL.

## Important files

- `Runtime/Scripts/SyncPlayer.cs`
- `Runtime/Scripts/TXLVideoPlayer.cs`
- `Runtime/Scripts/Component/VideoManager.cs`
- `Runtime/Scripts/Component/ScreenManager.cs`
- playlist/source manager scripts under `Runtime/Scripts/`
- `CHANGELOG.md`
- `LICENSE`

## License

VideoTXL is included here under the upstream MIT license.

Original copyright/licence text is preserved in:

`com.texelsaur.video-2.5.1/LICENSE`

Do not remove or rewrite that license.

## Project-truth boundary

Accepted Presentation product architecture lives in:

- `/PRESENTATION_ARCHITECTURE_DECISION.md`
- `/PRESENTATION_INTEGRATION_PLAN.md`
- `/CURRENT_WORK.md`

This reference may be inspected deeply for the VideoTXL adapter, but it must not silently turn VideoTXL back into a Core dependency.
