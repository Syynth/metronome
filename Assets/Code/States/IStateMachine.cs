using UnityEngine;
using System.Collections.Generic;


namespace Assets.Code.States
{

    public interface IStateMachine<T> where T : MonoBehaviour, IStateMachine<T>
    {

        ISet<ActorState<T>> States { get; }

        ActorState<T> CurrentState { get; set; }
        ActorState<T> PreviousState { get; set; }
        ActorState<T> NextState { get; set; }

        W GetState<W>() where W : ActorState<T>;
        void ChangeState<W>() where W : ActorState<T>;

    }


}