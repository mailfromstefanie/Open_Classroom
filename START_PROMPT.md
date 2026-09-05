# Start Prompt — Open Classroom Presentation Prefab

Use this file to start the next fresh ChatGPT session.

## Project

Primary repository:

`mailfromstefanie/Open_Classroom`

Related product/service repository:

`mailfromstefanie/StefanieInVR-Presentation-Service`

Later target:

`mailfromstefanie/Stefanies-Art-House-Cinema`

## Read first

1. `AGENTS.md`
2. `CURRENT_WORK.md`
3. `PRESENTATION_INTEGRATION_PLAN.md`
4. `VIDEOTXL_2_5_1_FINDINGS.md` only when VideoTXL details are needed
5. Presentation Service `CURRENT_WORK.md` for hosted/live truth

## Exact current truth

- Presentation Service Free Beta is live.
- Website refresh is live.
- Stef confirms the previous Presentation integration in Open Classroom has been dismantled.
- Do not describe the current Classroom as beta-ready with the Presentation Service.
- Rebuilding the Classroom Presentation integration is the active next task; prefab hardening comes after a working path is proven.
- The repository does not currently contain a committed `PresentationController.cs`.
- Reuse the existing VideoTXL player/screen foundation; do not add a duplicate player by default.

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

## How to work with Stef

- Speak Dutch.
- One tiny Unity action at a time.
- Explain what the action is meant to prove.
- Stef performs Unity scene work manually.
- Give full replacement scripts when code is needed; never snippets.
- Do not use CMD/PowerShell unless unavoidable.
- Do not redesign working behaviour before inspecting it.
- Update GitHub when a meaningful state or proof changes.

## First action

Ask Stef to make sure Unity is OUT OF PLAY MODE.

Then inspect the real Classroom scene around the existing VideoTXL player, projector/screen, playlist/source objects and tablet UI.

The first goal is to learn what foundation remains after the previous Presentation setup was dismantled.

CHANGE NOTHING until that inventory is clear.

After that, rebuild the smallest working path in this order:

- one Presentation Slot through the existing VideoTXL player/screen;
- pause/seek;
- Previous / Next / First;
- slide/page state;
- additional slots;
- Back to Video only after the core path is proven;
- then reusable prefab hardening.

## Prefab goal

```text
current Classroom VideoTXL/player/screen foundation
-> rebuild one working Presentation Slot
-> prove navigation
-> identify reusable boundary
-> preserve VideoTXL/player/screen architecture
-> separate scene-specific references
-> expose clean prefab configuration
-> test shared/multiplayer behaviour
-> test Quest
-> package for later Cinema/other-world reuse
```

Default architecture:

- existing VideoTXL 2.5.1 SyncPlayer;
- same existing screen/player;
- one slide = one second;
- no duplicate video player unless real testing proves necessary.

Do not start Website Editor, eReader or Cinema implementation in this chat unless Stef explicitly changes priority.
