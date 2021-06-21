using UnityEngine;
using Zenject;

public class ExampleBehaviour : MonoBehaviour
{
    [Inject] IExampleService m_service = default;

    void Start()
    {
        m_service.ExampleMethod();
    }
}
