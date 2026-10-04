using UnityEngine;

namespace Assets.Scripts.Weapons
{

    [System.Serializable]
    public struct IntRange
    {
        public int min;
        public int max;

        public int Roll() => Random.Range(min, max + 1);
        public bool IsValid() => min <= max;
    }

}
