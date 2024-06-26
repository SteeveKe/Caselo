namespace Enemies.Target
{
    public class EnemyPlayerTarget : EnemyTargetable
    {
        protected override void Init()
        {
            base.Init();
            GameManager.AddPlayer(this);
        }
    }
}
