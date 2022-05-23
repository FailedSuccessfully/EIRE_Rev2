using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public static class AttackUtilities
{
    public static class Movement
    {
        public static void MoveForward(Transform transform, float speed) => transform.Translate(transform.right * speed * Time.deltaTime);

        // TODO: change to only accept spelldrivers
        public static void RideSpline(GameObject gameObject, SplineComputer spline)
        {
            SplineFollower sFollower = gameObject.TryGetComponent<SplineFollower>(out SplineFollower component) ? component : gameObject.AddComponent<SplineFollower>();
            sFollower.motion.is2D = true;
            sFollower.spline = spline;

            //TODO: no hard coding k?
            sFollower.followSpeed = 5f;
            sFollower.useTriggers = true;
            sFollower.follow = true;

        }

        public static void DetachSpline(SplineFollower sFollower)
        {
            sFollower.follow = false;
            sFollower.enabled = false;
        }
    }

    public static class Splines
    {
        public static SplinePoint[] CalculatePoints(float baseRadius, Vector3 target)
        {
            float radius = baseRadius;

            SplinePoint[] value = new SplinePoint[3];
            value[0] = new SplinePoint(Vector3.zero);

            float angle = Random.Range(0, 180);
            Vector3 midPoint = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * radius;
            value[1] = new SplinePoint(midPoint);

            Vector3 originToTargetIntersect = Vector3.ClampMagnitude(target, radius);
            //TODO: Ratio math is bad
            float ratio = (originToTargetIntersect - midPoint).magnitude / (target - midPoint).magnitude;
            Vector3 endPoint = Vector3.Slerp(originToTargetIntersect, midPoint, ratio);
            Debug.Log(originToTargetIntersect);
            Debug.Log((target));
            value[2] = new SplinePoint(endPoint);

            return value;
        }
    }
    public static class Targeting { }
}
