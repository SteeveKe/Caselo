using Unity.Cinemachine;
using UnityEngine;

namespace Player
{
    public class PlayerRotation : MonoBehaviour
    {
        private Transform _cameraRotation;
        
        void Start()
        {
            _cameraRotation = FindFirstObjectByType<CinemachineBrain>().gameObject.transform;
        }
        
        void FixedUpdate()
        {
            transform.rotation = Quaternion.Euler(0f, _cameraRotation.eulerAngles.y, 0f);
        }
    }
}
