using System;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Utils
{
    public static class NumberAnimator
    {
        public static void AnimateNumber(int fromValue, int toValue, float duration, Action<int> onUpdate)
        {
            if (onUpdate == null) return;
             
            DOTween.Kill("NumberAnimation");
             
            DOTween.To(
                () => fromValue,
                x => onUpdate.Invoke(Mathf.RoundToInt(x)),
                toValue,
                duration
            )
            .SetEase(Ease.OutQuart)
            .SetId("NumberAnimation");
        }
        
        public static void AnimateNumberyWithCallback(int fromValue, int toValue, float duration, 
            Action<int> onUpdate, Action onComplete = null)
        {
            if (onUpdate == null) return;
            
            DOTween.Kill("NumberAnimation");
            
            DOTween.To(
                () => fromValue,
                x => onUpdate.Invoke(Mathf.RoundToInt(x)),
                toValue,
                duration
            )
            .SetEase(Ease.OutQuart)
            .SetId("NumberAnimation")
            .OnComplete(() => onComplete?.Invoke());
        }
        
        public static void AnimateNumberWithScale(int fromValue, int toValue, float duration,
            Action<int> onUpdate, Transform scaleTarget = null)
        {
            if (onUpdate == null) return;
            
            DOTween.Kill("NumberAnimation");
            DOTween.Kill("NumberScale");
            
            // Value animation
            DOTween.To(
                () => fromValue,
                x => onUpdate.Invoke(Mathf.RoundToInt(x)),
                toValue,
                duration
            )
            .SetEase(Ease.OutQuart)
            .SetId("NumberAnimation");
            
            // Scale animation
            if (scaleTarget != null)
            {
                scaleTarget.DOPunchScale(Vector3.one * 0.1f, duration * 0.5f, 5, 0.5f)
                    .SetEase(Ease.OutBounce)
                    .SetId("NumberScale");
            }
        }
    }
}