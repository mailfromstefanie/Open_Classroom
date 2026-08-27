# Current Work — Open Classroom

Last updated: 2026-08-27 (Europe/Amsterdam)

## ACTIVE GOAL

Simplify playlist access to **button visibility only**, then re-test the real VRChat multiplayer behaviour before returning to the StefanieInVR Presentation Service.

The previous multiplayer gate exposed two separate facts:

1. owner/VIP/public **selection-button visibility works as the real access boundary**;
2. hiding `VideoSourceUI/Content` is not part of Stef's actual requirement and introduces unnecessary state/timing complexity.

The intermittent wrong/missing playlist shown in the local VideoTXL UI remains a separate issue to investigate **after** the privacy simplification is implemented and tested.

## ACCEPTED ACCESS MODEL — 2026-08-27

Privacy/access for playlists means only:

```text
Visitor
→ public playlist selection buttons

VIP/admin
→ public + VIP playlist selection buttons

StefanieInVR
→ public + VIP + owner-only playlist selection buttons
```

Once any playlist/source has been started:

- the synchronized video/audio may be visible to everybody;
- the VideoTXL playlist content/UI may be visible to everybody;
- other users may still select/start other playlists for which they have a visible permitted button;
- no additional privacy is required on `VideoSourceUI/Content`.

Tablet Lock remains a separate existing system. When it is enabled, non-VIPs get the existing locked/dummy view and cannot operate the normal tablet controls.

## RESPONSIBILITY SPLIT

```text
VideoTXL
→ playlist/source objects
→ playback
→ synchronization
→ playlist content UI

VipAccessManager / tablet UI
→ public/VIP access
→ tablet lock
→ VIP content

small owner-only visibility logic
→ show owner-only selection button only to StefanieInVR
```

Do not add VideoTXL/CommonTXL `AccessControl` for per-playlist visibility.

## MULTIPLAYER TEST — IMPORTANT RESULT

Real VRChat testing with Stef, a VIP account and a normal user proved:

- `StefanieInVR Stream` was visible/selectable to Stef;
- the VIP did not see the owner-only stream selection button;
- synchronized playback still reached the VIP;
- synchronized playback also reached a normal user;
- VideoSourceUI playlist selection/content state is substantially local per user rather than automatically mirroring every remote source selection.

However, while the old Content-hiding privacy filter was active, recovery/navigation was inconsistent:

```text
owner/private source used
→ later public source / VIP interaction
→ VIP sometimes saw correct playlist
→ sometimes wrong/old playlist
→ sometimes no playlist content
```

Do not treat the old multiplayer gate as passed.

## PRIVACY SIMPLIFICATION — READ-ONLY CODEX REVIEW COMPLETE ✅

Current `TXLPlaylistPrivacyFilter` contains responsibilities that are no longer required under the accepted access model.

Remove from its responsibility:

- `videoSourceUI`
- `playlistContentRoot`
- Content `SetActive(...)` privacy
- SourceManager binding
- delayed initialization for privacy
- `EVENT_URL_READY` listener
- `OnVideoSourceReady()`
- `RefreshPrivacy()`
- source classification
- `vipOnlyPlaylists`
- `ownerOnlyPlaylists`
- VideoTXL/current-source dependent privacy logic

Keep only the local owner identity check and owner-only selection-button visibility.

## IMPORTANT CURRENT SCENE WIRING

Current owner identity:

`StefanieInVR`

Current owner-only source:

`Live Stream StefanieInVR`

Current owner-only selection object:

`PlaylistLoadButton (StefanieInVR Stream)`

The existing filter currently targets only its internal `ControlArea/Button`, but the simplified owner visibility should control the **full `PlaylistLoadButton (StefanieInVR Stream)` GameObject**.

Important conflict discovered:

`VipAccessManager.enableWhenVip` currently also references the full `PlaylistLoadButton (StefanieInVR Stream)` object.

That means the VIP manager can activate the owner-only button for every VIP. The owner-only stream object must therefore be removed from `enableWhenVip` when the simplified owner visibility is implemented.

Real multiplayer testing already proved that Stef currently has access to the parent VIP content, because Stef could see and use the stream button in the real instance.

## EXACT NEXT CHANGE — SMALL ONLY

Allowed implementation scope:

1. Simplify the existing local `TXLPlaylistPrivacyFilter.cs` so it only:
   - stores `ownerDisplayName`;
   - stores one or more owner-only button GameObjects;
   - compares `Networking.LocalPlayer.displayName` locally;
   - shows those GameObjects only to the configured owner.
2. Point the owner-only target at the full `PlaylistLoadButton (StefanieInVR Stream)` object.
3. Remove that same full stream button object from `VipAccessManager.enableWhenVip`.
4. Remove obsolete VideoTXL/Content/playlist references from the simplified component.
5. Do not change `VipAccessManager.cs`, `PaperTabletTabManager.cs`, VideoTXL package code, tablet lock logic, other playlist buttons, or any unrelated scene objects.

A script rename is deliberately not required for this first safe change. Keeping the existing component avoids unnecessary Unity reference churn. Rename/cleanup can be considered later only after the behaviour is proven.

## TEST IMMEDIATELY AFTER SIMPLIFICATION

Before investigating the playlist UI timing issue, prove the simplified access model:

### Stef
- sees public buttons;
- sees VIP buttons;
- sees `StefanieInVR Stream` button.

### VIP non-owner
- sees public + VIP buttons;
- does NOT see `StefanieInVR Stream` selection button;
- may see VideoTXL content for a source after it is active;
- may select another permitted playlist.

### ordinary visitor
- does not gain the owner-only selection button;
- receives synchronized playback normally;
- tablet lock behaviour remains unchanged.

## SEPARATE ISSUE — PLAYLIST UI SELECTION/TIMING

Do not mix this into the privacy change.

Observed real multiplayer symptom:

- after source changes, pressing a permitted custom playlist button sometimes shows the correct playlist in VideoSourceUI;
- sometimes an old/wrong playlist is shown;
- sometimes no playlist content appears.

Likely area to inspect later:

```text
TXLPlayPlaylistButton.PlayAndShow()
→ Playlist._MoveTo(0)
→ currentUrlSource/source-ready timing
→ VideoSourceUI._SelectActive()
```

This is still a hypothesis until separately proven.

## BUTTON WIRING — VERIFIED ✅

The active scene contains 12 `TXLPlayPlaylistButton` instances.

All 12 use:

`SendCustomEvent("PlayAndShow")`

The active Unity project contains one relevant script copy:

`Assets/StefanieInVR/Scripts/TXLPlayPlaylistButton.cs`

`PlayAndShow()` is the actual working public method. Earlier Codex output mentioning `Play()` was a reporting error, not scene truth.

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

Use small precise tasks. During inspection, default to read-only. During an approved implementation task, allow only the exact files/scene references named in that task.

## PRESENTATION SERVICE BLOCKER

The Presentation Service remains paused until:

1. simplified button-only playlist access is implemented;
2. it passes a real multiplayer check;
3. the separate playlist-UI selection/timing problem is understood/fixed enough that permitted playlist buttons reliably show their intended playlist.

Then return to:

`mailfromstefanie/StefanieInVR-Presentation-Service`

Milestone 3 — one real private Free Plan presentation slot.

## WORKING RULE

```text
inspect
→ make one small permanent-oriented change
→ test in real VRChat where multiplayer matters
→ record result
→ only then continue
```
