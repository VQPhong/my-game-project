using Assets.Scripts.UIs;
using UnityEngine;

namespace Assets.Scripts.ActionEconomies
{
    [CreateAssetMenu(fileName = "New Resource Type", menuName = "Resource/Resource Type")]
    public class ResourceTypeSO : ScriptableObject
    {
        [Tooltip("For UI display")]
        public string resourceName;

        [Tooltip("Max unit held by default")]
        public int maxAmount;

        [Tooltip("Is this resource shared team-wide")]
        public bool isShared;

        [Tooltip("How does it regen?")]
        public RegenMode regenMode;

        [Tooltip("In case of regenMode is Increment Each Turn, how much does it regen per turn?")]
        public int regenAmount;

        [Header("UI")]
        [Tooltip("Where this resource is shown")]
        public ResourceDisplay display;

        [Tooltip("Pip prefab used when display is WorldPips")]
        public ResourcePipUI pipPrefab;

    }

    public enum RegenMode
    {
        FullResetEachTurn,
        IncrementEachTurn,
        Manual
    }

    public enum ResourceDisplay
    {
        Hidden,
        WorldPips,
        ActionButton
    }

}
