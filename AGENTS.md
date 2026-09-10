# AGENTS.md — Open Classroom

## Purpose
This repository contains Open Classroom VRChat project reference material, including scripts and screenshots from the current Unity project. Treat existing material as valuable project truth and backup evidence.

## Ecosystem routing

Stef's normal Nova/ChatGPT entrypoint for the wider StefanieInVR ecosystem is:

`mailfromstefanie/StefanieInVR-Project-Hub/STARTPROMPT.txt`

This repository remains authoritative for Open Classroom implementation and evidence.
The Project Hub owns routing/cross-project summaries only and never overrides `CURRENT_WORK.md`, recovery decisions or the real Stef-approved Unity scene.

For a direct Open Classroom/Codex specialist session, use `START_PROMPT.md`.

## Collaboration model
Stef normally works through Nova (ChatGPT) as the technical intermediary.

Normal route:

Stef explains the desired experience
→ Nova identifies whether Open Classroom is source project, target project or both
→ Nova translates the active Classroom responsibility into one small technical task
→ Codex performs only that scoped task when useful
→ Stef tests the result in Unity / VRChat
→ Nova helps interpret the result and keeps GitHub project memory current

Codex is an implementation worker, not the product owner. Do not make product, UX, architecture, networking or scope decisions for Stef unless the repository already records them as accepted truth or the task explicitly authorises the decision.

## Cross-project boundary

When another StefanieInVR repository is read as reference:

```text
SOURCE PROJECT
= where a pattern is already proven

TARGET PROJECT
= where the current change is actually being built
```

Do not edit Art House Cinema or Presentation Service merely because their information was consulted.
Do not claim another project is implemented because Classroom proves a similar system.

## Communication with Stef
- Use simple/noob-friendly Dutch unless Stef asks otherwise.
- Explain what is changing and why before technical detail.
- For manual Unity/GitHub work, give one small action at a time.
- Name the exact file, GameObject, component or Inspector field when known.
- Never assume Stef is a programmer.
- Keep explanations short and practical.

## Change discipline
- Read relevant existing material before editing.
- Preserve working scripts, screenshots and backup/reference material.
- Never reorganize, rename or delete backup material merely to make the repository cleaner.
- Make the smallest permanent-oriented change that solves the current task.
- Do not broaden scope or redesign adjacent systems.
- Do not silently change local/global/network semantics.
- During recovery, never fix forward from a scene Stef has rejected when `CURRENT_WORK.md`/recovery docs require rollback.

## Testing and truth
- Never claim Unity, VRChat, PC, Quest or multiplayer behaviour is proven unless it was actually tested in the appropriate environment.
- ClientSim/editor behaviour is useful evidence but not final VRChat proof.
- A compile or Play Mode smoke pass does not prove visual/UI acceptance.
- Stef's real visual/functional rejection overrides earlier automated smoke notes.
- After a change, state exactly what Stef should test.
- Record proven results separately from assumptions.

## Repository safety
This repository also functions as backup/reference evidence from the real Unity project. Existing scripts and screenshots must be treated conservatively. Do not replace or remove them unless Stef explicitly asks for that exact change.

## Codex task style
When Nova provides a scoped task, follow that scope closely. Prefer:

inspect
→ explain briefly
→ change one thing
→ report exact files changed
→ give exact test
→ stop

Do not continue into the next feature automatically.

## Session close

Update Open Classroom files only for truth that actually changed.
Update `mailfromstefanie/StefanieInVR-Project-Hub/CURRENT_ECOSYSTEM.md` only when a real cross-project milestone or dependency changes.
Do not rewrite the Hub master startprompt after ordinary Classroom sessions.
