# Entrance Text Investigation & Implementation Plan — 2026-09-09

Status: **READ-ONLY INVESTIGATION COMPLETE / IMPLEMENTATION APPROVED TO START**

This document records the current entrance-system scene truth and the agreed V1 plan for Persistent Entrance Text.

The real Unity scene remains authoritative:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

Stef has already made a fresh full backup before implementation work.

## CURRENT ENTRANCE SCENE TRUTH

Codex completed a read-only investigation of the real Unity scene and reported no changes made during that investigation.

Current hierarchy:

```text
UIs
└── Other Toggles and Systems
    └── Canvas (Welcome)
        ├── Collider_Entrance
        ├── Image
        ├── Image
        ├── Text (Kop)
        ├── Text (Alinea)
        └── Button (Open)
            └── Text (Open)
```

Important current details:

- `Canvas (Welcome)` is a World Space Canvas.
- `Text (Kop)` is a `TextMeshProUGUI` fixed heading.
- `Text (Alinea)` is a `TextMeshProUGUI` and is the welcome body text that will become editable.
- `Text (Alinea)` is currently hardcoded in the Inspector.
- no UdonSharp/C# script currently controls the entrance text.
- no Animator or networked entrance controller is involved.
- `Button (Open)` currently calls `Canvas (Welcome).SetActive(false)`.
- because `Collider_Entrance` is a child of the welcome Canvas, pressing Open also disables the local physical blocker.
- visitor dismissal is completely local: one visitor pressing Open does not dismiss another visitor's welcome screen.
- late joiners receive the scene default welcome Canvas active and must dismiss it themselves.

This local visitor-dismiss behaviour is intentional and must be preserved.

## COLLIDER FINDING

`Collider_Entrance` is a non-trigger MeshCollider beneath the scaled World Space Canvas.

It currently works, but its local transform compensates heavily for the Canvas scale. That makes it fragile if the Canvas is later moved, rotated or rescaled.

Decision for V1:

- do **not** move `Collider_Entrance`;
- do **not** change the Open button;
- do **not** synchronize Canvas/blocker visibility;
- treat collider separation as a later independent cleanup only after the text feature is stable.

## PRODUCT INTENT

The entrance text is controlled by the teacher/host running that Classroom session.

Two different lifetimes are required:

```text
PlayerData
= teacher's personal saved entrance text
= persists across visits and future instances

synced currentEntranceText
= the text currently active in this one VRChat instance
= persists only while that instance exists
```

Desired lifecycle:

```text
authorized teacher creates a new Classroom instance
-> PlayerData restoration completes
-> teacher's saved personal text (or default) initializes the instance text
-> everyone sees the same current entrance text
-> late joiners receive the same current entrance text
```

During the same session the teacher may edit repeatedly:

```text
type locally
-> Apply / Save
-> save personal PlayerData value
-> update synced instance value
-> publish once
```

Typing itself must never be synchronized.

If the original teacher later leaves:

- do not clear the current entrance text;
- do not reset to default;
- do not seed from another player's PlayerData;
- players who remain keep the last published text;
- late joiners still receive that same synced text.

The shared instance text disappears naturally only when the VRChat instance ends.

If the teacher later creates a new instance, their latest PlayerData value can seed the new instance again.

## AUTHORITY MODEL

Keep these concepts separate:

```text
teacher authorization
!=
network object ownership
!=
VRChat Master
```

Rules:

- do not use `isMaster` as teacher identity;
- use existing `VipAccessManager.IsLocalPlayerVip()` as authorization input;
- for Invite / Invite+ / Friends / Friends+ instances, `Networking.IsInstanceOwner` is the preferred automatic host signal;
- network ownership is only the technical ability to serialize the manager;
- another VIP joining an already initialized instance must not automatically publish their own PlayerData or overwrite the current text.

For Group/Public/Build & Test, where automatic instance-creator detection is not reliable, use a small explicit teacher-only **Start / Claim Classroom** fallback if required.

V1 should not overbuild takeover logic. Priority is:

1. original authorized host can initialize;
2. host can edit repeatedly;
3. published text survives host leaving;
4. late join works.

