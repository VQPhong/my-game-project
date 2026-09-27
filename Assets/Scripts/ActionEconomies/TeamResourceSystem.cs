using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ActionEconomies
{
    public class TeamResourceSystem : MonoBehaviour
    {
        public static TeamResourceSystem Instance { get; private set; }

        [SerializeField] private List<ResourceTypeSO> sharedResourceTypes;
        private ResourcePool resourcePool = new ResourcePool();

        private void Start()
        {
            TurnSystem.Instance.OnTurnChanged += (s, e) => resourcePool.ApplyRegen();
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There's more than one TeamResourceSystem! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;

            resourcePool.Initialize(sharedResourceTypes);
        }

        public ResourcePool ResourcePool => resourcePool;

        public List<ResourceTypeSO> GetTeamResourceTypes() => sharedResourceTypes;

    }
}
