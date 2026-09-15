# EReader Mobile Focus View Research

**Project:** StefanieInVR — Open_Classroom  
**Target:** Book_A only  
**Document type:** Technical research / design handoff  
**Status:** RESEARCH COMPLETE — NOT IMPLEMENTED — NO IMPLEMENTATION GO  
**Date:** 2026-09-15 / 2026-09-16 (Europe/Amsterdam transition)  
**Primary purpose:** Give the next Codex/Astra session a complete, evidence-based basis for deciding how to improve Book_A on VRChat Mobile without disturbing the proven PC and VR implementations.

---

## Handoff clarification — 2026-09-16

This report is research, not a device-test result. Current implementation and reported PC/VR/mobile evidence are maintained in [EREADER_BOOK_A_WORK.md](EREADER_BOOK_A_WORK.md).

The receiving implementation session proposes a smaller first M0 than the RT/NEXT example below: **static test page + counter button**. Current Book_A activation also opens the custom camera-follow view, so isolate native Focus View first; connect real playback/navigation later. This is a proposed refinement, not a built prototype or implementation GO.

`CameraMode == FocusView` reports the client's camera mode; it does **not** identify which canvas is focused. Any later Book_A integration must account for focus on other world UI.

The accepted V1 final-drop-stays-open requirement is still a known gap in current Pin-dependent VR code. The broad VR baseline PASS below must not be read as proof of that unimplemented rule.

This report has been published as part of documentation synchronization. No Unity code/scene change, native prototype, platform switch or device acceptance is implied.

## 0. READ THIS FIRST — HARD BOUNDARIES

This document is **research only**.

Do **not** treat this file as permission to change:

- scripts;
- scenes;
- prefabs;
- GameObjects;
- Canvas settings;
- VRC_UIShape settings;
- platform settings;
- Book_B;
- the proven VR handle system;
- the proven PC focus-reading route;
- networking/ObjectSync;
- playback manager;
- Presentation Service;
- bookmarks/library systems.

A separate explicit GO from Stef is required before implementation.

Before implementing anything, also read:

- `EREADER_BOOK_A_WORK.md`
- the current project scene/wiring relevant to Book_A

The live project state always wins over stale notes.

---

# 1. EXECUTIVE CONCLUSION

Native **VRChat Mobile Focus View** is currently the strongest supported candidate for the mobile EReader reading experience because it directly addresses two problems in the present custom mobile route:

1. the avatar moves while the user tries to swipe the page;
2. too much screen area is being sacrificed to avoid VRChat's mobile HUD.

Official VRChat documentation confirms that while Focus View is active:

- the user's avatar stays in place;
- the user can pan and zoom the world-space UI;
- both portrait and landscape are supported;
- Focus View is available only on phone/tablet under specific conditions;
- the user remains in Focus View until they manually close it;
- interacting with selectable/scroll UI prevents Focus View pan/zoom during that interaction.

However, the currently documented Udon API does **not** expose a confirmed supported method to programmatically enter or exit native Focus View.

Udon **can detect** whether the mobile screen camera is currently in `FocusView` via:

`VRCCameraSettings.ScreenCamera.CameraMode`

Therefore the safest architecture is:

- preserve the current proven VR route unchanged;
- preserve the current proven PC route unchanged;
- create a separate native mobile Focus View route;
- let VRChat own the native Focus View enter/exit behavior;
- use Udon to detect Focus View state and coordinate EReader state around it;
- do not rely on disabling/destroying the focused canvas as a substitute for a supported exit API;
- do not assume that "native zoom" specifically means pinch-to-zoom until it is verified on a real device;
- avoid putting the whole reading surface inside a `ScrollRect` if native Focus View pan/zoom is desired.

Before full implementation, run a very small real-phone proof-of-concept.

---

# 2. CURRENT PROVEN PROJECT BASELINE

This section summarizes the current Book_A state from `EREADER_BOOK_A_WORK.md`.

## Proven / accepted

### VR

- Book_A has two separate VR handles.
- Constraint/pickup/drop/reset behavior has been implemented.
- Stef tested the two-handle version in VRChat.
- Result: **PASSED / proven baseline**.

This baseline must not be casually modified.

### PC / non-VR desktop

The current custom focus-reading route is reported by Stef as working well on PC.

Current PC route includes:

- local WorldSpace reading panel;
- same RenderTexture as the physical reader;
- same playback manager;
- same page progress;
- same page navigation;
- zoom controls;
- page-width / whole-page modes;
- ScrollRect pan;
- mouse-wheel zoom;
- `Tab` + mouse interaction;
- separate physical **LEZEN** and **VERPLAATSEN** behavior;
- **X** closes while preserving physical position and reading progress;
- **TERUGZETTEN** returns the physical reader to Home.

Result: **PC baseline accepted by Stef**.

This route should remain intact unless a later explicit task requires otherwise.

## Mobile current state

Current custom mobile mode works only partially.

Observed by Stef:

- the page is visible;
- controls function to some extent;
- swiping/dragging the reading area can also move/control the avatar;
- the reading buttons sit too high;
- a large amount of page space is unused because a large lower HUD margin was reserved;
- two-finger pinch zoom is missing.

This is the mobile problem this research addresses.

---

# 3. CURRENT CUSTOM MOBILE ARCHITECTURE

At the time of this research, the mobile route is based on the same custom focus system used for PC.

Relevant characteristics:

- local WorldSpace canvas;
- panel follows `VRCCameraSettings.ScreenCamera`;
- RawImage displays `RT_EReader`;
- ScrollRect provides page panning;
- custom zoom buttons;
- navigation buttons;
- custom mobile layout;
- approximately 32% lower-space reservation was added as a workaround for the VRChat mobile HUD;
- `VRC_UIShape.AllowFocusView = false` on the current custom focus panel;
- therefore this current route is **not native VRChat Focus View**.

Important distinction:

