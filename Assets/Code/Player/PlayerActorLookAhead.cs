using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code.Player
{
	public class PlayerActorLookAhead : MonoBehaviour
	{

		PlayerActor actor;
		public Vector2 damping = new Vector2(2f, 8f);
		
		void Start ()
		{
			actor = transform.parent.GetComponent<PlayerActor>();
		}
		
		void Update ()
		{
			var pos = transform.localPosition;
			pos.x = actor.velocity.x / damping.x;
			pos.y = actor.velocity.y / damping.y;
			transform.localPosition = pos;	
		}

	}

}