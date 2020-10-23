using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiHitRay
{
    public static List<Vector3> CheckMultiHitRay(Vector3 position, Vector3 direction, float length, LayerMask mask)
    {
        List<Vector3> list = new List<Vector3>();

        //   双方向Rayで大雑把な形状を取得
        RaycastHit[] foward = Physics.RaycastAll(position, direction, length, mask);
        RaycastHit[] back = Physics.RaycastAll(position + direction * length, direction * -1, length, mask);

        // 外枠のポイント一覧を登録
        foreach (RaycastHit hit in foward)
        {
            list.Add(hit.point);
        }
        foreach (RaycastHit hit in back)
        {
            list.Add(hit.point);
        }

        // 内部のコライダーを取得
        foreach (RaycastHit fowardHit in foward)
        {
            foreach (RaycastHit backHit in back)
            {
                if (fowardHit.collider == backHit.collider)
                {

                    float maxDistance = Vector3.Distance(fowardHit.point, backHit.point);

                    CheckInnerObject(fowardHit.collider, fowardHit.point, direction, maxDistance, list);
                    CheckInnerObject(fowardHit.collider, backHit.point, direction * -1, maxDistance, list);
                }
            }
        }
        return list;
    }

    public static void CheckInnerObject(Collider hit, Vector3 currentPoint, Vector3 direction, float distance, List<Vector3> list)
    {
        RaycastHit inMeshHit;

        while (hit.Raycast(new Ray(currentPoint + direction * 0.1f, direction), out inMeshHit, distance))
        {
            list.Add(inMeshHit.point);
            currentPoint = inMeshHit.point;
            distance -= inMeshHit.distance;
        }
    }
}