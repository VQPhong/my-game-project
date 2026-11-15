using Assets.Scripts.Grids;
using Assets.Scripts.Loadouts;
using Assets.Scripts.Soldiers;
using Assets.Scripts.Squads;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Battles
{
    public class BattleSetup : MonoBehaviour
    {
        [SerializeField] private RosterSO roster;
        [SerializeField] private LoadoutOptionsSO options;

        [Tooltip("Soldier i spawns at point i")]
        [SerializeField] private List<Transform> deployPoints = new List<Transform>();


        // Start, not Awake: every singleton sets Instance in Awake, and all Awakes finish before any Start
        private void Start()
        {
            if (!HasValidWiring()) return;

            SquadLoadout squad = ResolveSquad();
            DeploySquad(squad);
        }
        private void OnValidate()
        {
            //// No need for now
            //if (options != null && deployPoints.Count < options.MaxSquadSize)
            //    Debug.LogWarning($"Squad size is not valid", this);
        }

        private bool HasValidWiring()
        {
            bool isValid = true;

            if (roster == null || options == null)
            {
                Debug.LogError($"{name}: assign roster and options in the Inspector", this);
                isValid = false;
            }

            if (UnitSpawner.Instance == null)
            {
                Debug.LogError($"{name}: no UnitSpawner in the scene", this);
                isValid = false;
            }

            if (LevelGrid.Instance == null)
            {
                Debug.LogError($"{name}: no LevelGrid in the scene", this);
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Session is read only: the default squad is not written back (it is not the player's choice),
        /// and the session is not cleared (the Loadout scene can show the last squad again).
        /// </summary>
        private SquadLoadout ResolveSquad()
        {
            bool fromSession = LoadoutSession.HasSquad;

            SquadLoadout squad = fromSession
                                ? LoadoutSession.Current
                                : SquadLoadout.CreateDefault(roster, options);

            Debug.Log($"[BattleSetup] {(fromSession ? "squad from session" : "default squad")} ({squad.Members.Count})", this);
            return squad;
        }

        private void DeploySquad(SquadLoadout squad)
        {
            if (squad.IsEmpty)
            {
                Debug.LogWarning($"{name}: squad is empty", this);
                return;
            }

            HashSet<GridPosition> usedCells = new HashSet<GridPosition>();
            int deployedCount = 0;

            for (int i = 0; i < squad.Members.Count; i++)
            {
                SoldierLoadout loadout = squad.Members[i];

                // Skip only this soldier, the rest still deploy
                if (i >= deployPoints.Count || deployPoints[i] == null)
                {
                    Debug.LogWarning($"{name}: no deploy point {i} for {loadout.Soldier.DisplayName}", this);
                    continue;
                }

                GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(deployPoints[i].position);

                // Units register to LevelGrid next frame, so same-cell spawns in this loop must be caught here
                if (!usedCells.Add(gridPosition))
                {
                    Debug.LogWarning($"{name}: {deployPoints[i].name} shares cell {gridPosition} with another deploy point", deployPoints[i]);
                    continue;
                }

                if (!UnitSpawner.Instance.TrySpawnSoldier(loadout, gridPosition, out _))
                {
                    Debug.LogWarning($"{name}: failed to deploy {loadout.Soldier.DisplayName} at {gridPosition} (outside grid or occupied?)", deployPoints[i]);
                    continue;
                }

                deployedCount++;
            }

            Debug.Log($"[BattleSetup] deployed {deployedCount}/{squad.Members.Count}", this);
        }
    }
}