> `EReaderFocusView` is the project's own class/name.  
> VRChat's **Focus View** is a native mobile client feature.  
> They are not the same system.

---

# 4. RESEARCH QUESTIONS — STATUS TABLE

The research report retains its original section numbering (1, 3–8). The exact native entry/exit gesture is still a device-test question, not omitted proof.

| Question | Status | Result |
|---|---|---|
| **1. What happens to joystick, jump, mic/PTT and menu buttons? Distinguish avatar movement from camera movement.** | **PARTLY CONFIRMED** | Officially confirmed: avatar stays in place. Native Focus View provides pan/zoom over the focused canvas, so normal world navigation is replaced by Focus View interaction while active. Exact visibility/availability of joystick, jump, PTT/mic and menu buttons is not fully specified in current Focus View docs and must be checked on a real phone. |
| **3. Can Udon/UdonSharp open and close Focus View programmatically? Distinguish allow/detect/activate.** | **ALLOW = CONFIRMED; DETECT = CONFIRMED; PROGRAMMATIC ENTER/EXIT = NOT DOCUMENTED / NOT SUPPORTED BY FOUND API** | `VRC_UIShape.Allow Focus View` allows it. `VRCCameraSettings.ScreenCamera.CameraMode` can report `FocusView`. No documented Udon `EnterFocusView`, `ExitFocusView`, `SetFocusView`, etc. was found. Official docs say users stay in Focus View until manually closing it. |
| **4. What if X disables the canvas while Focus View is active?** | **UNSAFE / NOT DOCUMENTED AS A SUPPORTED EXIT** | No official documentation says disabling the focused canvas is a valid way to exit. A VRChat iOS bug report describes a soft-lock after the focused canvas was disabled/deleted. Even if that specific bug is fixed in a future/current build, canvas disable must not be treated as a supported exit API without real-device verification. |
| **5. Does Focus View support two-finger pinch and panning? What conflicts with ScrollRect/buttons?** | **PAN/ZOOM CONFIRMED; PINCH GESTURE SPECIFICALLY = UNCONFIRMED** | VRChat explicitly documents pan and zoom. Current docs do not explicitly promise that zoom is a two-finger pinch gesture. VRChat explicitly says Focus View pan/zoom is unavailable while interacting with a selectable object or scroll element. A full-page ScrollRect therefore conflicts with native pan/zoom. |
| **6. What Canvas setup is required? Can it keep following the camera?** | **CORE SETUP CONFIRMED; CAMERA-FOLLOW NOT RECOMMENDED FOR THIS DESIGN** | Focus View is designed around a correctly configured World Space Canvas with `VRC_UIShape`, correct layer and valid UI setup. A camera-following panel is unnecessary for native Focus View and adds risk. A reported ScreenCamera/Focus View bug has caused Position/Rotation to return world origin. Mobile native canvas should preferably remain attached to Book_A/world space. |
| **7. Android/iOS, portrait/landscape, touch/controller, settings, ClientSim?** | **MOSTLY CONFIRMED** | Current VRC_UIShape docs say phone/tablet. iOS creator documentation says Android/Quest guidelines also apply to iOS. Portrait + landscape are explicitly supported. Touch must be the only active input method; changing away from touch exits Focus View. User and world settings can disable Focus View. ClientSim cannot simulate all VRChat features and is not sufficient proof for Focus View/mobile acceptance. |
| **8. Is native Focus View suitable? Alternatives?** | **RECOMMENDED FOR A SMALL MOBILE PROTOTYPE** | Native Focus View is well aligned with reading: avatar stays still, world UI can fill more of the camera, native pan/zoom exists, portrait/landscape are supported. Main limitation: no documented creator API for direct enter/exit. A custom screen-space route remains possible but does not automatically solve locomotion/HUD conflicts. |

---

# 5. SOURCE CLASSIFICATION

Do not treat all sources as equally authoritative.

## A. Current official creator documentation — highest authority

### VRC UI Shape / Focus View
https://creators.vrchat.com/worlds/components/vrc_uishape/

Use for:

- required canvas setup;
- `Allow Focus View`;
- user/world requirements;
- activation/deactivation distance conditions;
- touch-only condition;
- manual close behavior;
- phone/tablet applicability.

### VRCCameraSettings
https://creators.vrchat.com/worlds/udon/vrc-graphics/vrc-camera-settings/

Use for:

- `VRCCameraSettings.ScreenCamera`;
- `CameraMode`;
- `Screen`;
- `FocusView`;
- camera data limitations and available properties.

### Mobile Best Practices
https://creators.vrchat.com/platforms/android/android-best-practices/

Use for:

- `OnInputMethodChanged`;
- `InputManager.GetLastUsedInputMethod()`;
- touchscreen behavior;
- real-device testing recommendation;
- mobile UI guidance;
- screen orientation APIs/guidance.

### iOS creator documentation
https://creators.vrchat.com/platforms/iOS/

Use for:

- confirmation that Android/Quest-oriented mobile guidance generally applies to iOS as well.

### ClientSim
https://creators.vrchat.com/worlds/clientsim/

Use for:

- ClientSim limitations;
- requirement to validate final behavior in real VRChat.

### Screen Canvas example
https://creators.vrchat.com/worlds/examples/screen-canvas/

Use for:

- supported 2D screen-space UI pattern;
- Desktop Tab interaction;
- mobile tap interaction;
- VRCButtonLayout helper for visualizing mobile HUD areas.

---

## B. Official historical release notes — useful behavior documentation

### VRChat 2024.2.1
https://docs.vrchat.com/docs/202421

Focus View introduction documented:

- mobile Focus View fills camera view with world-space canvas;
- avatar stays in place;
- pan and zoom;
- landscape + portrait;
- keyboard inputs still accessible;
- Focus View setting;
- touch-only entry;
- exit on input-method change away from touch;
- local and remote avatars not rendered;
- no native pan/zoom while interacting with selectable/scroll UI;
- distance-based activation/deactivation.

