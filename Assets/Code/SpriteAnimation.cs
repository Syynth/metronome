using UnityEngine;
using System.Collections.Generic;

namespace Assets.Code
{

    public class SpriteAnimation : ScriptableObject
    {

        public List<Sprite> sprites;

        public float framesPerSecond = 24f;

        public Sprite getFrame(float time)
        {
            int index = (int)(time / framesPerSecond);
            return sprites[index % sprites.Count];
        }

    }
}
