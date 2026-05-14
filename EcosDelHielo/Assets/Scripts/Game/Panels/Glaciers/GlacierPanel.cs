using UnityEngine;

namespace Game.Panels.Glaciers
{
    public class GlacierPanel : MonoBehaviour
    {
        [SerializeField] private Glacier[] glaciers;

        private void Reset()
        {
            glaciers = GetComponentsInChildren<Glacier>();
        }
    }
}
