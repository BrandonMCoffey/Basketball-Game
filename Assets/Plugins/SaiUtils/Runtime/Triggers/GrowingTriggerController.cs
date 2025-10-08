using DG.Tweening;
using SaiUtils.Extensions;
using UnityEngine;

namespace SaiUtils.Triggers
{
    [RequireComponent(typeof(TriggerController))]
    public class GrowingTriggerController : TriggerController
    {
        [SerializeField] float _finalVisionRadius = 10f;

        public void Initialize(float finalRadius, float existTime)
        {
            _finalVisionRadius = finalRadius;

            // set the scale of the trigger to 0.1f
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            transform.DOScale(_finalVisionRadius, existTime).SetEase(Ease.Linear);

            // set the trigger to destroy itself after the exist time
            Destroy(gameObject, existTime + 3f);
        }

       
    }
}