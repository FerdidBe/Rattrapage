using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.HID;

public enum AIState
{
    Patrol,
    Poursuit
}

public class AIController : MonoBehaviour
{
    // variables éditeur
    [SerializeField] private Transform markersListRef;
    [SerializeField] private bool loop = false;
    [SerializeField] private float detectTimer = 2f, poursuitTimer = 10f;
    [SerializeField] private Transform eyesPosition;
    [SerializeField] private LayerMask layerMask;

    // Navigation IA
    private NavMeshAgent AI;

    // Points de marquage suivis par l'IA
    private Transform[] markersList;
    private int targetMarker = 0;
    private int direction = 1;

    // Position du joueur suivi
    private Transform playerTarget;
    private bool detectPlayer = false;

    // Etat de la patrouille
    private AIState aiState = AIState.Patrol;

    private void Start()
    {
        // Récupération de la navigation IA
        AI = transform.GetComponent<NavMeshAgent>();
        
        // Récupération des points de marquage
        markersList = markersListRef.GetComponentsInChildren<Transform>();

        // Destination de l'IA au début
        AI.SetDestination(markersList[targetMarker].position);
    }

    private void Update()
    {
        // Choix de la fonction selon l'état de la patrouille
        if (aiState == AIState.Patrol)
        {
            Patrol();
        }
        else if (aiState == AIState.Poursuit)
        {
            Poursuit();
        }
    }

    // Fonction de patrouille
    private void Patrol()
    {
        if ((AI.transform.position - AI.destination).magnitude < 0.1f)
        {
            targetMarker += direction;
            if (targetMarker > markersList.Length - 1)
            {
                if (loop) targetMarker = 0;
                else
                {
                    direction *= -1;
                    targetMarker = markersList.Length - 2;
                }
            }
            if(targetMarker < 0)
            {
                if (loop) targetMarker = markersList.Length - 1;
                else
                {
                    direction *= -1;
                    targetMarker = 0;
                }
            }
        }

        AI.SetDestination(markersList[targetMarker].position);
    }

    // Fonction de poursuite
    private void Poursuit()
    {
        if (detectPlayer)
        {
            AI.SetDestination(playerTarget.position);
        }
    }

    private void DetectPlayer(PlayerControler playerControler)
    {
        playerTarget = playerControler.transform;

        aiState = AIState.Poursuit;
        detectPlayer = true;

        StopAllCoroutines();
        StartCoroutine(TimerDetect());
        StartCoroutine(TimerPoursuit());
    }
    
    private IEnumerator TimerDetect()
    {
        yield return new WaitForSeconds(detectTimer);

        detectPlayer = false;
    }
    private IEnumerator TimerPoursuit()
    {
        yield return new WaitForSeconds(poursuitTimer);

        aiState = AIState.Patrol;
    }

    public void OnPlayerDetected(PlayerControler playerControler)
    {
        Debug.Log("player triggered");

        Debug.DrawRay(eyesPosition.position, playerControler.GetEyesPosition() - eyesPosition.position);

        // Si le joueur est détecté par le cone, il faut vérifier qu'il n'est pas derrière un obstacle
        if (Physics.Raycast(eyesPosition.position, playerControler.GetEyesPosition() - eyesPosition.position, out var hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform.TryGetComponent<PlayerControler>(out playerControler))
            {
                DetectPlayer(playerControler);
                Debug.Log("Detected Player");
            }
        }
    }
}
