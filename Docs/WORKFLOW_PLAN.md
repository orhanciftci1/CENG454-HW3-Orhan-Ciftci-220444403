# GitHub Workflow Plan

Use this to satisfy the traceability requirement. Do the work in small commits even if the files are already prepared.

## Suggested Board Columns

- Backlog
- Ready
- In Progress
- Review / Playtest
- Done

## Suggested Issues

- Build player movement and objective health
- Add observer events for core damage and game result
- Add weapon strategy assets
- Add enemy movement strategies and wave pressure
- Add projectile object pool
- Add damage boost decorator
- Write PDF report and screenshot evidence

## Suggested Branches and Commits

1. `feat/core-loop`
   - Commit: `Add defender, core health, and win lose state`
   - Open PR and merge after playtesting.
2. `feat/strategy-observer`
   - Commit: `Add observer events and strategy based weapons`
   - Open PR and merge after checking Inspector references.
3. `feat/pool-decorator`
   - Commit: `Add projectile pool and damage boost decorator`
   - Open PR and merge after firing projectiles in play mode.
4. `chore/report`
   - Commit: `Add report draft and screenshot checklist`

Do not make one single final commit. The assignment warns that this can raise a plagiarism flag.
