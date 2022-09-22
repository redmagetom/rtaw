using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Patron : MonoBehaviour
{
    [Header("Greetings")]
    public bool greeted;
    public enum ArchState{Waiting, Seating, Ordering, Eating, Finished}
    public ArchState archstate;
    public enum GreetingStates{Standby, ReadyToBeGreeted, BeingGreeted, FollowingHost}
    public GreetingStates greetingState;
    public bool busy;
    public NavMeshAgent _nma;
    public Host targetedHost;

    void Start(){
        _nma = gameObject.GetComponent<NavMeshAgent>();
    }
    void Update(){
        if(greetingState == GreetingStates.FollowingHost){
            _nma.destination = targetedHost.transform.position;
        }
        if(!greeted && greetingState == GreetingStates.Standby){
            if(_nma.remainingDistance <= _nma.stoppingDistance){
                if(!_nma.hasPath || _nma.velocity.sqrMagnitude == 0){
                    greetingState = GreetingStates.ReadyToBeGreeted;
                }
            }
        }
    }
}
