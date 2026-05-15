using System;
using UnityEngine;

namespace Core
{
    public class TriggerNotifier : MonoBehaviour
    {
        public Action<Collider2D> OnEntered;
        private void OnTriggerEnter2D(Collider2D other) => OnEntered?.Invoke(other);
    }
}
