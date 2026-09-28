using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ActionEconomies
{
    public class ResourcePool
    {
        private Dictionary<ResourceTypeSO, int> pool = new Dictionary<ResourceTypeSO, int>();
        private Dictionary<ResourceTypeSO, int> maxAmounts = new Dictionary<ResourceTypeSO, int>();


        /// <summary>
        /// For default
        /// </summary>
        /// <param name="types"></param>
        public void Initialize(List<ResourceTypeSO> types)
        {
            foreach (ResourceTypeSO type in types)
            {
                pool[type] = type.maxAmount;
                maxAmounts[type] = type.maxAmount;
            }
        }

        /// <summary>
        /// For dynamic allotment
        /// </summary>
        /// <param name="allotments"></param>
        public void Initialize(List<ResourceAllotment> allotments)
        {
            foreach (ResourceAllotment allotment in allotments)
            {
                pool[allotment.resourceType] = allotment.maxAmount;
                maxAmounts[allotment.resourceType] = allotment.maxAmount;
            }
        }

        public int GetAmount(ResourceTypeSO type)
        {
            return pool.TryGetValue(type, out int amount) ? amount : 0;
        }

        public void Spend(ResourceTypeSO type, int amount)
        {
            if (pool.ContainsKey(type))
            {
                pool[type] -= amount;
            }
        }

        public void ApplyRegen()
        {
            List<ResourceTypeSO> keys = new List<ResourceTypeSO>(pool.Keys);

            foreach (ResourceTypeSO type in keys)
            {
                int maxAmount = maxAmounts[type];
                switch (type.regenMode)
                {
                    case RegenMode.FullResetEachTurn:
                        pool[type] = maxAmount;
                        break;
                    case RegenMode.IncrementEachTurn:
                        pool[type] = Mathf.Min(maxAmount, pool[type] + type.regenAmount);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
