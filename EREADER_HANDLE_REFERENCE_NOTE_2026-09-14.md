# EReader handle reference correction — 2026-09-14

Status: **CURRENT SOURCE-OF-TRUTH NOTE FOR THE EREADER HANDLE FOUNDATION**

This note clarifies the reference-source rule in `EREADER_V1_PRODUCT_SPEC_2026-09-14.md`.

## Important correction

Do **not** assume the HandheldUI scripts currently stored in the Art House Cinema GitHub repository are the latest working versions.

For the EReader V1 physical left/right ParentConstraint foundation, use this evidence order:

```text
1. real current Open Classroom Unity scene/runtime for the TARGET EReader state
2. real current Art House Cinema Unity project, inspected READ-ONLY, for the proven Paper Tablet ParentConstraint/left-right-handle reference
3. current scripts/components actually attached to that Cinema Paper Tablet when available through the local Unity project
4. example scripts supplied directly by Stef when requested
5. older Cinema GitHub HandheldUI scripts only as secondary historical/reference evidence
```

The real current Cinema Paper Tablet may be inspected with Kitwright to understand the working ParentConstraint hierarchy, handle references, constraints, pickup behaviour and reset pattern.

## HARD READ-ONLY BOUNDARY FOR CINEMA

The Art House Cinema Unity project is **REFERENCE ONLY** for this task.

Codex/Astra may inspect it, but must not modify it in any way.

Specifically, in the Cinema project do NOT:

- edit or save scripts;
- move, rename, add or delete GameObjects;
- change Transform values;
- add/remove/reconfigure components;
- alter ParentConstraints or source weights;
- change VRC Pickup, Rigidbody, collider or VRCObjectSync settings;
- change prefabs, scenes, materials or assets;
- run cleanup/refactor work;
- make any opportunistic fixes or improvements.

Cinema is SOURCE/REFERENCE ONLY.
Open Classroom is the only TARGET project where EReader implementation changes are allowed.

If a tool or workflow would require writing/importing/changing something in Cinema beyond Stef's already-authorized Kitwright installation, stop and ask Stef first.

## Standalone EReader requirement remains unchanged

The EReader must still be built as a standalone reusable prefab.

Therefore:

- do not make the EReader depend on the Paper Tablet scripts;
- do not reuse a Paper Tablet-specific controller as an EReader dependency;
- create EReader-specific handle/input/reset bridge/controller scripts where needed;
- shared generic physical ideas may be adapted, but the final EReader implementation must own its own behaviour;
- the Paper Tablet is a reference/example and later optional input adapter only.

## If the exact working example code is missing

If Codex/Astra cannot determine the proven ParentConstraint behaviour from the real Cinema Paper Tablet with confidence, it must **STOP and ask Stef for the current example scripts**.

Stef can provide the working handle/reset scripts directly.

Do not guess, reconstruct from stale Cinema GitHub files, or silently invent missing behaviour.

## Product specification relationship

All product behaviour in `EREADER_V1_PRODUCT_SPEC_2026-09-14.md` remains accepted.

This note changes only the evidence/reference priority and enforces a strict read-only boundary for the Cinema project while implementing the physical EReader handle foundation in Open Classroom.
