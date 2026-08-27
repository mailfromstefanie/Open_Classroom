# Current Work — Open Classroom

Last updated: 2026-08-27 (Europe/Amsterdam)

## STATUS

**PLAYLIST ACCESS / VIDEOTXL MULTIPLAYER BLOCKER CLEARED ✅**

The real VRChat multiplayer test now passes after simplifying playlist privacy to **button visibility only**.

Open Classroom is no longer blocking the StefanieInVR Presentation Service.

## ACCEPTED ACCESS MODEL

Playlist access means which custom selection buttons a local user may see/use:

```text
Visitor
→ public playlist selection buttons

VIP/admin
→ public + VIP playlist selection buttons

StefanieInVR
→ public + VIP + owner-only selection buttons
```

Once any playlist/source is active:

- VideoTXL owns playback and synchronization;
- synchronized video/audio may reach everybody;
- VideoTXL playlist content/UI may be visible;
- permitted users may select another playlist using their own visible custom buttons;
- no extra privacy is applied to `VideoSourceUI/Content`.

Tablet Lock remains a separate system. Non-VIPs still receive the existing locked/dummy UI when the tablet is locked.

## FINAL RESPONSIBILITY SPLIT

```text
VideoTXL
→ playlist/source objects
→ playback
→ synchronization
→ playlist content UI

VipAccessManager / tablet UI
→ public/VIP access
→ VIP content
→ tablet lock
→ stage blocker

TXLPlaylistPrivacyFilter
→ local owner-only selection-button visibility only
```

Do not add VideoTXL/CommonTXL `AccessControl` for per-playlist visibility unless a future requirement actually needs it.

## IMPLEMENTED SIMPLIFICATION ✅

`TXLPlaylistPrivacyFilter.cs` was reduced to one local responsibility:

- keep `ownerDisplayName`;
- keep `GameObject[] ownerOnlyButtons`;
- compare `Networking.LocalPlayer.displayName` locally;
- show owner-only GameObjects only to the configured owner.

Removed from that script:

- VideoSourceUI references;
- playlist Content hiding;
- SourceManager binding;
- source-ready events;
- VIP/owner playlist classification;
- active/current source privacy logic;
- all VideoTXL-content manipulation.

Scene wiring now uses the full object:

`PlaylistLoadButton (StefanieInVR Stream)`

The same object was removed from `VipAccessManager.enableWhenVip`, so VIP status cannot turn the owner-only button on.

`VipAccessManager.enableWhenVip` is currently empty.

## UDON SYNC WARNING — FIXED ✅

`TXLPlaylistPrivacyFilter` and `VipAccessManager` are on the same GameObject.

The simplified privacy script was initially set to `BehaviourSyncMode.None`, while `VipAccessManager` uses `BehaviourSyncMode.Manual` and contains synchronized state.

Unity/UdonSharp warned about mixing sync methods on the same GameObject.

The privacy script was changed to:

`[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]`

It still has no own synced fields and does not request serialization. The change only keeps the UdonBehaviours on that GameObject compatible and removes the warning.

## REAL VRCHAT MULTIPLAYER PROOF — PASSED ✅

Tested in the freshly uploaded world.

Proven:

- StefanieInVR sees the owner-only `StefanieInVR Stream` selection button;
- VIP non-owner does not see the owner-only stream selection button;
- ordinary users cannot enter the VIP panel, so they cannot access that owner-only button;
- synchronized playback reaches VIPs and ordinary users;
- users may use their own permitted playlist buttons;
- after the simplification, the previously observed wrong/missing playlist UI behaviour could no longer be reproduced during the full retest;
- playlist switching and the VideoTXL UI behaved correctly in that retest.

Important evidence distinction:

The disappearance of the intermittent UI problem after removing Content-hiding is strong evidence that the old privacy/content interaction was involved. The exact internal timing/root cause was not instrumented separately, so do not claim a more specific mechanism than the test proves.

## BLOCKER RESULT

The accepted gate is now satisfied:

```text
owner-only source selection is hidden from unauthorized users
+ synchronized playback still works
+ permitted playlist switching works
+ VideoTXL UI remains usable
+ normal tablet/VIP access remains intact
```

**Result: PASS.**

No further VideoTXL/privacy work is required before resuming the Presentation Service.

## ACTIVE UNITY PROJECT

Use the actual Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

Do not confuse it with the separate older local checkout:

`E:/GitHub/Open_Classroom`

GitHub `mailfromstefanie/Open_Classroom` `main` remains durable project memory.

## CODEX + UNITY MCP

Working route:

```text
Unity Editor
→ KitWright MCP for Unity
→ Direct HTTP
→ Codex CLI
```

Keep future Codex tasks small and precisely scoped. Prefer read-only inspection until a specific change is approved.

## NEXT PROJECT

Return to:

`mailfromstefanie/StefanieInVR-Presentation-Service`

Resume:

**Milestone 3 — one real private Free Plan presentation slot.**

## FUTURE OPEN CLASSROOM IDEAS — PARKED

Later, the architecture may support a central content manager for multiple worlds, for example:

```text
StefanieInVR Content Manager
→ Open Classroom catalog
→ Open Arthouse Cinema catalog
```

Potential fields: title, description, image URL, media URL, category, order, enabled state and access level.

Do not build this while Presentation Service Milestone 3 is active.

## DURABLE VIDEOTXL REFERENCE

Before future player, playlist, remote-catalog or Cinema reuse work, read:

`VIDEOTXL_2_5_1_FINDINGS.md`

It records the verified VideoTXL 2.5.1 behaviour, the final button-only access model, the meaning of the PlaylistData JSON Inspector box, runtime PlaylistData/_LoadData building blocks, and the future online-JSON playlist/content-manager direction with explicit items that still require testing.

Do not reconstruct these facts from old chats when this file exists.

## WORKING RULE

```text
inspect
→ make one small permanent-oriented change
→ test in real VRChat where multiplayer matters
→ record proven result
→ continue only after proof
```
