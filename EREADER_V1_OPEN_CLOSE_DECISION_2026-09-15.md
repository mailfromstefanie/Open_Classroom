# EReader V1 — Open / Close Behaviour Decision — 2026-09-15

Status: **ACCEPTED PRODUCT DECISION / VR DROP RULE NOT YET IMPLEMENTED OR ACCEPTANCE-PROVEN**

Implementation reconciliation — 2026-09-16:
- The physical Book_A two-handle baseline exists and Stef reported it successful in VRChat.
- Current local `EReaderBook.EndPhysicalHold()` still closes when Pin is off and custom focus is not open. This document's always-stay-open-on-final-drop target therefore remains an explicit implementation gap.
- PC/mobile now have separate LEZEN and movement-only VERPLAATSEN, plus X preserving physical placement/progress and explicit TERUGZETTEN. PC is accepted; mobile is not. The first-pickup-opens description below applies to the VR handle target, not the non-VR movement grip.
- Saved Book_A distance tuning remains 1 m / 30 s / 5 s checks; the 4 m / 15 s recommendation below has not been applied or proven.
- Native mobile Focus View is only research/proposed proof. Its native entry/exit constraints are not a silent amendment of this product decision.
- See [EREADER_BOOK_A_WORK.md](EREADER_BOOK_A_WORK.md). No behaviour is changed as part of this documentation reconciliation.

This decision refines and overrides the earlier drop/Keep Open wording in `EREADER_V1_PRODUCT_SPEC_2026-09-14.md` for EReader open/close behaviour.

Real Unity/VRChat behaviour remains the strongest source of truth once implemented and tested.

---

## Accepted user experience

Dropping the physical EReader does **not** mean the user is finished reading.

Therefore:

```text
release final active handle
-> physical hold ends
-> reader stays open
-> current local reading state stays intact
```

The reader closes only when one of these happens:

1. the local user explicitly presses the reader Close / X control; or
2. the local user remains sufficiently far away from the reader for a grace period.

A temporary step across the distance boundary must not instantly close the reader.

---

## Distance-close behaviour

Desired pattern:

```text
user moves beyond configured distance
-> grace timer starts

user returns within range before timer expires
-> cancel timer
-> reader stays open

user remains beyond distance for full grace period
-> close local reader
```

Starting implementation recommendation for testing:

- distance: approximately **4 metres**;
- grace period: approximately **15 seconds**.

These exact numbers are tuning defaults, not yet acceptance-proven constants. They may be adjusted after real PCVR/Quest usability testing.

Distance-close is a **local per-player** behaviour. It must not globally close another player's reader state.

---

## Responsibility split

```text
LEFT / RIGHT HANDLES
= physical pickup / hold / drop input

EREADER MANAGER / READER CONTROLLER
= local open/close behavioural authority

CLOSE / X BUTTON
= explicit local close request

DISTANCE CHECK
= local automatic close request after sustained out-of-range time
```

Dropping a handle must not directly own reader visibility.

---

## Two-handle semantics after this decision

```text
first handle pickup
-> activate/open local reader once if needed

second handle pickup
-> physical two-hand behaviour only
-> no duplicate reload

release one of two handles
-> still physically held
-> reader stays open

release final active handle
-> no longer physically held
-> reader still stays open
```

The existing `EReaderBook.OnDrop()` behaviour that closes when Keep Open is false must therefore **not** be preserved as the final V1 product behaviour.

Implementation must separate physical hold/drop state from local reader open/close state.

---

## Keep Open / Pin

Earlier documents treat `Keep Open / Pin` as the switch deciding whether a dropped reader remains visible.

That meaning is superseded by this decision: **normal physical drop now keeps the reader open by default**.

Do not delete or repurpose the existing Keep Open / Pin feature blindly. Inspect its current real use first. Its later V1 role may be retained, changed or removed only through a deliberate follow-up decision after the new open/close foundation works.

---

## Current acceptance consequence

The first Book_A gate now requires:

```text
LEFT pickup works
RIGHT pickup works
only thin handle highlight appears
first pickup opens existing local reader once
second handle does not reload it
releasing one of two handles keeps it held/open
releasing final handle leaves reader open
explicit Close / X can close locally
existing page/navigation/progress behaviour is preserved
```

Distance-close may be implemented in the same open/close manager block or immediately after the handle foundation, but it must be proven before the standalone EReader V1 open/close behaviour is considered complete.

Do not broaden this decision into Book_B, bookmarks, Lesson Books or website-converter work.