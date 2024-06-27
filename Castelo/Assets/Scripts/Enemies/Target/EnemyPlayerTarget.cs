using UnityEngine;

namespace Enemies.Target
{
    public class EnemyPlayerTarget : EnemyTargetable
    {
        protected override void Init()
        {
            base.Init();
            GameManager.AddPlayer(this);
        }

        public override void TakeDamage(float damage)
        {
            //Debug.Log("player ouch");
        }
    }
}
