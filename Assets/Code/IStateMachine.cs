using UnityEngine;
using System.Collections.Generic;


namespace Assets.Code
{

    public interface IStateMachine<T> where T : MonoBehaviour, IStateMachine<T>
    {

        List<ActorState<T>> States { get; }

        ActorState<T> CurrentState { get; set; }
        ActorState<T> PreviousState { get; set; }
        ActorState<T> NextState { get; set; }

        void ChangeState(ActorState<T> nextState);

    }


}