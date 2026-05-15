using System;
using UnityEngine;

namespace Core
{
    public class TriggerNotifier : MonoBehaviour
    {
        public Action<Collider2D> OnEntered;
        public Action<Collider2D> OnExited;
        private void OnTriggerEnter2D(Collider2D other) => OnEntered?.Invoke(other);
        private void OnTriggerExit2D(Collider2D other)  => OnExited?.Invoke(other);
    }
}
