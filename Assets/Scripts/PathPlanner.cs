using UnityEngine;
using System.Collections.Generic;

public class PathPlanner
{
    public struct PathSegment
    {
        public Vector3 startPos;
        public Vector3 endPos;
        public float speed;
        public float turnRadius;
    }

    public static List<PathSegment> CalculateOptimalPath(
        Vector3 startPos,
        Vector3 startDir,
        Vector3 endPos,
        float maxSpeed,
        float turnSpeed)
    {
        List<PathSegment> segments = new List<PathSegment>();

        // 시작점에서 도착점까지의 각도 계산
        Vector3 targetDir = (endPos - startPos).normalized;
        float totalAngle = Vector3.SignedAngle(startDir, targetDir, Vector3.up);

        // 회전 반경 계산
        float turnRadius = maxSpeed / (turnSpeed * Mathf.Deg2Rad);

        if (Mathf.Abs(totalAngle) > 0.1f)
        {
            // 첫 번째 세그먼트: 초기 회전
            PathSegment turnSegment = new PathSegment
            {
                startPos = startPos,
                endPos = startPos + (Quaternion.Euler(0, totalAngle/2, 0) * startDir * turnRadius),
                speed = maxSpeed * 0.5f,
                turnRadius = turnRadius
            };
            segments.Add(turnSegment);

            // 두 번째 세그먼트: 직진
            PathSegment straightSegment = new PathSegment
            {
                startPos = segments[0].endPos,
                endPos = endPos,
                speed = maxSpeed,
                turnRadius = 0
            };
            segments.Add(straightSegment);
        }
        else
        {
            // 직진만 필요한 경우
            PathSegment straightSegment = new PathSegment
            {
                startPos = startPos,
                endPos = endPos,
                speed = maxSpeed,
                turnRadius = 0
            };
            segments.Add(straightSegment);
        }

        return segments;
    }
}