using UnityEngine;
using System.Collections;

namespace Assets.Code.Interactive
{

    public interface IForceReceiver
    {

        void ReceiveForce(Vector3 force, bool doubleOpposingForce = false);

    }


}