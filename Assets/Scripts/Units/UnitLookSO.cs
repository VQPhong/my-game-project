using UnityEngine;

namespace Assets.Scripts.Units
{
    [CreateAssetMenu(fileName = "New Unit Look", menuName = "Game/Unit Look")]
    public class UnitLookSO : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField] private Material material;

        public string DisplayName => displayName;
        public Material Material => material;

        private void OnValidate()
        {
            if (material == null)
                Debug.LogWarning($"{name}: material is not assigned", this);
        }
    }
}
