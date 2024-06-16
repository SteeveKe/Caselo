using Cinemachine;
using UnityEngine;

namespace Player
{
    public class PlayerRotation : MonoBehaviour
    {
        private Transform _cameraRotation;
        
        void Start()
        {
            _cameraRotation = FindObjectOfType<CinemachineBrain>().gameObject.transform;
        }
        
        void Update()
        {
            transform.rotation = Quaternion.Euler(0f, _cameraRotation.eulerAngles.y, 0f);
        }
    }
}