Important:

This is older release documentation. Use current creator docs first where they overlap.

---

## C. VRChat Canny / feedback reports — evidence of observed bugs, NOT API contracts

These reports are useful as warnings and test targets. They are not guaranteed to describe the current live client forever.

### ScreenCamera position/rotation during Focus View
https://feedback.vrchat.com/udon/p/vrccamerasettingsscreencamera-postion-rotation-returns-world-origin-if-focusing

Reported behavior:

- during mobile Focus View, `VRCCameraSettings.ScreenCamera.Position/Rotation` reportedly returned world origin;
- this can break world systems that attach HUD-like objects to ScreenCamera.

Design implication:

- do not make native mobile Focus View dependent on continuous ScreenCamera-following unless tested.

### iOS Focus View soft-lock after focused canvas disappears
https://feedback.vrchat.com/ios-mobile-beta/p/ios-focus-menu-soft-lock

Reported behavior:

- user enters Focus View;
- focused canvas is deleted/disabled;
- user can become stuck in Focus View.

Status seen during research:

- report was marked/tracked and indicated availability in a future release at one point.

Design implication:

- even if fixed in current/future client, do not use canvas destruction/deactivation as the intended exit mechanism unless VRChat documents it as supported.

### Focus View blocks player teleportation
https://feedback.vrchat.com/udon/p/focus-view-blocks-player-teleportation

Reported behavior:

- world teleport action does not move the player while Focus View is active;
- player remains until manually leaving Focus View.

Design implication:

- supports the conclusion that Focus View intentionally owns a strong client-side "stay here" state;
- also demonstrates why creators need to avoid assuming they can force client state changes while Focus View is active.

### Focus View canvas pivot problem
https://feedback.vrchat.com/mobile-beta/p/focus-view-problems-on-some-canvases

Reported behavior:

- panning bounds can behave incorrectly when canvas pivot is not centered.

Design implication:

- use a centered, conventional RectTransform/pivot for the mobile native focus canvas;
- test pan bounds on the real device.

---

# 6. QUESTION 1 — MOVEMENT, CAMERA AND VRCHAT MOBILE HUD

## Officially confirmed

VRChat's 2024.2.1 Focus View release notes explicitly state:

- the user's avatar remains in place while Focus View is active;
- the user can pan and zoom around the focused UI;
- pan should stop at the canvas bounds;
- local and remote avatars are not rendered while in Focus View.

This directly addresses the current Book_A issue where dragging the page also causes avatar movement.

## Avatar movement vs camera movement

These are different:

### Avatar locomotion

Confirmed:

- avatar stays in place.

Therefore Focus View is much better aligned with a "reading mode" than the current custom swipe-over-world approach.

### Camera / reading view movement

Confirmed:

- Focus View itself provides pan and zoom over the focused world-space UI.

Do not interpret "avatar stays still" as "nothing can move on screen." The user still navigates the focused canvas through Focus View's own view controls.

## Joystick, jump, microphone/PTT and menus

Current official Focus View pages do **not** provide a complete item-by-item HUD visibility table.

Therefore the following are **not proven from documentation**:

- whether the movement joystick visually disappears;
- whether the camera/look joystick visually disappears;
- whether jump remains visible;
- exact mic/PTT placement;
- exact location of menu buttons;
- whether every visible HUD control remains interactive;
- whether layouts differ between Android and iOS or between phone models/aspect ratios.

### Required real-device test

On Stef's phone, while native Focus View is active, record:

| Control | Visible? | Usable? | Position |
|---|---:|---:|---|
| Movement joystick | | | |
| Camera/look control | | | |
| Jump | | | |
| Mic/PTT | | | |
| Main/Quick Menu entry | | | |
| Native Focus View exit | | | |

This should become evidence in `EREADER_BOOK_A_WORK.md` after the test.

---

# 7. QUESTION 3 — ALLOW, DETECT, ENTER AND EXIT

This distinction is critical.

## 7.1 Allow Focus View — SUPPORTED

`VRC_UIShape` exposes:

- `Allow Focus View`

This determines whether an eligible phone/tablet user can enter Focus View for that canvas.

Additional requirements include:

- valid Canvas setup;
- VRC_UIShape;
- world Focus View not disabled;
- user Focus View not disabled;
- touchscreen-only input;
- user within the required distance window.

## 7.2 Detect Focus View — SUPPORTED

Current VRChat camera API exposes:

`VRCCameraSettings.ScreenCamera.CameraMode`

Documented ScreenCamera values include:

- `Screen`
- `FocusView`

Therefore an Udon/UdonSharp behavior can inspect whether the user is currently in native Focus View.

This is useful for:

- detecting entry;
- detecting return to normal Screen mode;
- changing local EReader state;
- avoiding unsafe cleanup while native Focus View is still active;
- making Book_A respond after the user manually exits.

### Caution

Do not assume `OnVRCCameraSettingsChanged` is guaranteed to fire exactly when `CameraMode` changes unless separately documented/tested.

A robust implementation may need to poll `CameraMode` lightly while the mobile reading state is relevant.

That is an implementation recommendation, not an official VRChat requirement.

## 7.3 Programmatically enter Focus View — NO SUPPORTED API FOUND

No currently documented method was found such as:

- `EnterFocusView()`
- `OpenFocusView()`
- `SetFocusView(true)`
- `CameraMode = FocusView`

`CameraMode` is documented as state information, not as a setter for entering Focus View.

Do not implement guessed/internal APIs.

## 7.4 Programmatically exit Focus View — NO SUPPORTED API FOUND

No documented method was found such as:

- `ExitFocusView()`
- `CloseFocusView()`
- `SetFocusView(false)`

Current VRC_UIShape documentation says users remain in Focus View until they manually close it.

The historical release note also says Focus View exits if the active input method changes away from touch, but deliberately spoofing/changing input method is not a creator-controlled exit API.

## Product implication

