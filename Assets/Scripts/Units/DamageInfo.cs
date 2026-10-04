using Assets.Scripts.ActionEconomies;
using Assets.Scripts.Actions;
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
    }
}