Any richer teacher takeover flow is optional unless needed by implementation constraints.

## AGREED V1 ARCHITECTURE

Use the smallest isolated system.

### EntranceTextManager.cs

Proposed GameObject:

`UIs/Managers/Entrance Text Manager`

Single responsibility: entrance-text state.

Responsibilities:

- reference existing `Canvas (Welcome)/Text (Alinea)`;
- wait for local `OnPlayerRestored`;
- load/save one PlayerData string;
- maintain manually synced current instance text;
- maintain an initialized flag;
- use only minimal additional authority state if genuinely necessary;
- initialize a fresh instance once;
- validate Apply requests;
- update `Text (Alinea)`;
- handle deserialization / late join;
- preserve the shared value across host departure and network ownership changes;
- consult `VipAccessManager` for teacher authorization.

It must not control:

- Canvas visibility;
- `Collider_Entrance`;
- general VIP logic;
- Presentation / VideoTXL / VRCDN;
- e-reader / table-screen / Marker Pro systems.

### EntranceTextEditorUI.cs

Single responsibility: local teacher editing UI.

Responsibilities:

- local `TMP_InputField` draft;
- Apply / Save;
- Reset to Default;
- simple local status feedback;
- optional explicit Claim/Start control where instance type requires it;
- forward publish requests to `EntranceTextManager`.

No synced variables and no direct PlayerData writes in the editor UI.

Proposed UI:

```text
Entrance Text Editor
├── TMP_InputField
├── Button (Apply Save)
├── Button (Reset Default)
├── Button (Claim / Start)   [only when needed]
└── Text (Status)
```

## APPLY / SAVE CONTRACT

Apply / Save should:

1. read the local draft;
2. normalize and validate it;
3. require PlayerData restoration;
4. require teacher/VIP authorization;
5. obtain manager-object ownership if needed for serialization;
6. save the validated personal string with PlayerData;
7. update the synced current instance text;
8. keep instance initialization true;
9. update `Text (Alinea)` immediately;
10. call `RequestSerialization()` once;
11. show local success/error feedback.

Multiple edits in one session are supported.

Reset to Default should only load the default text into the local draft. The teacher must still press Apply / Save to publish it.

## VALIDATION

Initial plan:

- approximately 400 characters maximum;
- approximately 9 explicit lines maximum;
- plain-text treatment;
- normalize line endings;
- reject over-limit text with clear local feedback;
- whitespace-only input resolves to the known default;
- do not change the existing `Text (Alinea)` font/layout/overflow behaviour as part of the networking implementation unless functionality requires it.

Layout refinement is a separate follow-up after real text behaviour is proven.

## PRESERVE / DO NOT TOUCH

During V1 implementation preserve:

- existing `Canvas (Welcome)`;
- `Text (Kop)`;
- `Text (Alinea)` as display target;
- `Button (Open)`;
- `Collider_Entrance` and its exact working placement;
- local per-visitor dismissal;
- Presentation;
- exact VideoTXL 2.5.1;
- VRCDN;
- projector/display/readability;
- e-readers;
- table screens;
- Marker Pro reset;
- unrelated tablet/reset systems.

No broad refactor.

## IMPLEMENTATION STATUS / NEXT

Codex is now allowed to implement V1 in the real Unity project.

Working instruction:

- be token-efficient;
- do not repeat the previous investigation;
- inspect only directly relevant files/objects;
- create only the two small scripts and minimal manager/editor wiring;
- use complete scripts;
- do meaningful local/ClientSim checks only;
- do not claim real VRChat multiplayer, late-join or persistence PASS until tested with real clients.

Required real follow-up proof after implementation:

- two-client shared text update;
- repeated Apply;
- late join receives newest text;
- one visitor pressing Open does not affect another;
- host leaves and text remains;
- persistence into a fresh later instance;
- Group/Public fallback only if that route is intended for use.

## SOURCE-OF-TRUTH RULE

This document records the read-only investigation and agreed plan.

Once Codex implements the feature, the tested real Unity scene may become newer than this plan. Record actual implementation and test evidence separately rather than forcing the scene back to this planned shape.
