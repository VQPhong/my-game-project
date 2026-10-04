# Turn-Based Tactics

A turn-based tactical game prototype built in Unity, inspired by XCOM - for learning purpose.

The project started from Code Monkey's *Unity Turn-Based Strategy* course.

## Current features

**Core (following the course)**
- Grid system with grid visuals and grid-based mouse selection
- Unit selection, movement, and an action system with busy state
- Turn system: player turn / enemy turn
- Shooting, health system, and death ragdolls
- Camera movement, rotation, and zoom

**Extended beyond the course**
- **Multi-resource action economy.** Actions can cost several resource types (e.g. action points, move points, skill charges).
- **Weapon system.** `WeaponSO` holds weapon data (damage range, range, accuracy, impact type and force, projectile).
- **Improve ragdoll effect.** Ragdoll built into the unit rig and switched on at death instead of spawning a separate one.

## Project Structure

```
Assets/Scripts/
├── Actions/           BaseAction, Move/Shoot/Spin actions, ActionFactory
├── ActionEconomies/   Resource types, costs, pools, team resources
├── Grids/             Grid system, LevelGrid, grid visuals
├── Squads/            Unit configs, spawner, unit manager
├── Units/             Unit, health, animator, ragdoll, damage info
├── Weapons/           WeaponSO, Weapon, IntRange
├── Cameras/           Camera controller
└── UIs/               Action buttons, turn UI, resource UI
```

## Roadmap

- [ ] Complete the course
- [ ] Complete dynamic units spawning
- [ ] Squad Loadout
- [ ] Add HQ base
- [ ] More skills
- [ ] Multiple factions and allies beyond player vs. enemy
- [ ] Unit skins on a shared rig

## Credits

- Base course: *Unity Turn-Based Strategy* by Code Monkey: [Unity Turn Based Strategy](https://gamedev.tv/courses/unity-turn-based-strategy/)
- 3D assets: POLYGON Prototype by Synty Studios : [POLYGON - Prototype Pack - Art by Synty](https://assetstore.unity.com/packages/3d/environments/polygon-prototype-pack-art-by-synty-137126?srsltid=AU7gw4XYyAEP8GXB9HdDIk-tk5feDZWEL3OqpdN7NzxWjR0T5TnrQuP2)
