using Assets.Scripts.ActionEconomies;
using Assets.Scripts.Actions;
using Assets.Scripts.Weapons;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Units
{
    [System.Serializable]
    public struct DamageInfo
    {
        public int amount;
        public Vector3 sourcePosition;
        public float impactForce;
        public ImpactType impactType; 
        public Vector3 impactPoint;
    }
}