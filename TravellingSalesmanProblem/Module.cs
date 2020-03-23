using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanProblem
{
    class City
    {
        public int ID { get; }
        public double X { get; }
        public double Y { get; }

        public City(double _X, double _Y, int _ID)
        {
            X = _X;
            Y = _Y;
            ID = _ID;
        }

        public static double Distance(City _pierwsze, City _drugie)
        {
            return Math.Sqrt((_drugie.X - _pierwsze.X) * (_drugie.X - _pierwsze.X) + (_drugie.Y - _pierwsze.Y) * (_drugie.Y - _pierwsze.Y));
        }
    }

    class Problem
    {
        public City[] Cities { get; }
        public Random RNG { get; }

        public Problem(int _cityCount)
        {
            Cities = new City[_cityCount];
            RNG = new Random();
            for (int i = 0; i < _cityCount; i++)
            {
                Cities[i] = new City(RNG.NextDouble() * 100, RNG.NextDouble() * 100, i);
            }
        }
    }

    class Solution
    {
        Problem problem;
        public int[] CitySequence { get; internal set; }
        public double Score { get; internal set;  }

        public Solution(Problem _problem)
        {
            problem = _problem;
            CitySequence = new int[problem.Cities.Length];
            GenerateRandomSolution();
            CalculateScore();
        }

        private void GenerateRandomSolution()
        {
            int max = CitySequence.Length;
            CitySequence = Enumerable.Range(0, max).OrderBy(c => problem.RNG.Next()).ToArray();
            int value = CitySequence[0];
            for (int i = 1; i < CitySequence.Length; i++)
            {
                if(CitySequence[i] == 0)
                {
                    CitySequence[0] = 0;
                    CitySequence[i] = value;
                }
            }
        }

        private void CalculateScore()
        {
            double suma = 0;
            for (int i = 0; i < CitySequence.Length - 1; i++)
            {
                suma += City.Distance(problem.Cities[CitySequence[i]], problem.Cities[CitySequence[i + 1]]);
            }
            suma += City.Distance(problem.Cities[CitySequence[CitySequence.Length - 1]], problem.Cities[CitySequence[0]]);
            Score = suma;
        }

    }

}
