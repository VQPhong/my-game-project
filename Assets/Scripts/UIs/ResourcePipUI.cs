using UnityEngine;

namespace Assets.Scripts.UIs
{
    public class ResourcePipUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float spentAlpha = 0.25f;

        public void SetAvailable(bool available)
        {
            canvasGroup.alpha = available ? 1f : spentAlpha;
        }
    }
}
