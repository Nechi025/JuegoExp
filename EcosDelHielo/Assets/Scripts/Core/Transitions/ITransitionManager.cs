using System;
using Core.Services;

namespace Core.Transitions
{
    public interface ITransitionManager : IService
    {
        void FadeToBlack(Action onComplete = null);
        void FadeFromBlack(Action onComplete = null);
        void Transition(Action onMidpoint = null, Action onComplete = null);
    }
}
