using System.Collections.Generic;
using Assets.Scripts.ActionEconomies;
using UnityEngine;

namespace Assets.Scripts.Squads
{
    [CreateAssetMenu(fileName = "New Unit Config", menuName = "Squad/Unit Config")]
    public class UnitConfigSO : ScriptableObject
    {
        [Tooltip("Display name for this unit type/tier")]
        public string unitName;

        [Tooltip("Prefab to spawn - shared across tiers using same model/skills")]
        public Transform prefab;

        [Tooltip("Resource caps for THIS config - overrides ResourceTypeSO.maxAmount when spawned")]
        public List<ResourceAllotment> resourceAllotments;
    }
}
