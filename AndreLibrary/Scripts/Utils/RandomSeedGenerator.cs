using UnityEngine;

namespace Andre.Utils.RandomGeneration
{
    public class RandomSeedGenerator : MonoBehaviour
    {
        public bool UseGeneratedSeed = true;
        public float seed = 0;
        public string Name;
        public float time = 0;

        private void Awake()
        {
            time = System.Environment.TickCount;
            seed = time + Name.GetHashCode();
            if (UseGeneratedSeed)
                Random.InitState((int)seed);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ChangeSeed();
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                ResetSeed();
            }
        }

        public void ChangeSeed()
        {
            time = System.Environment.TickCount;
            Random.InitState((int)time + Name.GetHashCode());
        }

        public void ResetSeed()
        {
            Random.InitState((int)seed);
        }

    }
}
