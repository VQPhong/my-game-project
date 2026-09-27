using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Grids
{
    public class GridSystemVisualSingle : MonoBehaviour
    {

        [SerializeField] private MeshRenderer meshRenderer;


        public void Show()
        {
            meshRenderer.enabled = true;
        }

        public void Hide()
        {
            meshRenderer.enabled = false;
        }

    }
}
