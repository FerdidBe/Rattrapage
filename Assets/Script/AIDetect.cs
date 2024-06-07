using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDetect : MonoBehaviour
{
    [SerializeField] private AIController aiController;

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<PlayerControler>(out var playerControler))
        {
            aiController.OnPlayerDetected(playerControler);
        }
    }
}
