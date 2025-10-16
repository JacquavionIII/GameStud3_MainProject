using DG.Tweening;
using UnityEngine;

public class TrailSplineAnim : MonoBehaviour
{
    [SerializeField] private Transform trailVFX;
    [SerializeField] private Transform homePoint;
    [SerializeField] public float trailDuration = 3f;
    void Start()
    {
        if (homePoint != null)
        {
            trailVFX.DOMove(homePoint.position, trailDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            Debug.LogWarning("HomePoint is fucking out lol.");
        }
        //transform.DOMove(new Vector3(10, 0, 0), trailDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
}