Do not make "LEZEN must directly force native Focus View open" a hard requirement unless VRChat exposes a supported API in a future SDK.

Instead design around:

1. Book_A prepares the eligible mobile canvas;
2. user enters Focus View through VRChat's native interaction;
3. Udon detects `CameraMode == FocusView`;
4. user later uses VRChat's native exit;
5. Udon detects return to `Screen`;
6. Book_A safely updates its local reading state.

---

# 8. QUESTION 4 — WHAT SHOULD X DO?

## Do not use `SetActive(false)` as the native Focus View exit command

No current official creator documentation says:

> disabling the focused canvas causes a clean supported Focus View exit.

A Canny report documented a soft-lock on iOS when a focused canvas disappeared.

Even if that exact bug has been fixed in a newer client, the design should not depend on a behavior VRChat does not document as its exit contract.

## Safe design principle

While:

`VRCCameraSettings.ScreenCamera.CameraMode == FocusView`

avoid:

- destroying the Focus View canvas;
- disabling its root immediately;
- moving it to an unrelated position;
- moving it to world origin;
- switching to the PC ScreenCamera-follow system;
- resetting Book_A in a way that removes the focus target.

## Recommended X behavior for prototype

Option A — safest first prototype:

- do not put a project X inside native Focus View yet;
- user closes using VRChat's native Focus View exit;
- detect return to `Screen`;
- then run existing local Book_A close/release behavior.

Option B — later UX refinement:

- Book_A's X can request "reader wants to close";
- set a local pending-close flag;
- do not destroy/disable the focus target while still in `FocusView`;
- instruct user to use native exit or wait for `CameraMode == Screen`;
- once Screen mode is detected, finish close safely.

Whether a better X workflow is possible should be decided only after the phone prototype.

---

# 9. QUESTION 5 — NATIVE PAN/ZOOM, PINCH AND UI CONFLICTS

## Pan — CONFIRMED

Official release notes:

- Focus View supports panning;
- panning is bounded by the focused canvas.

## Zoom — CONFIRMED

Official release notes:

- Focus View supports zooming.

## Two-finger pinch specifically — NOT CONFIRMED BY CURRENT TEXT DOCUMENTATION

The reviewed official documents say "zoom" but do not explicitly specify:

> zoom is always performed with a two-finger pinch gesture.

Do not write "pinch confirmed" into code comments, product specs or marketing until Stef tests it on her actual device/client.

### Required test

On the real phone:

1. enter native Focus View;
2. place two fingers on the book-page area;
3. spread fingers;
4. pinch fingers together;
5. record whether zoom changes.

If not:

- determine how VRChat exposes native zoom on that client;
- do not immediately assume the feature is broken.

---

## ScrollRect conflict — CONFIRMED

VRChat 2024.2.1 notes explicitly state:

> Focus View pan/zoom is not available while the user is interacting with a selectable object / scroll element.

This matters directly because the current Book_A mobile page uses a `ScrollRect`.

## Recommended native mobile page setup

For native Focus View:

### Page surface

Prefer:

- `RawImage` showing `RT_EReader`;
- no full-page ScrollRect if native pan/zoom should own gestures;
- avoid making the entire page a selectable control;
- only keep raycast targets where genuinely needed.

### Buttons

Keep dedicated controls such as:

- Previous;
- Next;
- -10;
- +10;
- First;
- page indicator;
- possibly Pin;
- possibly X/pending close.

Buttons are selectables. While touching a button, native pan/zoom should not occur. That is desirable.

### Do not wrap the whole reading canvas in a giant invisible selectable

That could effectively suppress native pan/zoom across the entire surface.

### Center pivot

Because of the reported Focus View panning/pivot bug, use conventional centered geometry/pivot for the main focus canvas unless a real-device test proves another setup safe.

---

# 10. QUESTION 6 — CANVAS SETUP

## Official base setup for VRC world UI

VRChat's VRC_UIShape docs recommend:

1. create a Canvas;
2. add `VRC_UIShape`;
3. use Default or another valid world-interactable layer rather than Unity UI layer where appropriate;
4. scale the world-space UI correctly;
5. use **World Space** render mode;
6. add UI controls normally.

## Focus View requirements

For Focus View to be available:

- correctly configured Canvas;
- `VRC_UIShape`;
- `Allow Focus View = true`;
- world has not disabled Focus View;
- user is on phone/tablet;
- touchscreen is sole input;
- user has not disabled Focus View;
- distance is within the client-defined activation/deactivation range.

Current documented distance range:

- activation/near boundary depends on canvas size, roughly **0.6–2 m** minimum;
- deactivation/far boundary depends on canvas size, roughly **3–6 m** maximum.

Do not hardcode one exact distance from these ranges.

## Important Book_A implication

The current PC focus panel is deliberately camera-following.

Native mobile Focus View should not need that.

Recommended:

```text
Book_A
└── MobileNativeFocusCanvas
    ├── RawImage / RT_EReader
    ├── navigation controls
    └── optional mobile help text
```

Keep the mobile native canvas in world space with Book_A.

Let VRChat's Focus View camera frame it.

## Why ScreenCamera-following is discouraged here

There is a VRChat Canny report in which:

`VRCCameraSettings.ScreenCamera.Position`
and
`Rotation`

returned world origin during Focus View.

That could cause a feedback loop or panel jump if the focused panel is continuously positioned using ScreenCamera values.

This is a bug report, not a guaranteed current behavior, but there is no benefit in taking the risk for a feature that already has its own camera framing system.

### Rule for implementation

- PC route may keep its current ScreenCamera-following system because it is already proven.
- Native mobile Focus View should be a **separate canvas** not driven by the existing PC follow loop.

---

# 11. QUESTION 7 — PLATFORM, ORIENTATION, INPUT AND SETTINGS

## Android

Focus View was introduced/documented for Android Mobile.

Current VRC_UIShape docs now describe Focus View generically for phone/tablet.

