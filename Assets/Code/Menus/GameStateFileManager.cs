using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;

namespace Assets.Code.Menu
{
    public class GameStateFileManager : MonoBehaviour
    {

        public GameState.GameState EditorGameState = null;
        public GameObject LoadScreenButton = null;

        public Button LoadFileButton = null;
        public RectTransform LoadFileButtonList = null;

        [SerializeField]
        string[] saveFiles = new string[0];

        public void DetectSaveGames()
        {
            Directory.CreateDirectory(Utils.SaveFileDirectory);
            saveFiles = Directory.GetFileSystemEntries(Utils.SaveFileDirectory, "*.json");
            if (LoadScreenButton != null)
            {
                LoadScreenButton.SetActive(saveFiles.Length > 0);
            }
            else
            {
                for (var index = 0; index < saveFiles.Length; ++index)
                {
                    var nextButton = Instantiate<Button>(LoadFileButton);
                    nextButton.GetComponentInChildren<Text>().text = saveFiles[index].Split('/').Last().Replace(".json", "");
                    var count = index;
                    nextButton.onClick.AddListener(() => {
                        EditorGameState.Load(saveFiles[count]);
                        EditorGameState.CurrentScene.GoTo();
                    });
                    var rect = nextButton.GetComponent<RectTransform>().rect;
                    rect.xMin = 25;
                    rect.xMax = 25;
                    var pos = nextButton.transform.localPosition;
                    pos.y = -50 - 75 * index;
                    nextButton.transform.localPosition = pos;
                    nextButton.transform.SetParent(LoadFileButtonList, false);
                }
            }
        }

        private void Start()
        {
            DetectSaveGames();
        }

    }

}

