# VideoTXL 2.5.1 — Verified Findings and Online Playlist Direction

Last updated: 2026-08-27 (Europe/Amsterdam)

Purpose: durable technical memory for the Open Classroom player setup and for reuse by other StefanieInVR worlds, especially Stefanie's Art House Cinema.

> **Scope note — 2026-09-05:** This file remains authoritative reference for the existing Open Classroom VideoTXL 2.5.1 setup, playlist/privacy behavior, and future VideoTXL adapter work. It is **not** the architecture authority for the reusable Presentation prefab. The Presentation Core is now standalone; read `PRESENTATION_ARCHITECTURE_DECISION.md` for that decision.

> **Durable source reference — 2026-09-05:** The exact package is checked into `REFERENCE/VideoTXL_2.5.1/com.texelsaur.video-2.5.1/`. Use that copy for future source inspection of the Classroom adapter.

This file deliberately separates **proven facts**, **accepted project decisions**, and **future ideas that still require implementation/testing**.

---

## 1. Proven Open Classroom architecture

The working responsibility split is:

```text
VideoTXL
→ owns Playlist/source objects
→ owns playback
→ owns synchronization
→ owns playlist content UI

our custom tablet UI
→ decides which playlist SELECTION BUTTONS a local user may see/use

VipAccessManager
→ public/VIP access
→ VIP panel/content
→ tablet lock
→ stage blocker

TXLPlaylistPrivacyFilter
→ local owner-only selection-button visibility only
```

Accepted access rule:

```text
Visitor
→ public playlist buttons

VIP/admin
→ public + VIP playlist buttons

StefanieInVR
→ public + VIP + owner-only playlist buttons
```

Once a source/playlist has been selected, its playback and VideoTXL playlist UI do not need extra privacy. A source may synchronize to everybody even when only one access class has the custom button that can select it.

This was tested in a real uploaded VRChat world with StefanieInVR, a VIP non-owner and an ordinary user.

---

## 2. Real multiplayer evidence

Proven in the 2026-08-27 test:

- `StefanieInVR` sees the owner-only `StefanieInVR Stream` selection button.
- A VIP non-owner does not see that owner-only selection button.
- Ordinary users cannot enter the VIP panel and therefore cannot reach the owner-only button there.
- Playback selected by Stef synchronizes to VIPs and ordinary users.
- Other permitted users can select/start another playlist through their own visible custom buttons.
- After removing the old `VideoSourceUI/Content` hiding logic, the previously intermittent wrong/blank playlist-UI behaviour could no longer be reproduced during the full retest.

Evidence limit:

The disappearance of the UI problem is strong evidence that the old Content-hiding interaction was involved, but the exact internal timing/root cause was not instrumented. Do not invent a more specific cause later.

---

## 3. Source Manager and native selector facts

Open Classroom currently uses VideoTXL 2.5.1 with a `SourceManager` containing Playlist sources.

Verified during read-only Unity inspection:

- the Playlist/source GameObjects may all remain active;
- playlist access is not implemented by enabling/disabling VideoTXL source objects;
- `SyncPlayer.accessControl == null` in the inspected Classroom setup;
- no scene `Texel.AccessControl` instance was found connected as per-playlist access control;
- VideoTXL's own automatically generated source-selection footer is hidden;
- therefore the custom tablet playlist buttons are the intended user-facing discovery/selection route.

Do not add CommonTXL/VideoTXL `AccessControl` merely to reproduce button visibility. Only reconsider it if a future requirement genuinely needs source-level control.

---

## 4. VideoSourceUI behaviour learned from VideoTXL 2.5.1

A synchronized remote source change updates playback/current source on receiving clients, but `VideoSourceUI` does not automatically force every remote user's locally selected content panel to mirror the owner's panel selection.

Useful consequence:

```text
remote user changes active source
→ playback can synchronize
→ each user's VideoSourceUI presentation can remain substantially local
```

`VideoSourceUI._SelectActive()` is a local operation that reads the currently active/current source when it is called and selects the matching panel.

This is one reason the project now keeps access rules at the custom selection-button layer instead of trying to hide/show VideoSourceUI content dynamically.

---

## 5. TXLPlayPlaylistButton route

The custom Classroom playlist buttons use the bridge:

```text
custom Unity Button
→ SendCustomEvent("PlayAndShow")
→ TXLPlayPlaylistButton.PlayAndShow()
→ playlist._MoveTo(0)
→ videoSourceUI._SelectActive()
```

The active scene was inspected and the relevant button calls use `PlayAndShow()`.

Do not revert to an old `PlaylistLoadData._Load()` path for Playlist sources that already exist as their own Source Manager Playlist unless a future VideoTXL change makes that necessary.

---

## 6. PlaylistData JSON box — what it really is

The `Import from JSON` box visible in the VideoTXL `PlaylistData` Inspector is an **Editor importer**, not a runtime web loader.

VideoTXL 2.5.1 editor code accepts JSON in this shape:

```json
{
  "title": "Playlist Name",
  "entries": [
    {
      "title": "Track Name",
      "url": "https://example.com/video.mp4",
      "questUrl": "https://example.com/video-quest.mp4"
    }
  ]
}
```

