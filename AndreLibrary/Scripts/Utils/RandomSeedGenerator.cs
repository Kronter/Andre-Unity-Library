using UnityEngine;

namespace Andre.Utils.RandomGeneration
{
    public class RandomSeedGenerator : MonoBehaviour
    {
        public bool UseGeneratedSeed = true;
        public float seed = 0;
        public string Name;
        public float time = 0;
        private RandomNumberGen random;

        private void Awake()
        {
            time = System.Environment.TickCount;
            seed = time + Name.GetHashCode();
            if (random == null)
                random = new RandomNumberGen();
            if (UseGeneratedSeed)
                random.SetSeed((int)seed);
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
            random.SetSeed((int)time + Name.GetHashCode());
        }

        public void ResetSeed()
        {
            random.SetSeed((int)seed);
        }

    }
}
