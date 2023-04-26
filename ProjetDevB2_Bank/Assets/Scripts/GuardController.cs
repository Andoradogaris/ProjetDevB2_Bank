using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PossibleStates
{
    onPatrol,
    onShoot,
    onHide
}

public class GuardController : MonoBehaviour
{
    [Header("Switch States")]
    [SerializeField] private bool patrol;
    [SerializeField] private bool shoot;
    [SerializeField] private bool hide;

    public int detectorValue;

    [Header("Patrol")]
    private NavMeshAgent agent;
    [SerializeField] private List<GameObject> patrolPoints;
    public int patrolIndex;
    private PossibleStates state;

    [SerializeField] private float minPauseTime;
    [SerializeField] private float maxPauseTime;

    float currentPauseTime = 0f;
    float rdmLuckToStop;

    [Header("Hide")]
    [SerializeField] private List<GameObject> HidePoints;
    [SerializeField] private List<GameObject> ValidHidePoints;
    private int index;
    [SerializeField] private LayerMask layermask;
    private float distance;
    [SerializeField] private GameObject player;
    private bool isHidden;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        state = PossibleStates.onPatrol;
        if (patrolPoints.Count > 0)
        {
            agent.SetDestination(patrolPoints[patrolIndex].transform.position);
        }
    }

    private void Update()
    {
        switch (state)
        {
            case PossibleStates.onPatrol:
                isHidden = false;
                Patrol();
                break;
            case PossibleStates.onShoot:
                isHidden = false;
                break;
            case PossibleStates.onHide:
                if (!isHidden)
                {
                    Hide();
                }
                break;
        }

        if (patrol)
        {
            shoot = false;
            hide = false;
            state = PossibleStates.onPatrol;
        }

        if (shoot)
        {
            patrol = false;
            hide = false;
            state = PossibleStates.onShoot;
        }

        if (hide || detectorValue == 100)
        {
            hide = true;
            patrol = false;
            shoot = false;
            state = PossibleStates.onHide;
        }


        /*if (player.GetComponent<PlayerController>().isHidden && Vector3.Distance(player.transform.position, transform.position) < distanceToHide)
        {

        }*/
    }

    private void Patrol()
    {
        if (agent.remainingDistance < agent.stoppingDistance)
        {
            if (currentPauseTime > 0f)
            {
                currentPauseTime -= Time.deltaTime;
            }
            else
            {
                rdmLuckToStop = Random.Range(0, 100);
                if(rdmLuckToStop < 30)
                {
                    currentPauseTime = Random.Range(minPauseTime, maxPauseTime);
                }
                patrolIndex++;

                if (patrolIndex >= patrolPoints.Count)
                {
                    patrolIndex = 0;
                }

                agent.SetDestination(patrolPoints[patrolIndex].transform.position);
            }
        }
    }

    private void Hide()
    {
        for (int i = 0; i < HidePoints.Count; i++)
        {
            distance = Vector3.Distance(player.transform.position, HidePoints[i].transform.position);
            RaycastHit hit;

            if (Physics.Raycast(player.transform.position, HidePoints[i].transform.position - player.transform.position, out hit, distance))
            {
                ValidHidePoints.Add(HidePoints[i]);
            }
        }

        distance = 0;

        for (int i = 0; i < ValidHidePoints.Count; i++)
        {
            if (distance > Vector3.Distance(transform.position, ValidHidePoints[i].transform.position))
            {
                distance = Vector3.Distance(transform.position, ValidHidePoints[i].transform.position);
                index = i;
            }
        }

        agent.SetDestination(ValidHidePoints[index].transform.position);

        isHidden = true;
    }
}
