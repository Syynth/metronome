using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Code.References;
using System.Threading.Tasks;

namespace Assets.Code.Zones
{

    public class ZoneLoadingManager : MonoBehaviour
    {

        public SharedObjectReference ZoneLoadingManagerReference;
        public SceneLoadingZone CurrentZone;

        private Tuple<HashSet<SceneVariable>, HashSet<SceneVariable>> OpenScenes = Tuple.Create(new HashSet<SceneVariable>(), new HashSet<SceneVariable>());
        private Tuple<HashSet<SceneVariable>, HashSet<SceneVariable>> LoadingScenes = Tuple.Create(new HashSet<SceneVariable>(), new HashSet<SceneVariable>());

        private HashSet<SceneVariable> CancelledScenes = new HashSet<SceneVariable>();

        private Dictionary<SceneVariable, Tuple<TaskCompletionSource<Tuple<Scene, SceneVariable>>, bool>> SceneTasks = new Dictionary<SceneVariable, Tuple<TaskCompletionSource<Tuple<Scene, SceneVariable>>, bool>>();

        private void Start()
        {
            var zlms = FindObjectsOfType<ZoneLoadingManager>();
            if (!zlms.Contains(ZoneLoadingManagerReference.Value))
            {
                ZoneLoadingManagerReference.Value = this;
                EnterZone(CurrentZone, forceEntry: true);
            }
        }

        private void OnDestroy()
        {
            var zlms = FindObjectsOfType<ZoneLoadingManager>();

            if (ZoneLoadingManagerReference.Value == this)
            {
                ZoneLoadingManagerReference.Value = zlms.SingleOrDefault(zlm => zlm != this);
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
        }

        private void LoadScene(SceneVariable scene, bool isParent)
        {
            Debug.Log($"Call to load scene {scene.Value}");
            var isLoading = LoadingScenes.Item2.Contains(scene);
            var isOpen = OpenScenes.Item2.Contains(scene);
            if (!isLoading && !isOpen)
            {
                if (SceneManager.GetSceneByPath(scene.Value).IsValid())
                {
                    Debug.Log($"Scene already loaded in Unity {scene.Value}");
                    if (isParent)
                    {
                        OpenScenes.Item1.Add(scene);
                    }
                    OpenScenes.Item2.Add(scene);
                }
                else
                {
                    Debug.Log($"Scene not already loaded or loading {scene.Value}");
                    scene.LoadAsync();
                    if (isParent)
                    {
                        LoadingScenes.Item1.Add(scene);
                    }
                    LoadingScenes.Item2.Add(scene);
                }
            }
            else if (isOpen && isParent)
            {
                OpenScenes.Item1.Add(scene);
            }
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var loadingScene = LoadingScenes.Item2.SingleOrDefault(var => var.Value == scene.path);
            if (loadingScene != null)
            {
                OpenScenes.Item1
                    .Where(openScene =>
                        openScene.ConnectedScenes.All(connectedScene => OpenScenes.Item1.Contains(connectedScene) || connectedScene == loadingScene)
                    )
                    .ToList()
                    .ForEach(openScene =>
                    {
                        if (SceneTasks.ContainsKey(openScene) && !SceneTasks[openScene].Item2)
                        {

                            SceneTasks[openScene].Item1.TrySetResult(Tuple.Create(scene, openScene));
                            SceneTasks[openScene] = Tuple.Create(SceneTasks[openScene].Item1, true);
                        }
                    });

                if (LoadingScenes.Item1.Contains(loadingScene))
                {
                    LoadingScenes.Item1.Remove(loadingScene);
                    OpenScenes.Item1.Add(loadingScene);
                    var l = new List<int>();
                    if (SceneTasks.ContainsKey(loadingScene) && !SceneTasks[loadingScene].Item2 && loadingScene.ConnectedScenes.All(connectedScene => OpenScenes.Item2.Contains(connectedScene)))
                    {
                        SceneTasks[loadingScene].Item1.SetResult(Tuple.Create(scene, loadingScene));
                        SceneTasks[loadingScene] = Tuple.Create(SceneTasks[loadingScene].Item1, true);
                    }
                }
                LoadingScenes.Item2.Remove(loadingScene);
                OpenScenes.Item2.Add(loadingScene);
            }
            else
            {
                var cancelledScene = CancelledScenes.SingleOrDefault(cancelled => cancelled.Value == scene.path);
                if (cancelledScene != null)
                {
                    SceneManager.UnloadSceneAsync(scene);
                    CancelledScenes.Remove(cancelledScene);
                }
            }
        }

        public Task<Tuple<Scene, SceneVariable>> EnterScene(SceneVariable newScene, List<GameObject> transfers = null)
        {
            LoadScene(newScene, isParent: true);
            newScene.ConnectedScenes.ForEach(scene => LoadScene(scene, isParent: false));

            if (!SceneTasks.ContainsKey(newScene))
            {
                SceneTasks[newScene] = Tuple.Create(new TaskCompletionSource<Tuple<Scene, SceneVariable>>(), false);
            }

            return SceneTasks[newScene].Item1.Task;
        }

        public void ExitScene(SceneVariable scene)
        {
            var isLoading = LoadingScenes.Item1.Contains(scene);
            var isOpen = OpenScenes.Item1.Contains(scene);

            if (isLoading)
            {
                CancelledScenes.Add(scene);
                LoadingScenes.Item1.Remove(scene);
            }
            if (isOpen)
            {
                OpenScenes.Item1.Remove(scene);
            }

            var dependantScenes = OpenScenes.Item1
                .Concat(LoadingScenes.Item1)
                .SelectMany(primaryScene => primaryScene.ConnectedScenes);

            OpenScenes.Item2
                .Concat(LoadingScenes.Item2)
                .Where(s => !dependantScenes.Contains(s) && !OpenScenes.Item1.Contains(s))
                .ToList()
                .ForEach(unneededScene =>
                {
                    if (LoadingScenes.Item2.Contains(unneededScene))
                    {
                        LoadingScenes.Item2.Remove(unneededScene);
                        CancelledScenes.Add(unneededScene);
                    }
                    else
                    {
                        OpenScenes.Item2.Remove(unneededScene);
                        SceneManager.UnloadSceneAsync(unneededScene.Value);
                    }
                });
        }

        public void EnterZone(SceneLoadingZone newZone, bool forceEntry = false)
        {
            if (newZone == CurrentZone && !forceEntry) return;
            
            LoadingScenes.Item2.ToList().ForEach(scene =>
            {
                CancelledScenes.Add(scene);
                LoadingScenes.Item1.Remove(scene);
                LoadingScenes.Item2.Remove(scene);
            });
            OpenScenes.Item2.ToList().ForEach(scene =>
            {
                OpenScenes.Item1.Remove(scene);
                OpenScenes.Item2.Remove(scene);
                SceneManager.UnloadSceneAsync(scene.Value);
            });

            Destroy(CurrentZone.SceneContainer);

            CurrentZone = newZone;

            if (newZone == null) return;

            CurrentZone.SceneContainer = new GameObject();
            CurrentZone.SceneContainer.transform.position = Vector3.zero;

            DontDestroyOnLoad(CurrentZone.SceneContainer);

            FindObjectsOfType<ZonePersistence>().ToList().ForEach(obj =>
            {
                if (obj.transform.parent == null)
                {
                    obj.transform.parent = CurrentZone.SceneContainer.transform;
                }
            });
        }

    }


}