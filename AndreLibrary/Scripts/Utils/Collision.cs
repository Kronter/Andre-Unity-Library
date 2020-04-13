using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andre.Utils
{
    public static class Collision
    {
        public static bool BulletColision(Vector3 previousPos, Vector3 currentPos, LayerMask collisionLayer, bool debug = false)
        {
            RaycastHit[] hits = Physics.RaycastAll(new Ray(previousPos, (currentPos - previousPos).normalized), (currentPos - previousPos).magnitude);
            if (hits.Length > 0)
                return true;
            if (debug)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    Debug.Log(hits[i].collider.gameObject.name);
                }
                Debug.DrawLine(currentPos, previousPos);
            }

            return false;
        }
    }
}