## iOS

VRChat's current iOS creator docs state that guidance referring to Android/Quest generally applies to iOS as well.

iOS remains a platform that requires its own real-device testing.

Do not infer Android success guarantees iOS success.

## Portrait / landscape

Explicitly supported by VRChat Focus View release documentation.

Both must still be tested because:

- Book_A's control layout may need safe areas;
- device aspect ratios vary;
- VRChat HUD layout can differ.

## Touch vs external controller

Focus View requirements say:

- touchscreen must be the user's **only input device** for entry;
- Focus View exits if the active input method changes away from touch.

Implication:

A phone/tablet user with an active gamepad/controller may not be eligible for native Focus View.

This should be treated as an expected limitation, not necessarily an EReader bug.

## User settings

User can disable Focus View in VRChat settings.

Therefore native Focus View must not be assumed available to every mobile user.

## World settings

World creator can disable Focus View for the world.

Therefore verify that Open_Classroom world settings allow it before acceptance.

## Canvas-specific setting

`VRC_UIShape.Allow Focus View` must be enabled for the mobile canvas.

## Input detection available to Udon

Official mobile best-practice APIs:

```csharp
public override void OnInputMethodChanged(VRCInputMethod inputMethod)
```

and:

```csharp
VRC.SDKBase.InputManager.GetLastUsedInputMethod()
```

can identify `VRCInputMethod.Touch`.

Existing project input detection may be reusable, but implementation should inspect current code instead of duplicating systems.

---

# 12. CLIENTSIM — WHAT IT CAN AND CANNOT PROVE

Official ClientSim documentation states:

- ClientSim replicates many VRChat behaviors in Unity;
- player is controlled with mouse/keyboard/gamepad;
- UI can be exercised;
- ClientSim cannot simulate every VRChat feature;
- creators should test in the real VRChat client before publishing.

For this task:

## ClientSim can help with

- Udon compilation;
- local state transitions;
- button target wiring;
- visibility toggles;
- non-mobile fallback;
- basic Canvas/UI references.

## ClientSim must NOT be accepted as proof for

- native mobile Focus View entry;
- exact mobile HUD;
- exact avatar-control suppression;
- native touch pan;
- native zoom gesture;
- two-finger pinch;
- portrait/landscape behavior;
- Android vs iOS differences;
- safe native Focus View exit behavior.

Final mobile acceptance requires a real phone/tablet.

---

# 13. NATIVE MOBILE ARCHITECTURE — RECOMMENDED DIRECTION

Do not merge the PC and native mobile view into one clever all-platform controller if that risks the proven PC path.

Prefer explicit separation.

```text
Book_A_Root
│
├── existing VR physical system
│   ├── LeftHandle
│   ├── RightHandle
│   └── Book_A body
│
├── existing PC custom focus system
│   └── Book_A_FocusCanvas
│       └── current camera-follow / zoom / ScrollRect behavior
│
└── proposed MobileNativeFocusCanvas
    ├── Canvas (World Space)
    ├── VRC_UIShape
    │   └── Allow Focus View = true
    ├── centered RectTransform / conventional pivot
    ├── Page
    │   └── RawImage -> RT_EReader
    ├── NavigationControls
    │   ├── First
    │   ├── -10
    │   ├── Previous
    │   ├── Next
    │   └── +10
    ├── PageStatus
    └── optional close/help UI
```

## Reuse

Mobile native route should reuse:

- `RT_EReader`;
- existing `EReaderLocalPlaybackManager`;
- existing Book_A page navigation;
- existing local progress;
- existing page bounds;
- existing local reader identity;
- existing local-only reading architecture.

Do not add:

- a second video player;
- networking;
- ObjectSync;
- shared page state;
- Book_B conversion;
- bookmarks/library in this phase.

---

# 14. PROPOSED MOBILE STATE FLOW

This is a recommendation, not a currently implemented contract.

## Initial

```text
Normal mobile world state
CameraMode = Screen
Book_A is available in world
```

## User presses/taps LEZEN

Recommended behavior:

```text
Book_A enters "mobile reading requested" local state
MobileNativeFocusCanvas becomes eligible/available
Do not move it onto ScreenCamera
Do not automatically deactivate physical Book_A
```

The user then enters Focus View through VRChat's native UI interaction.

## Detect native Focus entry

When:

```text
VRCCameraSettings.ScreenCamera.CameraMode == FocusView
```

Book_A may mark:

```text
mobileNativeFocusActive = true
```

Only local state.

No networking.

## While reading

- VRChat owns camera Focus View;
- VRChat owns native pan/zoom;
- Book_A owns page content;
- Book_A buttons own page navigation;
- playback/progress remain unchanged.

## User exits native Focus View

When:

```text
CameraMode changes back to Screen
```

Book_A may:

- clear mobile native focus state;
- finish any pending reader-close request;
- restore normal mobile controls;
- preserve progress;
- preserve physical Book_A position unless user explicitly chose Reset/Home.

## Reset

Reset remains a separate physical action.

Never use Focus View exit itself as a reason to erase:

- page;
- progress;
- library state;
- physical placement unless product design explicitly says so.

---

# 15. LEZEN BUTTON — PRODUCT BEHAVIOR RECOMMENDATION

Because no supported Udon "enter Focus View" API was found, avoid promising one-tap automatic native focus until tested.

Possible UX:

### Step 1

User taps **LEZEN** on Book_A.

### Step 2

The mobile Focus-capable canvas becomes the active reader target.

### Step 3

A short local hint may say:

> Tik op de boekpagina om Focus View te openen.

Exact wording can be designed later.

If native VRChat automatically enters Focus View when tapping an eligible canvas, keep the experience as short as possible.

Do not create fake "automatic" behavior through undocumented hacks.

---

# 16. X / CLOSE — PRODUCT BEHAVIOR RECOMMENDATION

Two states must be separated:

