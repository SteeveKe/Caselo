using System;
using UnityEngine;

namespace Game.Nexus
{
    public class NexusManager : MonoBehaviour
    {
        [SerializeField] private float initLife;
        [SerializeField] private float life;

        private void Start()
        {
            life = initLife;
        }

        private void Update()
        {
            if (life <= 0)
            {
                Debug.Log("Game Over");
            }
        }

        public void TakeDamage(float dmg)
        {
            life -= dmg;
        }
    }
}
