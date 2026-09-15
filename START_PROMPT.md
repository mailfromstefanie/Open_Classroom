# Start Prompt — Open Classroom

Updated: 2026-09-16 Europe/Amsterdam

This is the direct Classroom specialist entrypoint. Stef's normal cross-project entrypoint remains [the Project Hub STARTPROMPT](https://github.com/mailfromstefanie/StefanieInVR-Project-Hub/blob/main/STARTPROMPT.txt).

## Read in order

1. [AGENTS.md](AGENTS.md).
2. [CURRENT_WORK.md](CURRENT_WORK.md).
3. [EREADER_BOOK_A_WORK.md](EREADER_BOOK_A_WORK.md).
4. [Mobile Focus View research](EREADER_MOBILE_FOCUS_VIEW_RESEARCH_2026-09-16.md) for the current mobile question.
5. [Open/close product decision](EREADER_V1_OPEN_CLOSE_DECISION_2026-09-15.md) and [V1 specification](EREADER_V1_PRODUCT_SPEC_2026-09-14.md) when design scope matters.
6. Historical e-reader, persistence and beta handoffs only when the active task needs them.

Real target: `E:/Projects/Open_Classroom/#Unity/Open_Classroom`.

## Current orientation

Book_A two-handle implementation exists; Stef accepted the previous VRChat handle baseline. PC reading/movement is now reported good. Mobile is partly working but not accepted: swipe also moves the avatar, the layout sacrifices page space, and pinch is absent.

Current custom `EReaderFocusView` is not native VRChat Focus View.

Next proposed step: an isolated native Focus View canvas with a static page and counter button, tested by Stef on her phone. No prototype or full mobile replacement has been built. A documentation-sync request is not implementation GO. If Stef explicitly authorizes the prototype in her new request, do not ask for that same approval again.

## Keep evidence and product targets separate

The accepted final-drop-stays-open decision remains valid, but current VR `EndPhysicalHold()` still closes when Pin is off. This gap is recorded, not resolved by the successful handle test. Do not silently rewrite working VR behaviour during mobile work.

The proposed 4 m / 15 s distance tuning is not current Book_A wiring (1 m / 30 s / 5 s checks). Do not present future defaults as tested.

## Preserve

- Book_A VR handles, controller, constraints and physical scale.
- Accepted PC WorldSpace camera-follow panel, Tab/wheel zoom, ScrollRect pan.
- Separate LEZEN / VERPLAATSEN; X preserves placement/progress; TERUGZETTEN goes Home.
- Existing local playback manager, RT and progress; no second player or reading synchronization.
- Book_B remains on its existing direct-pickup route.
- Presentation, exact VideoTXL 2.5.1, Paper Tablet, markers and unrelated resets.
- Entrance Text / Poster persistence bugs remain parked and unresolved.

Cinema is a read-only source reference; do not repeat its full inspection without a concrete need. Do not modify Cinema or the hosted Presentation Service for a Classroom-only mobile issue.

## Native mobile proof boundaries

- Use an independent stationary WorldSpace canvas, centered pivot, UIShape, Allow Focus View.
- First proof uses static content and a counter button, avoiding existing playback activation opening the custom panel.
- No full-page ScrollRect; native gestures must be measured on device.
- No invented EnterFocusView/ExitFocusView API.
- Do not use disabling the focused canvas as an exit command.
- CameraMode can report FocusView but does not identify the focused canvas.
- HUD disappearance and pinch remain unproven until the phone test.
- Leave current PC/VR and current mobile route intact during the isolated proof.

## Working with Stef

Use short, plain Dutch. For manual steps: explain why, give one small action, wait for her result.
Stef performs practical tests. Do not independently start Play Mode, ClientSim or a platform switch.
Compile after authorized code changes; distinguish compilation from runtime acceptance.
Existing backups are available; no new backup without a concrete reason discussed with Stef.

After an authorized step, record changes, evidence, open points and the next step in the work handoff. Update GitHub only within the authorized scope and distinguish local saves from verified remote commits.

If this prompt is supplied without a new task, summarize the mobile test-panel proposal and establish whether Stef wants to authorize that experiment. Do not restart the completed handle implementation.

## Wider product work

Bookmarks, 5-book personal library, Lesson Books, visibility/network policy, standalone packaging and the PDF/EPUB converter remain later scoped work under the V1 spec. No mobile research document grants permission to implement those features.
