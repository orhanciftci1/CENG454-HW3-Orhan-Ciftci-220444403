using CoreBreach.Interfaces;
using UnityEngine;

namespace CoreBreach.Enemies
{
    public sealed class DirectCoreMovementStrategy : MonoBehaviour, IEnemyMovementStrategy
    {
        public void Move(EnemyUnit enemy, float deltaTime)
        {
            Vector2 direction = (enemy.Target.position - enemy.transform.position).normalized;
            enemy.MoveBy(direction * enemy.MoveSpeed * deltaTime);
        }
    }
}
