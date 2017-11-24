using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code.Player
{
	public class PlayerActorLookAhead : MonoBehaviour
	{

		PlayerActor actor;
		public float damping = 10f;
		
		void Start ()
		{
			actor = transform.parent.GetComponent<PlayerActor>();
		}
		
		void Update ()
		{
			var pos = transform.localPosition;
			pos.x = actor.velocity.x / damping;
			transform.localPosition = pos;	
		}

	}

}