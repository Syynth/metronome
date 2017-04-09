using UnityEngine;
using System.Collections;


namespace Assets.Code
{

    public interface IStateMachine<T> where T : MonoBehaviour, IStateMachine<T>
    {

        ActorState<T> CurrentState { get; set; }
        ActorState<T> PreviousState { get; set; }
        ActorState<T> NextState { get; set; }

        void ChangeState(ActorState<T> nextState);

    }


}