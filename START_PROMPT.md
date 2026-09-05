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
- Stef confirms Open Classroom is already ready for beta testing with the Presentation Service.
- The reusable Presentation prefab is the active next task.
- The repository does not currently contain a committed `PresentationController.cs`, so the real Unity scene is newer or contains setup not yet captured in GitHub.
- Do not rebuild the Classroom presentation system from an old plan.

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

Then inspect the current presentation-related hierarchy and components in the real Classroom scene.

The first goal is only to learn what is already working:

- slot selection;
- Previous / Next / First;
- slide/page state;
- pause/seek;
- VideoTXL integration;
- projector/screen references;
- Back to Video if present;
- tablet/access UI wiring.

CHANGE NOTHING until that inventory is clear.

## Prefab goal

```text
working beta-ready Classroom presentation setup
-> identify reusable boundary
-> preserve VideoTXL/player/screen architecture
-> separate scene-specific references
-> expose clean prefab configuration
-> test Slot 1
-> test navigation
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
