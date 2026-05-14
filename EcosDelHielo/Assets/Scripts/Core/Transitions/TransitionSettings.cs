using System;
using DG.Tweening;
using UnityEngine;

namespace Core.Transitions
{
    [Serializable]
    public class TransitionSettings
    {
        [Min(0f)] public float fadeOutDuration = 0.4f;
        [Min(0f)] public float fadeInDuration = 0.4f;
        [ColorUsage(false)] public Color overlayColor = Color.black;
        public Ease fadeOutEase = Ease.InQuad;
        public Ease fadeInEase = Ease.OutQuad;
    }
}
