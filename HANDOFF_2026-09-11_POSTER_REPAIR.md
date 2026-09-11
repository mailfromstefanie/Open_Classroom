# Handoff — 2026-09-11 — Current Scene Repair / Poster Gate

## Status

Stef has deliberately changed the immediate recovery strategy.

The 2026-09-10 decision to restore the older backup remains valid history and a safe fallback, but it is **not the current next action**.

Current decision:
- keep the current Unity scene for now;
- repair it cautiously instead of immediately rolling back;
- one bounded change at a time;
- Stef visually/functionally checks every UI change;
- Codex must not autonomously redesign large Canvas/UI areas;
- protected systems remain untouched unless explicitly requested.

Real Unity project:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

## Current user-observed truth

### Entrance Text

Functionally, the welcome/entrance text is working well enough for now.

Current problem is mainly visual:
- its editor UI does not match the established Classroom tablet style;
- typography/material/scale/layout differ from existing tablet UI.

Do **not** reopen Entrance Text logic now unless new functional evidence appears.
UI restyling is parked until the poster technical gate closes.

### Presentation / PowerPoint UI

Stef also sees visual inconsistency here, but this is explicitly **not a current priority**.
Do not modify Presentation or VideoTXL while repairing the poster.

## Read-only investigation findings — 2026-09-11

Codex investigated the saved scene without initially changing files/scripts/scene objects.

Important findings:

1. `Panel (Content)` / poster/entrance UI uses inconsistent styling compared with known-good tablet controls.
2. New poster/content text includes very small font sizes, dark text and mixed TMP font/material presets; the Entrance editor also has a reduced parent scale.
3. Several decorative TMP/Image elements unnecessarily have raycast enabled, but the fundamental Canvas/GraphicRaycaster/EventSystem path appears intact.
4. The generated poster is not Stef's intended existing plane. Current generated hierarchy uses a new pickup/root plus Cube/Quad geometry.
5. The poster UI accidentally referenced the Entrance editor's `Button (Claim Start)` as its own claim button.
6. Stef manually cleared `PersistentPosterEditorUI.claimButton` back to `None`. Entrance Apply/Save had already been clickable, so this cross-reference was real but was not the main user-visible poster failure.

## Poster architecture currently present

Historical/current generated implementation uses:

```text
PersistentPosterEditorUI
-> VRCUrlInputField draft
-> scale preview / Apply

PersistentPosterManager
-> synced URL/scale state + PlayerData pieces

PersistentPoster
-> VRCImageDownloader
-> renderer/material assignment
-> VRCObjectSync / VRCPickup for generated movable poster
```

Generated physical poster uses new Cube/Quad objects. This is **not** Stef's desired final physical design.

Desired final direction after functionality is proven:
- Stef selects the exact existing Plane/GameObject in Unity;
- retain that plane's transform/mesh/look where practical;
- connect the working image-loading path to that renderer/material;
- do not generate a replacement Cube/Quad/frame unless Stef explicitly asks;
- use existing Classroom/local-table-screen technique and existing tablet style as reference rather than inventing a parallel visual system.

## Concrete runtime failure discovered

A previous real VRChat log showed the poster image request reaching `VRCImageDownloader`, but the URL:

`https://imgur.com/oaEbiM2.png`

failed with:

`Redirect limit exceeded`

This is important positive evidence:
- the request reached the downloader;
- the immediate failure was the URL/redirect path, not proof that the whole poster chain is broken.

Use a direct final image URL for the next runtime test, for example:

`https://i.imgur.com/oaEbiM2.png`

The system must remain **generic**. Do not add Imgur-specific logic. The intended product must accept any valid direct HTTPS image URL supported by VRChat's URL/image-loading rules, including suitable GitHub-hosted/GitHub Pages URLs or Stef's own website where allowed.

## Interrupted Codex work

Codex reported that it was limiting its next scene edits to two narrow settings:
- change the serialized/default test URL from the redirecting `imgur.com` URL to the direct `i.imgur.com` URL;
- improve the existing poster URL-input readability/contrast enough to make the runtime test possible.

Codex then ran out of credits while it was updating GitHub documentation.

GitHub showed no new Open Classroom commit after 2026-09-10 at the time Nova checked.

Therefore:
- do **not** assume the documentation update completed;
- do **not** assume the two local scene edits completed merely because Codex described them;
- first inspect the real Unity scene and verify the current serialized URL/input settings before changing them again.

## Current gate — POSTER FUNCTIONALITY FIRST

Do not spend the next session beautifying the whole Content UI.
Do not replace the poster mesh yet.

First prove this minimal real-VRChat chain:

```text
URL field is readable enough to use
-> paste/type a direct HTTPS image URL
-> Load / Apply is available when authorized
-> request reaches VRCImageDownloader
-> image load succeeds
-> texture appears on the CURRENT TEMPORARY poster surface
```

Test with a known direct URL such as:

`https://i.imgur.com/oaEbiM2.png`

Use a real VRChat Build & Run / real client for acceptance, not compile success alone.

If it fails:
- inspect the actual VRCImageDownloader callback/error from the real log;
- repair only the failing link in the chain;
- do not simultaneously redesign UI or geometry.

If it passes:
1. record URL -> Load/Apply -> visible image as accepted technical evidence;
2. then restyle Content/Entrance poster controls to match existing tablet UI by reusing proven button/TMP/material/spacing patterns;
3. then replace/adapt the generated physical poster to Stef's specifically selected existing plane;
4. only after that continue multiplayer/late-join/persistence acceptance.

## UI styling rule after the technical gate

The existing Classroom tablet is the visual source of truth.

Prefer duplicating/copying visual properties from known-good existing tablet elements rather than creating new styling.
Use existing:
- TMP font/material presets;
- button sprites;
- transitions/hover/pressed states;
- colors;
- padding/dimensions;
- input-field technique;
- layer/raycast conventions.

Visual acceptance belongs to Stef, not to compile/Play Mode.

## Protected systems

Do not modify during this poster repair unless Stef explicitly redirects the task:
- Presentation Core / Presentation integration;
- VideoTXL 2.5.1;
- e-reader/library;
- Marker Pro reset;
- local table screens;
- unrelated tablet panels/navigation;
- working Entrance Text logic.

## Exact next action

1. Open the current real Unity scene.
2. Verify whether Codex's interrupted local edits are actually present:
   - poster default/test URL is direct `https://i.imgur.com/oaEbiM2.png`;
   - URL input text is visibly readable enough for a test.
3. Make no broad UI changes.
4. Run a real VRChat test of the direct URL.
5. Record exactly whether the image appears and, if not, the real downloader error/status.
6. STOP and choose the next smallest repair from that evidence.
