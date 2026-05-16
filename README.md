# Core Breach

CENG454 HW3 prototype: a short defend-the-core action game built around Observer, Strategy, Object Pool, Interfaces, and Decorator.

## Setup

1. Open this folder as a Unity 6 project.
2. Wait for scripts to compile.
3. Run `Tools > Core Breach > Build Prototype Scene`.
4. Open `Assets/Scenes/CoreBreachPrototype.unity`.
5. Press Play.

## Controls

- Move: WASD or Arrow Keys
- Aim: Mouse
- Fire: Left Mouse Button
- Weapon mode: keys `1` and `2`
- Decorator boost: key `Q`

## Required Evidence Map

- Interfaces: `IDamageable`, `ITargetProvider`, `IWeaponStrategy`, `IEnemyMovementStrategy`, `IPoolable`
- Observer: `CoreHealth.OnHealthChanged`, `EnemyUnit.OnEnemyKilled`, `WaveSpawner.OnWaveCompleted`
- Strategy: `PlayerWeapon` uses `WeaponStrategyBase`; `EnemyUnit` uses movement strategies.
- Object Pool: `ProjectilePool` and `Projectile`
- Decorator: `DamageBoostWeaponDecorator`

## Submission Files

- Main playable scene: `Assets/Scenes/CoreBreachPrototype.unity`
- Report draft: `Docs/REPORT_DRAFT.md`
- PDF report template: `Docs/CENG454_HW3_Orhan_Ciftci_220444403_REPORT_TEMPLATE.pdf`
## Submission Notes

- Main playable scene: `Assets/Scenes/CoreBreachPrototype.unity`
- Report draft: `Docs/REPORT_DRAFT.md`
- PDF template: `Docs/CENG454_HW3_Orhan_Ciftci_220444403_REPORT_TEMPLATE.pdf`
