using UnityEngine;
using System;

namespace Andre.Utils
{
    public class RandomNumberGen
    {
        private const int a = 16807;
        private const int m = 2147483647;
        private const int q = 127773;
        private const int r = 2836;
        private int seed;
        System.Random random = new System.Random();

        public RandomNumberGen()
        {
            seed = random.Next(1 << 30);
        }

        public RandomNumberGen(float _seed)
        {
            SetSeed(_seed);
        }

        public RandomNumberGen(char _seed)
        {
            SetSeed(_seed);
        }

        public RandomNumberGen(string _seed)
        {
            SetSeed(_seed);
        }

        public RandomNumberGen(double _seed)
        {
            SetSeed(_seed);
        }

        public RandomNumberGen(int _seed)
        {
            SetSeed(_seed);
        }

        public void SetSeed(float _seed)
        {
            seed = (int)_seed;
        }

        public void SetSeed(char _seed)
        {
            seed = (int)_seed.GetHashCode();
        }

        public void SetSeed(string _seed)
        {
            seed = (int)_seed.GetHashCode();
        }

        public void SetSeed(double _seed)
        {
            seed = (int)_seed;
        }

        public void SetSeed(int _seed)
        {
            seed = _seed;
        }

        public double Lehmer32Next()
        {
            int hi = seed / q;
            int lo = seed % q;
            seed = (a * lo) - (r * hi);
            if (seed <= 0)
                seed = seed + m;
            return (seed * 1.0) / m;
        }

        public int RandomInt()
        {
            return (int)Lehmer32Next();
        }

        public int RandomInt(int _min, int _max)
        {
            return (int)(Lehmer32Next() * (_max - _min) + _min);
        }

        public double RandomDouble()
        {
            return Lehmer32Next();
        }

        public double RandomDouble(double _min, double _max)
        {
            return Lehmer32Next() * (_max - _min) + _min;
        }

        public float RandomFloat()
        {
            return (float)Lehmer32Next();
        }

        public float RandomFloat(float _min, float _max)
        {
            return (float)RandomDouble((double)_min, (double)_max);
        }

    }
}
