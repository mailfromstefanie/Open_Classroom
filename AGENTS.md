# AGENTS.md — Open Classroom

## Purpose
This repository contains Open Classroom VRChat project reference material, including scripts and screenshots from the current Unity project. Treat existing material as valuable project truth and backup evidence.

## Collaboration model
Stef normally works through Nova (ChatGPT) as the technical intermediary.

Normal route:

Stef explains the desired experience
→ Nova translates it into one small technical task
→ Codex performs only that scoped task when useful
→ Stef tests the result in Unity / VRChat
→ Nova helps interpret the result and keeps GitHub project memory current

Codex is an implementation worker, not the product owner. Do not make product, UX, architecture, networking or scope decisions for Stef unless the repository already records them as accepted truth or the task explicitly authorises the decision.

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
- If intent is ambiguous and the choice affects behaviour or architecture, stop and ask.

## Testing and truth
- Never claim Unity, VRChat, PC, Quest or multiplayer behaviour is proven unless it was actually tested in the appropriate environment.
- ClientSim/editor behaviour is useful evidence but not final VRChat proof.
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