1. EReader content wants to close.
2. Native VRChat Focus View camera wants to exit.

They are not proven to be creator-controllable through the same Udon event.

## Safe first implementation

Native VRChat exit is authoritative.

Flow:

```text
user uses native Focus View exit
-> CameraMode becomes Screen
-> Book_A detects it
-> Book_A closes/cleans up local reading state
```

## Later refinement

If Stef wants an X in the reader UI:

```text
X
-> set pendingClose = true
-> preserve focus canvas while CameraMode == FocusView
-> show small "Exit Focus View" hint if needed
-> when CameraMode == Screen:
   finish CloseReader
```

Do not silently call `SetActive(false)` on the native focus target while still focused unless real-client testing and updated documentation justify it.

---

# 17. HUD SPACE STRATEGY

The current custom mobile route reserves about 32% of the bottom screen.

That was a workaround, not an official safe-area measurement.

Do not carry that 32% number into native Focus View by default.

Native Focus View must first be tested on Stef's real phone.

Measure:

- actual page bounds;
- native Focus View exit control;
- mic/PTT;
- menu access;
- remaining client HUD;
- portrait safe area;
- landscape safe area.

Only then reserve margins if necessary.

## Useful official SDK helper

The official Screen Canvas example includes:

`VRCButtonLayout`

which visually shows approximate regions occupied by VRChat mobile controls in editor/design time.

This can be useful for the custom fallback UI, but it does not replace a real Focus View device test.

---

# 18. WHAT TO DO WITH THE CURRENT MOBILE SCROLLRECT

Current custom route uses a ScrollRect for page drag/pan.

For native Focus View:

**Do not automatically reuse it.**

Reason:

VRChat explicitly documents that native Focus View pan/zoom does not work while interacting with a selectable/scroll element.

Recommended prototype:

- native page RawImage;
- no ScrollRect over the page;
- native Focus View owns pan/zoom;
- page buttons remain normal Buttons.

Keep the PC ScrollRect unchanged.

The PC route already works.

---

# 19. WHAT TO DO WITH CUSTOM ZOOM BUTTONS

Do not delete existing PC zoom controls.

For mobile native prototype:

- first test VRChat native zoom;
- leave project zoom buttons out of the first minimal prototype unless needed;
- if native zoom proves insufficient, custom zoom controls may be reintroduced later, but that should not fight native Focus View transform/pan logic.

The goal is to avoid two independent zoom systems manipulating the same reading surface.

---

# 20. PHONE PROOF-OF-CONCEPT — MINIMUM TEST

Before implementing the full mobile route, build only the smallest possible test target.

## Prototype contents

One world-space test canvas attached to Book_A or placed next to it:

- `VRC_UIShape`;
- `Allow Focus View = true`;
- RawImage showing `RT_EReader`;
- one `NEXT` button;
- page/status label;
- no ScrollRect;
- no ScreenCamera-following;
- centered pivot;
- no custom close logic that disables the canvas.

Do not remove the existing mobile implementation yet.

---

# 21. REAL-PHONE TEST PLAN

Stef performs these tests.

Record result for each item as:

- PASS
- FAIL
- DIFFERENT THAN EXPECTED
- NOT TESTED

## Test A — Can Focus View open?

1. Open Open_Classroom on phone.
2. Stand/move Book_A into an eligible distance.
3. Tap the native Focus-capable reader canvas.
4. Confirm whether Focus View opens.

Record:

- how Focus View is triggered;
- number of taps;
- whether LEZEN needs to prepare anything first.

## Test B — Avatar movement

1. Enter Focus View.
2. Drag/pan the page.
3. Watch whether avatar position changes.

Expected from official docs:

- avatar remains in place.

## Test C — Camera / pan

1. Drag the focused page.
2. Confirm view pans within canvas bounds.
3. Check if normal world camera turning occurs at the same time.

## Test D — zoom

1. Try two-finger spread.
2. Try two-finger pinch.
3. If nothing happens, identify native zoom control/gesture.

Record:

- pinch confirmed yes/no;
- alternative zoom mechanism if any.

## Test E — page button

1. Tap `NEXT`.
2. Confirm page changes exactly once.
3. Confirm button press does not accidentally pan the page.

## Test F — HUD inventory

While still focused, record:

| Client control | Visible | Usable | Notes |
|---|---|---|---|
| Movement joystick | | | |
| Camera/look joystick | | | |
| Jump | | | |
| Mic/PTT | | | |
| Menu buttons | | | |
| Focus exit | | | |

Take one screenshot if useful.

## Test G — portrait

- enter Focus View in portrait;
- pan;
- zoom;
- Next;
- inspect page space.

## Test H — landscape

- rotate to landscape;
- repeat pan/zoom/Next;
- check whether Focus View remains stable through orientation change.

## Test I — native exit

1. Use VRChat's native Focus View exit.
2. Confirm normal mobile controls return.
3. Confirm avatar can move again.
4. Confirm Book_A progress remains.

## Test J — external input edge case, optional

If a Bluetooth/gamepad controller is available:

1. connect it;
2. attempt Focus View;
3. verify documented touch-only limitation.

This test is optional.

---

# 22. ACCEPTANCE GATE FOR FULL MOBILE IMPLEMENTATION

Do not proceed to a full native mobile EReader implementation until the proof-of-concept answers:

- Focus View can reliably open on Stef's device;
- avatar stays still;
- page can pan;
- zoom mechanism is understood;
- Next button works;
- native exit reliably restores normal controls;
- portrait is usable;
- landscape is usable;
- HUD occupancy is known;
- no soft-lock occurs;
- physical Book_A remains sane after exit.

If any core item fails, stop and redesign only the mobile route.

Do not touch proven VR/PC behavior as the first response.

---

# 23. FALLBACK IF NATIVE FOCUS VIEW IS NOT SUITABLE

If native Focus View proves unreliable for the actual EReader, supported alternatives remain.

## Alternative A — keep custom WorldSpace/ScreenCamera mobile mode

