# EReader handle reference correction — 2026-09-14

Status: **CURRENT SOURCE-OF-TRUTH NOTE FOR THE EREADER HANDLE FOUNDATION**

This note clarifies the reference-source rule in `EREADER_V1_PRODUCT_SPEC_2026-09-14.md`.

## Important correction

Do **not** assume the HandheldUI scripts currently stored in the Art House Cinema GitHub repository are the latest working versions.

For the EReader V1 physical left/right ParentConstraint foundation, use this evidence order:

```text
1. real current Open Classroom Unity scene/runtime
2. current Paper Tablet ParentConstraint/left-right-handle setup inside that real Classroom project
3. current scripts actually attached/wired in the real Classroom project
4. example scripts supplied directly by Stef when requested
5. older Cinema GitHub HandheldUI scripts only as secondary historical/reference evidence
```

The current Paper Tablet in the Open Classroom Unity project may be inspected to understand the working ParentConstraint hierarchy, handle references, constraints and interaction pattern.

## Standalone EReader requirement remains unchanged

The EReader must still be built as a standalone reusable prefab.

Therefore:

- do not make the EReader depend on the Paper Tablet scripts;
- do not reuse a Paper Tablet-specific controller as an EReader dependency;
- create EReader-specific handle/input/reset bridge/controller scripts where needed;
- shared generic physical ideas may be adapted, but the final EReader implementation must own its own behaviour;
- the Paper Tablet is a reference/example and later optional input adapter only.

## If the exact working example code is missing

If Codex/Astra cannot determine the proven ParentConstraint behaviour from the real Classroom scene/current files with confidence, it must **STOP and ask Stef for the current example scripts**.

Stef can provide the working handle/reset scripts directly.

Do not guess, reconstruct from stale Cinema GitHub files, or silently invent missing behaviour.

## Product specification relationship

All product behaviour in `EREADER_V1_PRODUCT_SPEC_2026-09-14.md` remains accepted.

This note changes only the evidence/reference priority for implementing the physical handle foundation.
