using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ActionEconomies
{
    public class ResourcePool
    {
        private Dictionary<ResourceTypeSO, int> pool = new Dictionary<ResourceTypeSO, int>();

        public void Initialize(List<ResourceTypeSO> types)
        {
            foreach (ResourceTypeSO type in types)
            {
                pool[type] = type.maxAmount;
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
                switch (type.regenMode)
                {
                    case RegenMode.FullResetEachTurn:
                        pool[type] = type.maxAmount;
                        break;
                    case RegenMode.IncrementEachTurn:
                        pool[type] = Mathf.Min(type.maxAmount, pool[type] + type.regenAmount);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
