using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code.Player
{
	public class PlayerActorLookAhead : MonoBehaviour
	{

		PlayerActor actor;
		public Vector2 damping = new Vector2(2f, 8f);
        public Vector2 maxLookahead = new Vector2(6, 12);
		
		void Start ()
		{
			actor = transform.parent.GetComponent<PlayerActor>();
		}
		
		void Update ()
		{
			var pos = transform.localPosition;
			pos.x = actor.velocity.x / damping.x;
			pos.y = actor.velocity.y / damping.y;
            pos.x = Mathf.Sign(pos.x) * Mathf.Min(maxLookahead.x, Mathf.Abs(pos.x));
            pos.y = Mathf.Sign(pos.y) * Mathf.Min(maxLookahead.y, Mathf.Abs(pos.y));
            transform.localPosition = pos;	
		}

	}

}