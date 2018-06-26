extern alias mscorlib;
using mscorlib::System.Threading.Tasks;
    
using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using Assets.Code.References;

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
            if (ZoneLoadingManagerReference.Value == null)
            {
                ZoneLoadingManagerReference.Value = this;
            }
        }

        private void OnDestroy()
        {
            var zlms = FindObjectsOfType<ZoneLoadingManager>();

            if (ZoneLoadingManagerReference.Value == this)
            {
                ZoneLoadingManagerReference.Value = zlms.FirstOrDefault(zlm => zlm != this);
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

        private void LoadScene(SceneVariable scene, bool isParent, LoadSceneMode loadSceneMode)
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
                    if (loadSceneMode == LoadSceneMode.Additive)
                    {
                        Debug.Log($"Entering scene {scene.Value} in additive mode");
                        scene.LoadAsync();
                    }
                    else
                    {
                        Debug.Log($"Entering scene {scene.Value} in single mode");
                        scene.GoTo();
                        LoadingScenes.Item1.RemoveWhere(lscene => true);
                        LoadingScenes.Item2.RemoveWhere(lscene => true);
                        OpenScenes.Item1.RemoveWhere(oscene => true);
                        OpenScenes.Item2.RemoveWhere(oscene => true);
                        CancelledScenes.RemoveWhere(cscene => true);
                        return;
                    }
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
                    Debug.Log($"Call to unload cancelled scene {cancelledScene.name}");
                    SceneManager.UnloadSceneAsync(scene);
                    CancelledScenes.Remove(cancelledScene);
                }
            }
        }

        public Task<Tuple<Scene, SceneVariable>> EnterScene(SceneVariable newScene, List<GameObject> transfers = null, LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {
            LoadScene(newScene, isParent: true, loadSceneMode: loadSceneMode);
            newScene.ConnectedScenes.ForEach(scene => LoadScene(scene, isParent: false, loadSceneMode: LoadSceneMode.Additive));

            if (!SceneTasks.ContainsKey(newScene))
            {
                SceneTasks[newScene] = Tuple.Create(new TaskCompletionSource<Tuple<Scene, SceneVariable>>(), false);
            }

            return SceneTasks[newScene].Item1.Task;
        }

        public void ExitScene(SceneVariable scene)
        {
            Debug.Log($"Exit scene called {scene.name}");
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
                        Debug.Log($"Found unreferenced scene ${unneededScene.Value}");
                        SceneManager.UnloadSceneAsync(unneededScene.Value);
                    }
                });
        }

        public async Task EnterZone(SceneLoadingZone newZone, SceneVariable scene, bool forceEntry = false)
        {
            if (newZone == CurrentZone && !forceEntry) return;

            Debug.Log($"Call to EnterZone, zone {newZone.name}");
            
            LoadingScenes.Item2.ToList().ForEach(lscene =>
            {
                CancelledScenes.Add(lscene);
                LoadingScenes.Item1.Remove(lscene);
                LoadingScenes.Item2.Remove(lscene);
            });
            OpenScenes.Item2.ToList().ForEach(oscene =>
            {
                OpenScenes.Item1.Remove(oscene);
                OpenScenes.Item2.Remove(oscene);
                SceneManager.UnloadSceneAsync(oscene.Value);
            });

            Destroy(CurrentZone.SceneContainer);

            CurrentZone = newZone;

            if (newZone == null) return;

            if (scene != null)
            {
                await EnterScene(scene, loadSceneMode: LoadSceneMode.Single);
            }

            CurrentZone.SceneContainer = new GameObject();
            CurrentZone.SceneContainer.transform.position = Vector3.zero;

            DontDestroyOnLoad(CurrentZone.SceneContainer);

            FindObjectsOfType<ZonePersistence>().ToList().ForEach(zp =>
            {
                zp.SceneReady(newZone);
            });

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