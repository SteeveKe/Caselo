using System;
using DG.Tweening;
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

            if (Input.GetKeyDown(KeyCode.Q))
            {
                TakeDamage(1);
            }
        }

        public void TakeDamage(float dmg)
        {
            life -= dmg;
            transform.DOComplete();
            transform.DOShakePosition(0.5f, 0.1f);
            transform.DOShakeScale(0.5f, 0.1f);
            transform.DOShakeRotation(0.5f, 10f);
        }
    }
}
