# Persistent Synchronized Movable Poster — implementation 2026-09-10

## Status

Implemented, Unity/UdonSharp compiled, scene-wired and editor-smoke-tested in:

`E:/Projects/Open_Classroom/#Unity/Open_Classroom`

This is not yet a real VRChat multiplayer PASS. The acceptance boundary is listed below.

## Runtime architecture

```text
PersistentPosterEditorUI
-> local VRCUrlInputField draft
-> local scale preview
-> Apply or slider PointerUp

PersistentPosterManager
-> PlayerData: URL string history, position, rotation, scale
-> synced instance state: VRCUrl, scale, initialized flag

PersistentPoster
-> VRCImageDownloader loads locally on every client
-> VRCObjectSync carries position and rotation
-> VRCPickup transfers temporary object ownership for movement
```

The synchronized running-instance state remains separate from every player's personal PlayerData. A player leaving does not clear the current synchronized URL/scale or the VRCObjectSync transform.

## Files and scene objects

Runtime scripts:

- `Assets/!StefanieInVR/Scripts/Managers/PersistentPoster.cs`
- `Assets/!StefanieInVR/Scripts/Managers/PersistentPosterManager.cs`
- `Assets/!StefanieInVR/Scripts/Managers/PersistentPosterEditorUI.cs`

Supporting assets:

- matching UdonSharp program assets and generated serialized Udon programs;
- `Assets/!StefanieInVR/PersistentPosterImage.mat`;
- `Assets/!StefanieInVR/PersistentPosterFrame.mat`;
- `Assets/!StefanieInVR/PersistentPosterPlaceholder.mat`;
- guarded editor setup tool `Assets/!StefanieInVR/Scripts/Editor/PersistentPosterSceneSetupTool.cs`.

Scene changes in `Assets/#Classroom/Scenes/Classroom.unity`:

- `UIs/Other Toggles and Systems/Persistent Poster Home`;
- `UIs/Other Toggles and Systems/Persistent Poster`;
- `UIs/Managers/Persistent Poster Manager`;
- `.../===Panels and Tabs===/Panels/Panel (Content)` at tab index `8`;
- `Tab (Edit)` wired to index `8` with the supplied Edit On/Off/Highlight sprites;
- the existing Entrance Text editor moved into the upper portion of `Panel (Content)`.

The Content tab is now fully connected. Play Mode confirmed that pressing `Tab (Edit)` selects index `8`, activates only `Panel (Content)`, makes both editors visible and changes the Edit sprite to `EditTabOn` / `EditTabOnHigh`.

Follow-up UI correction: the cloned VideoTXL URL field retained pixel-sized mask offsets that collapsed inside the smaller Content layout. Its mask/text RectTransforms were refitted to the `19 x 2` field. Play Mode now confirms that the URL input is visible-sized, active, interactable and can receive focus; both poster buttons are interactable when unlocked.

## Image behaviour

- User input must come through `VRCUrlInputField`.
- Load/Apply publishes the secure `VRCUrl` to the current instance.
- Every client calls `VRCImageDownloader` locally for that same URL.
- Mipmaps, trilinear filtering, clamp wrapping and anisotropic filtering are enabled.
- Source texture width/height changes an aspect-ratio root; the user scale remains one uniform multiplier.
- Empty URL clears the downloaded image and shows the placeholder.
- A failed replacement keeps the last successfully loaded image where possible and reports a local status error.

VRChat still requires users to allow untrusted URLs when the image domain is not trusted. Redirect-based links may fail; use a direct final image URL.

## Persistence boundary

PlayerData supports string, Vector3, Quaternion and float storage, so these are saved:

- URL as string history;
- position;
- rotation;
- uniform scale.

Udon currently exposes neither construction of `VRCUrl` from a restored string nor arbitrary runtime assignment to the `VRCUrlInputField` text. Consequently:

- URL synchronization and late-join delivery work by synchronized `VRCUrl` for the life of the current instance;
- a future new instance cannot automatically rebuild that secure URL from PlayerData;
- the host must paste the direct URL once again;
- pose and scale can be restored normally.

This is a platform/API boundary, not a new persistence framework problem. Do not add a proxy service to work around it.

## Authorization and ownership

Existing `VipAccessManager` is reused without modification:

- unlocked tablet: all users may edit, resize and pick up the poster;
- locked tablet: only a verified VIP may do so.

VRC object ownership is obtained when required for physical movement or serialization. Ownership alone does not bypass the Classroom lock rule.

## Scale and reset

Scale range: `0.4x` through `2.0x`.

- Slider OnValueChanged: local preview only.
- Slider PointerUp: publish request.
- Publish interval: at least one second.
- During cooldown, the latest released value replaces earlier queued values.

Reset is deliberate:

```text
Reset Draft
-> empty/default URL + 1.0x + default placement draft
-> no personal/shared mutation yet
-> Load / Apply commits all three
```

## Validation performed

Implemented/compiled:

- Unity reports no C# compilation errors;
- `PersistentPoster`, `PersistentPosterManager` and `PersistentPosterEditorUI` UdonSharp assets report Current Version;
- all three scene proxies have backing UdonBehaviours and serialized Udon program assets;
- URL input, Load/Apply, Reset, slider OnValueChanged and PointerUp listeners are wired;
- poster has kinematic Rigidbody, VRCPickup, VRCObjectSync and its own outline renderer;
- scene saves cleanly.

Editor Play Mode smoke:

- PlayerData restoration callback completed;
- instance initialized with empty URL placeholder;
- poster was pickupable while the tablet was unlocked;
- local scale preview did not change the synchronized value;
- release published the scale;
- one-second queue published the latest of multiple releases;
- Reset stayed draft-only until Apply;
- no runtime error appeared during this smoke test.

## Real VRChat acceptance still required

1. Enter a direct HTTPS image URL and confirm successful display on PC and Quest if supported.
2. Try an invalid/non-image URL and confirm the poster remains stable and status is useful.
3. With two clients, confirm URL/image and scale agreement.
4. Pick up, move, rotate and drop; confirm the remote client sees the final pose.
5. Join late and confirm URL/image, scale and VRCObjectSync pose.
6. Let the original publisher leave; confirm the remaining instance retains the poster state.
7. Tablet unlocked: confirm a non-VIP may edit and move.
8. VIP locks the tablet: confirm non-VIP controls/pickup disable and VIP remains authorized.
9. Confirm pose/rotation/scale restoration in a later session.
10. Confirm the host can repaste the stored direct URL for a new instance; do not expect automatic URL restoration.

## Protected systems

No intentional changes were made to:

- Entrance Text V1.1;
- VideoTXL;
- Presentation;
- e-reader architecture;
- local table screens;
- Marker Pro reset behaviour;
- existing tablet tab arrays/buttons.