Pros:

- already mostly implemented;
- shared with current PC architecture;
- full creator control over layout.

Cons:

- current swipe can conflict with mobile avatar/camera controls;
- creator cannot assume control of native VRChat HUD;
- custom pan/zoom requires more code;
- true multi-touch/pinch is not currently proven available through the project's Udon approach.

Potential improvement:

- avoid drag gestures;
- use explicit directional/page/zoom buttons;
- reserve only measured HUD-safe areas rather than arbitrary 32%.

## Alternative B — dedicated Screen Space mobile UI

VRChat's official Screen Canvas example supports mobile screen UI.

Pros:

- designed for 2D displays;
- easy tapping;
- can use resolution/orientation-responsive layout.

Cons:

- does not automatically provide Focus View's "avatar remains in place" behavior;
- does not guarantee VRChat HUD disappears;
- user touch may still interact with client controls depending on layout;
- previous project ScreenSpace overlay attempt had input/collider problems and would need a clean supported implementation rather than revival by assumption.

## Alternative C — separate immobilization technique

A world-level mechanism could potentially stop the avatar while reading, but:

- immobilizing the avatar is not the same as hiding VRChat HUD;
- it may have accessibility/UX side effects;
- it was not researched in this Focus View investigation;
- do not implement a Station/immobilization hack without a separate design and test task.

Native Focus View should be tested first.

---

# 24. KNOWN RISK REGISTER

| Risk | Level | Mitigation |
|---|---:|---|
| Trying to programmatically enter/exit native Focus View via undocumented API | High | Do not use guessed APIs. Use Allow + detection + native user flow. |
| Disabling focused canvas while native Focus View is active | High | Keep focus target alive until native exit is observed. |
| Reusing full-page ScrollRect | High | Remove from native mobile prototype; let VRChat own pan/zoom. |
| ScreenCamera-follow loop during native Focus View | Medium/High | Keep native mobile canvas world-attached to Book_A. |
| Assuming zoom = pinch | Medium | Verify on phone. |
| Assuming HUD controls disappear | Medium | Inventory actual HUD on device. |
| Copying 32% bottom margin into native Focus View | Medium | Measure actual Focus View first. |
| Editing proven VR controller/constraints | High | Explicitly out of scope. |
| Replacing current PC route | High | Explicitly preserve it. |
| Treating ClientSim as mobile proof | High | Real VRChat phone test mandatory. |
| Canvas pivot causing bad pan bounds | Medium | Use centered pivot; device test. |
| Touch-only eligibility fails with controller connected | Expected limitation | Detect/fallback to custom mobile route if needed. |
| User/world Focus View setting disabled | Expected limitation | Provide graceful fallback. |

---

# 25. SUGGESTED IMPLEMENTATION PHASES AFTER A FUTURE GO

These phases are recommendations only.

## Phase M0 — proof canvas

Goal:

- prove native Focus View behavior on Stef's phone.

Changes should be minimal and reversible.

No replacement of current mobile route.

## Phase M1 — mobile native page

After M0 passes:

- add `MobileNativeFocusCanvas`;
- reuse RT and navigation;
- no ScrollRect page;
- native Focus allowed;
- detect `CameraMode`.

Test.

## Phase M2 — integrate LEZEN and exit lifecycle

After M1 passes:

- LEZEN prepares native mobile reading state;
- detect FocusView entry;
- detect Screen return;
- synchronize local reader open/close state;
- preserve physical position/progress.

Test.

## Phase M3 — polish

Only after functionality is proven:

- safe areas;
- HUD alignment;
- labels/hints;
- portrait/landscape refinement;
- optional X pending-close UX;
- fallback when Focus View unavailable.

## Phase M4 — regression

Retest:

- PC;
- VR;
- mobile;
- Book_B unchanged.

---

# 26. DO-NOT-BREAK CONTRACT

Any future implementation must preserve the following unless Stef explicitly changes the product requirements.

## VR

- two separate handles;
- existing constraints;
- safe pickup/drop/reset;
- proven VR behavior.

## PC

- current accepted local focus reading;
- same playback;
- same progress;
- same page controls;
- same custom PC zoom/pan;
- physical LEZEN / VERPLAATSEN distinction;
- X position preservation;
- TERUGZETTEN Home behavior.

## Shared reading logic

- one existing local playback manager;
- same `RT_EReader`;
- no second video player;
- local page/progress;
- no networking for reading state.

## Scope

- Book_A only.
- Book_B is not converted in this phase.

---

# 27. IMPLEMENTATION NOTES FOR CODEX / ASTRA

When implementation is later authorized:

1. Read `EREADER_BOOK_A_WORK.md` fully.
2. Read this document fully.
3. Inspect the **current** SDK/package versions installed locally.
4. Search local SDK source/node exposure before assuming a public API exists.
5. Prefer current official VRChat creator docs over old release notes.
6. Treat Canny reports only as warnings/test evidence.
7. Do not repeat a broad project audit.
8. Do not modify VR handles/controllers unless a concrete dependency requires it and Stef approves.
9. Do not replace the proven PC path just to share code.
10. Keep mobile Focus state local.
11. No networking/ObjectSync.
12. No new video player.
13. No bookmarks/library in this task.
14. No autonomous Play Mode/test cycle; Stef performs device acceptance unless she explicitly asks otherwise.
15. Update `EREADER_BOOK_A_WORK.md` after each completed implementation/test step.
16. Clearly label evidence:
    - IMPLEMENTED
    - COMPILE CHECKED
    - CLIENTSIM TESTED
    - DESKTOP TESTED
    - MOBILE ANDROID TESTED
    - MOBILE IOS TESTED
    - PCVR TESTED
    - QUEST TESTED
    - MULTIPLAYER TESTED
17. Never infer one label from another.

---

# 28. WHAT IS FACT VS INFERENCE

## Confirmed by official documentation

