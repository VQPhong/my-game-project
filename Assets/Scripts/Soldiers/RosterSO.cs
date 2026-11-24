using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Soldiers
{
    [CreateAssetMenu(fileName = "New Roster", menuName = "Game/Roster")]
    public class RosterSO : ScriptableObject
    {
        [SerializeField] private List<SoldierSO> soldiers = new List<SoldierSO>();

        public IReadOnlyList<SoldierSO> Soldiers => soldiers;

        private void OnValidate()
        {
            HashSet<SoldierSO> seen = new HashSet<SoldierSO>();
            HashSet<string> seenNames = new HashSet<string>();

            foreach (SoldierSO soldier in soldiers)
            {
                if (soldier == null)
                {
                    Debug.LogWarning($"{name}: soldiers contains an empty slot", this);
                    continue;
                }

                if (!seen.Add(soldier))
                    Debug.LogWarning($"{name}: {soldier.name} is listed more than once", this);

                if (!string.IsNullOrWhiteSpace(soldier.DisplayName) && !seenNames.Add(soldier.DisplayName))
                    Debug.LogWarning($"{name}: display name '{soldier.DisplayName}' is used by more than one soldier", this);
            }
        }
    }
}
