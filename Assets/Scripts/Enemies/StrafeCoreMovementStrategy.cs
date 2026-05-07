using CoreBreach.Interfaces;
using UnityEngine;

namespace CoreBreach.Enemies
{
    public sealed class StrafeCoreMovementStrategy : MonoBehaviour, IEnemyMovementStrategy
    {
        [SerializeField] private float orbitWeight = 0.55f;

        public void Move(EnemyUnit enemy, float deltaTime)
        {
            Vector2 toTarget = (enemy.Target.position - enemy.transform.position).normalized;
            Vector2 tangent = new(-toTarget.y, toTarget.x);
            Vector2 direction = (toTarget + tangent * orbitWeight).normalized;
            enemy.MoveBy(direction * enemy.MoveSpeed * deltaTime);
        }
    }
}
