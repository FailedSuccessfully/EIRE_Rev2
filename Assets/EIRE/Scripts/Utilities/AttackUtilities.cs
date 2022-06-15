using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public static class AttackUtilities
{
    public static class Movement
    {
        public static void MoveForward(Transform transform, float speed) => transform.Translate(transform.right * speed * Time.deltaTime);

        public static void RideSpline(SpellDriver driver, SplineComputer spline)
        {
            GameObject obj = driver.activeObject;
            SplineFollower sFollower = obj.TryGetComponent<SplineFollower>(out SplineFollower component) ? component : obj.AddComponent<SplineFollower>();
            sFollower.motion.is2D = true;
            sFollower.spline = spline;

            sFollower.followSpeed = driver.MountContext.speed * driver.MountContext.splineData.SpeedMultiplyer;
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

            float angle = Random.Range(0, 360);
            Vector3 endPoint = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * radius;
            value[2] = new SplinePoint(endPoint);

            Vector3 midPoint = new Vector3((endPoint.x - target.x) * 0.5f, (target.y + endPoint.y) * 2, 0);

            value[1] = new SplinePoint(Vector3.ClampMagnitude(midPoint, baseRadius * 2));

            return value;
        }
    }
    public static class Targeting { }
}
