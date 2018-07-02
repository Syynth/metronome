extern alias mscorlib;
using mscorlib::System.Threading.Tasks;

using UnityEngine;

using Assets.Code.References;
using Assets.Code.Zones;

namespace Assets.Code
{
    public class SceneEntrance : MonoBehaviour
    {

        public SceneVariable Target;
        public GameState.GameState GameState;

        public SharedObjectReference ZoneLoadingManagerReference;

        ZoneLoadingManager ZoneLoadingManager
        {
            get
            {
                return ZoneLoadingManagerReference?.Value as ZoneLoadingManager;
            }
        }

        public bool TransitionImmediately = true;

        public async Task Enter()
        {
            GameState.CurrentScene = Target;
            GameState.Save();
            await ZoneLoadingManager.EnterZone(Target.LoadingZone, Target);
        }

    }
}
