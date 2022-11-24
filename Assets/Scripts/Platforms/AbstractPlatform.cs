using System;

using DG.Tweening;

using UnityEngine;


public abstract class AbstractPlatform : MonoBehaviour
{
    [SerializeField] private float _kickingDuration = 1.5f;
    [SerializeField] private float _kickingYEndPostion = -5f;

    [SerializeField] private PointsInfo _nextPlatformPositions;
    public PointsInfo NextPlatformPositions => ConvertPoints(_nextPlatformPositions);

    public int ID => id;
    private int id = -1;
    
    private static int _id = -1;

    private void Awake()
    {
        id = ++_id;
    }

    public virtual void Kick(Action<AbstractPlatform> afterKickAction)
    {
        transform
            .DOMoveY(_kickingYEndPostion, _kickingDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => { afterKickAction?.Invoke(this); })
            .Play();
    }
    
    protected PointsInfo ConvertPoints(PointsInfo points)
    {
        var matrix = transform.localToWorldMatrix;

        points.firsPoint = matrix.MultiplyPoint3x4(points.firsPoint);
        points.secondPoint = matrix.MultiplyPoint3x4(points.secondPoint);

        return points;
    }

    public virtual void OnDrawGizmos()
    {
        var rad = transform.localScale.x / 10;

        var nextPoints = ConvertPoints(_nextPlatformPositions);

        Gizmos.color = Color.cyan;

        Gizmos.DrawSphere(nextPoints.firsPoint, rad);
        Gizmos.DrawSphere(nextPoints.secondPoint, rad);
    }
}