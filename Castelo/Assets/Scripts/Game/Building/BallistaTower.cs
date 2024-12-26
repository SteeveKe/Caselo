using System;
using UnityEngine;

namespace Game.Building
{
    public class BallistaTower : DefenseTower
    {
        protected override void TurnOnTarget()
        {
            base.TurnOnTarget();
            
            Vector3 dir = target.transform.position - partToFace.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            
            Vector3 targetFace = Quaternion.Lerp(partToFace.rotation, lookRotation, Time.deltaTime * pivotSpeed).eulerAngles;
            float face = targetFace.x < 180 ? Mathf.Clamp(targetFace.x, 0, faceClamp.x) : 
                Mathf.Clamp(targetFace.x, 360 - faceClamp.y, 360);
            
            partToFace.localRotation = Quaternion.Euler(face, 0f, 0f);
        }
    }
}
