using UnityEngine;

namespace T2FToolKit
{
    public partial class T2FToolKit : MonoBehaviour
    {
        private void Awake()
        {
            OnAwake();
        }

        private void Start()
        {
            OnStart();
        }

        private void Update()
        {
            OnUpdate();
        }

        private void OnDestroy()
        {
            OnTerminate();
        }
    }
}