- Focus View applies to eligible phone/tablet world-space UI.
- `VRC_UIShape.Allow Focus View` exists.
- users must be using touch as their only input method.
- user/world settings can disable Focus View.
- distance restrictions exist.
- avatar stays in place.
- native pan exists.
- native zoom exists.
- portrait and landscape are supported.
- users manually close Focus View.
- changing input method away from touch exits Focus View.
- local and remote avatars are not rendered while focused.
- native pan/zoom is not available while interacting with selectable/scroll UI.
- `VRCCameraSettings.ScreenCamera.CameraMode` can report `FocusView`.
- ClientSim cannot simulate all VRChat behavior.
- `OnInputMethodChanged` and `GetLastUsedInputMethod()` can detect touch input.
- official screen-space UI example exists for Desktop/Mobile.

## Not confirmed by current official text docs

- exact mobile joystick visibility during Focus View;
- exact jump button visibility;
- exact PTT position during Focus View;
- exact menu button availability during Focus View;
- exact native exit button position;
- two-finger pinch specifically as the zoom gesture;
- supported Udon API to enter Focus View;
- supported Udon API to exit Focus View;
- disabling canvas as a supported exit;
- `OnVRCCameraSettingsChanged` firing exactly on every FocusView mode transition.

## Evidence from bug reports, not guaranteed current behavior

- ScreenCamera position/rotation returning world origin during Focus View;
- focused canvas disable/delete causing iOS soft-lock;
- focus canvas pivot affecting pan bounds;
- teleport being blocked while Focus View remains active.

## Project-specific design inference

Recommended but not officially mandated:

- separate mobile native canvas;
- keep it attached to Book_A;
- no ScrollRect over the main native-focus page;
- detect FocusView state;
- keep native focus target alive until Screen mode returns;
- preserve current PC and VR systems unchanged.

---

# 29. SHORT DECISION SUMMARY FOR THE NEXT SESSION

**Current decision direction:**

Use native VRChat Focus View for a **small mobile proof-of-concept first**.

Do not yet replace the current mobile route.

The proof-of-concept must determine:

1. exact Focus View entry gesture;
2. actual mobile HUD layout;
3. whether avatar stays still as documented;
4. actual pan behavior;
5. actual zoom gesture / whether pinch works;
6. button interaction;
7. portrait behavior;
8. landscape behavior;
9. native exit behavior;
10. whether Book_A can safely coordinate around `CameraMode`.

If this passes, build a separate `MobileNativeFocusCanvas` while preserving the proven PC and VR baselines.

---

# 30. RECOMMENDED START PROMPT FOR THE NEXT CODEX RESEARCH/PROTOTYPE SESSION

Use this only when Stef is ready to continue.

```text
Project: Open_Classroom / Book_A.

Read EREADER_BOOK_A_WORK.md fully first.
Then read EREADER_MOBILE_FOCUS_VIEW_RESEARCH_2026-09-16.md fully.

The VR two-handle implementation is a proven baseline.
The current PC reading implementation is a proven baseline.
Do not redesign either one.

The mobile route is not accepted yet.

For now, do not implement a full replacement.

First inspect only the current Book_A mobile/focus wiring and the installed VRChat SDK APIs needed for a minimal native Mobile Focus View proof-of-concept.

Use the research document's evidence rules:
- current official VRChat docs > old release notes;
- Canny bug reports are warnings, not API contracts;
- do not invent an EnterFocusView/ExitFocusView API;
- do not assume native zoom specifically means pinch until device-tested;
- do not disable/delete a focused canvas as an exit hack;
- avoid reusing the PC ScreenCamera-following panel for native mobile Focus View;
- avoid a page-filling ScrollRect in the native Focus View prototype.

Before changing anything, give Stef a short proposed M0 proof-of-concept:
- exact objects/components to add/change;
- what remains untouched;
- how it will be tested on a real phone;
- rollback boundary.

Wait for explicit GO before implementing.
```

---

# 31. PRIMARY SOURCE INDEX

## Current official VRChat creator docs

**VRC UI Shape / Focus View**  
https://creators.vrchat.com/worlds/components/vrc_uishape/

**VRCCameraSettings**  
https://creators.vrchat.com/worlds/udon/vrc-graphics/vrc-camera-settings/

**Mobile Best Practices**  
https://creators.vrchat.com/platforms/android/android-best-practices/

**iOS creator overview**  
https://creators.vrchat.com/platforms/iOS/

**ClientSim**  
https://creators.vrchat.com/worlds/clientsim/

**Screen Canvas example**  
https://creators.vrchat.com/worlds/examples/screen-canvas/

**Platforms**  
https://creators.vrchat.com/platforms/

## Official historical release notes

**VRChat 2024.2.1 — Focus View introduction**  
https://docs.vrchat.com/docs/202421

## VRChat feedback / bug reports

**ScreenCamera Position/Rotation and Focus View**  
https://feedback.vrchat.com/udon/p/vrccamerasettingsscreencamera-postion-rotation-returns-world-origin-if-focusing

**iOS Focus View soft-lock when focused canvas disappears**  
https://feedback.vrchat.com/ios-mobile-beta/p/ios-focus-menu-soft-lock

**Focus View blocks player teleportation**  
https://feedback.vrchat.com/udon/p/focus-view-blocks-player-teleportation

**Focus View panning / canvas pivot report**  
https://feedback.vrchat.com/mobile-beta/p/focus-view-problems-on-some-canvases

---

# 32. FINAL STATUS

**Research:** COMPLETE for current design decision.  
**Implementation:** NOT STARTED.  
**Files/scenes/scripts changed by this research:** NONE.  
**Play Mode started:** NO.  
**ClientSim started:** NO.  
**Mobile native Focus View device test:** NOT YET PERFORMED.  
**Recommended next action:** minimal M0 native Focus View proof-of-concept, only after Stef gives explicit GO.

The current working PC and VR implementations remain the protected baseline.