Accepted entry fields in the inspected editor code:

- `title`
- `url`
- `questUrl`
- legacy/alternate `urlForQuest`

Pressing `Import` in Unity parses the pasted JSON and writes the data into the serialized `PlaylistData` component/asset.

Important:

```text
Inspector JSON import
≠ download JSON from the web at runtime
```

So putting a JSON file on `stefanieinvr.com` does not automatically make a stock VideoTXL PlaylistData follow it live.

---

## 7. Runtime VideoTXL building blocks that make online playlists plausible

Verified from VideoTXL 2.5.1 source:

`PlaylistData` exposes runtime data fields including:

```text
playlistName
VRCUrl[] playlist
VRCUrl[] questPlaylist
string[] trackNames
```

`Playlist` holds a `PlaylistData` reference and has runtime loading logic including:

```text
_LoadData(PlaylistData data)
```

Therefore VideoTXL already has the internal building blocks to use playlist data that is prepared at runtime.

What VideoTXL's Inspector does **not** provide is the web-fetch layer.

---

## 8. Future online playlist architecture — technically plausible, NOT implemented yet

Desired future model:

```text
StefanieInVR web/content manager
→ writes playlist JSON
→ JSON is hosted online
→ VRChat world downloads JSON at runtime
→ small UdonSharp adapter parses it
→ adapter builds/updates PlaylistData-compatible values
→ VideoTXL Playlist loads the data
```

Possible VRChat-side ingredients to investigate when this becomes active work:

- `VRCStringDownloader` for downloading text/JSON;
- `VRCJson` or another Udon-compatible parsing route;
- a small adapter between parsed web data and VideoTXL `PlaylistData` / `Playlist._LoadData(...)`.

This is a future engineering direction, not a proven implementation.

Before building it, explicitly verify current VRChat requirements for:

- trusted URL/domain rules;
- whether users need `Allow Untrusted URLs` for the chosen host;
- CORS/server headers and HTTPS behaviour;
- Quest compatibility;
- refresh/reload behaviour;
- multiplayer ownership/synchronization;
- late joiners;
- failure/fallback behaviour when JSON is unavailable or malformed.

Do not assume `stefanieinvr.com` is automatically accepted by VRChat string loading without re-checking the current official documentation at implementation time.

---

## 9. Recommended future JSON/content model

A central StefanieInVR Content Manager may eventually maintain different catalogs for different worlds:

```text
StefanieInVR Content Manager
├─ Open Classroom
└─ Stefanie's Art House Cinema
```

Useful web-managed fields may include:

```text
playlist id
title
description
image URL
category
order
enabled
accessLevel: public | vip | owner
tracks[]
  title
  media URL
  Quest URL (optional)
```

Important architecture rule learned from the Classroom:

`accessLevel` should normally control **which custom selection button/catalog entry is shown locally**, not whether VideoTXL is allowed to play or display an already-active source.

That keeps content management and player playback responsibilities separate.

---

## 10. Safest future proof-of-concept

When online playlists become active work, do not build the full Content Manager first.

Smallest recommended proof:

```text
ONE Playlist
→ ONE hosted JSON file
→ ONE runtime loader
→ ONE VideoTXL Playlist
```

Test in order:

1. PC/editor-safe development proof.
2. Uploaded VRChat PC client.
3. second user multiplayer behaviour.
4. late join.
5. Quest.
6. malformed/missing JSON fallback.
7. only then generalize to multiple playlists/world catalogs/admin UI.

---

## 11. Reuse guidance for Stefanie's Art House Cinema

These findings are relevant to the Cinema because its future film/documentary catalog can benefit from content that changes without rebuilding the Unity world.

But do not blindly copy the Classroom implementation.

Before Cinema implementation:

```text
verify Cinema's exact VideoTXL version and scene wiring
→ inspect its actual SourceManager/player structure
→ preserve Cinema's own global/local/admin semantics
→ reuse the proven responsibility split
→ test in Cinema
```

The transferable principle is:

```text
web/catalog determines available metadata + custom selection entries
custom UI determines local discovery/access
VideoTXL determines playback + synchronization
```

---

## 12. Facts to remember in future chats

```text
1. VideoTXL PlaylistData JSON Inspector is EDITOR IMPORT, not live web JSON.
2. Runtime online playlists are plausible but need a custom Udon web-loader adapter.
3. PlaylistData exposes playlistName/URLs/Quest URLs/track names.
4. Playlist has runtime _LoadData(PlaylistData).
5. Open Classroom privacy/access is BUTTON VISIBILITY ONLY.
6. Do not hide VideoSourceUI/Content for the current access requirement.
7. Playback may synchronize to everyone even when only some users can select the source.
8. Native VideoTXL source selector is hidden in the Classroom; custom tablet buttons are the navigation route.
9. Keep facts vs hypotheses separate; re-verify VRChat web-domain rules before implementation.
10. Online catalogs are a strong future shared direction for Classroom + Cinema, but are not implemented yet.
```
