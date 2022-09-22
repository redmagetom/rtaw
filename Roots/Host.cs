using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Host : MonoBehaviour
{
    public GameplayManager gpm;
    public NavMeshAgent _nma;
    public bool busy;
    public enum HostState{Standby, GreetingPatron, TakingPatronToSeat}
    public HostState hostState;
    public Patron targetedPatron;
    public List<Patron> tiedPatrons;

    void FixedUpdate(){
        GreetPatrons();


    }


    private void GreetPatrons(){
        if(hostState == Host.HostState.GreetingPatron){
            if(_nma.remainingDistance <= _nma.stoppingDistance){
                if(!_nma.hasPath || _nma.velocity.sqrMagnitude == 0){
                    if(hostState != HostState.TakingPatronToSeat)
                    hostState = HostState.TakingPatronToSeat;
                    targetedPatron.targetedHost = this;
                    targetedPatron.greetingState = Patron.GreetingStates.FollowingHost;
                    targetedPatron._nma.destination = gameObject.transform.position;

                    // find blank seat
                    _nma.destination = gpm.ri.allSeating.chairs[0].transform.position;
                }
            }
        }
    }

    void Start(){
        _nma = gameObject.GetComponent<NavMeshAgent>();
    }
}
