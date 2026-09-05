# Open Classroom Performance Audit — 2026-09-05

Status: **READ-ONLY ANALYSIS — NO PERFORMANCE CHANGES APPLIED**

Codex performed a targeted performance inspection after the multi-e-reader work.

The scene was left:
- out of Play Mode;
- clean / not dirty;
- with no compile/runtime console errors;
- with Profiler disabled again.

## ClientSim profile snapshot

Measured over 30 frames on Stef's local PC:

- Main thread median: 6.03 ms
- Main thread P95: 6.60 ms
- GPU median: 0.50 ms
- visible triangles: 78,510
- draw calls: 36
- SetPass calls: 33

Measured Udon cost:
- Update average: 0.26 ms
- FixedUpdate average: 0.14 ms

Measured UI rendering:
- about 0.07 ms

Interpretation:
- current idle CPU/Udon/UI/geometry cost is low in this local ClientSim sample;
- Marker Pro objects are not indicated as the primary performance bottleneck by this sample.

Do not treat this desktop ClientSim profile as a Quest device PASS.

## Highest-priority findings

### 1. Video RenderTextures — likely unnecessary GPU/memory cost

`Assets/Ereader/RT_EReader.asset`

Reported configuration:
- 720x1280
- 2x MSAA
- automatic mipmaps
- about 10.5 MB

`Assets/#Classroom/Scenes/Classroom/VideoTXLCRT-Quest.asset`

Reported configuration:
- 1920x1080
- 2x MSAA
- automatic mipmaps
- about 23.7 MB
- plus a temporary video buffer of roughly similar size

Codex's recommendation:
- test 1x AA / no MSAA;
- disable automatic mipmaps;
- validate video readability/output afterward.

Reasoning:
video frames generally do not benefit from multisampling of the RenderTexture itself, while MSAA and mip generation can add bandwidth/memory/work.

Presentation RenderTexture was already reported as:
- 1x AA
- no mipmaps

Do not change the proven Presentation RenderTexture as part of this optimization round.

### 2. Large transparent Graphlit glass — likely Quest fill-rate risk

Codex found approximately 10 large transparent Graphlit materials on:
- Classroom windows;
- hallway windows;
- skylight;
- glass doors.

They use transparent rendering / render queue 3000.

Main risk:
- stereo overdraw / fill-rate on Quest;
- potentially much more important than triangle count.

Do not redesign glass blindly from desktop profiling.

If Quest evidence later shows fill-rate pressure, test simpler Quest-safe glass variants deliberately and compare visuals/performance.

### 3. Three NPOT poster textures — PC memory issue

Three PC poster textures were reported at approximately:
- 28.1 MB
- 26.5 MB
- 14.6 MB

Combined reported uncompressed footprint:
- about 69 MB

Their Android import settings were already reported as:
- max size 1024
- ASTC 6x6

Therefore this is primarily a PC memory cleanup opportunity, not the first Quest FPS target.

Possible later cleanup:
- normalize source dimensions/import setup;
- verify desktop compression/memory afterward.

Do not prioritize this ahead of RenderTexture and Quest glass evidence.

### 4. E-reader material has a suspicious emission reference

`Assets/Ereader/M_EReaderScreen.mat`

Reported current mapping:
- `_MainTex = RT_EReader`
- `_EmissionMap = VideoTXLCRT-Quest`
- emission enabled

This appears likely to be an accidental cross-reference.

Before changing it, verify the intended visual result in the real scene.

Likely correction:
- use the e-reader's own `RT_EReader` for emission if emission is needed;
- or disable unnecessary emission if the material does not need it.

This should be treated as a narrow e-reader material fix, not a Presentation/VideoTXL architecture change.

## Recommended optimization order

Use this order and change one risk area at a time:

1. verify/fix the e-reader emission-map reference;
2. test `RT_EReader` with:
   - MSAA = 1x
   - automatic mipmaps OFF;
3. validate e-reader readability/page output;
4. separately test `VideoTXLCRT-Quest` with:
   - MSAA = 1x
   - automatic mipmaps OFF;
5. validate normal VideoTXL and Presentation takeover/restore;
6. only then do real Quest profiling;
7. only if Quest shows fill-rate pressure, test simpler transparent glass variants;
8. later normalize NPOT poster textures for PC memory reduction.

## Protected systems

Do not change without new evidence:
- accepted Presentation networking;
- Presentation RenderTexture settings;
- Presentation/VideoTXL adapter contract;
- projector/readability behavior;
- Bakery/Magic Light Probes merely for runtime optimization;
- unrelated Marker issues.

## Evidence boundary

This audit identifies likely optimization targets.

It does **not** prove:
- Quest frame time;
- Quest thermal behavior;
- glass is definitely the current Quest bottleneck;
- the suggested RenderTexture changes are regression-free.

Each optimization needs a focused before/after validation.
