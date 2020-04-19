﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Andre.Generation
{
    public class KruskalsEdge
    {
        public int Vertex1 { get; set; }
        public DungeonRoom Room1 { get; set; }
        public int Vertex2 { get; set; }
        public DungeonRoom Room2 { get; set; }
        public float Weight { get; set; }
    }

    public class DisjointSet
    {
        int[] parent;   // stores index of parent node
        int[] rank;     //stores rank of perticular node

        /// <summary>
        /// Initialize n natural numbers
        /// </summary>
        /// <param name="length">value of n</param>
        public DisjointSet(int length)
        {
            parent = new int[length];   //initializng length of set
            rank = new int[length];

            //initially making parent of element itself
            for (int i = 0; i < parent.Length; i++)
                parent[i] = i;
        }

        /// <summary>
        /// Making set with one element
        /// </summary>
        /// <param name="x">element</param>
        public void MakeSet(int x)
        {
            parent[x] = x;  //making parent itself
            rank[x] = 0;    //initially rank is zero
        }

        /// <summary>
        /// Union by rank
        /// </summary>
        /// <param name="x">set 1</param>
        /// <param name="y">set 2</param>
        public void Union(int x, int y)
        {
            int representativeX = FindSet(x); //finding representative of x
            int representativeY = FindSet(y); //finding representative of y

            //if both has same rank maiking y is x's parent
            if (rank[representativeX] == rank[representativeY])
            {
                rank[representativeY]++; //incrementing rank of y
                parent[representativeX] = representativeY;
            }

            // making parent which is having higher rank
            else if (rank[representativeX] > rank[representativeY])
            { parent[representativeY] = representativeX; }
            else
            { parent[representativeX] = representativeY; }
        }

        /// <summary>
        /// Finds representative of a set
        /// </summary>
        /// <param name="x">element of a set</param>
        /// <returns></returns>
        public int FindSet(int x)
        {
            if (parent[x] != x)
                parent[x] = FindSet(parent[x]); //path compression
            return parent[x];
        }

        /// <summary>
        /// Finding Immidiate Parent
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int FindImmidiateParent(int x)
        {
            return parent[x];
        }
        /// <summary>
        /// Finding Rank
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int FindRank(int x)
        {
            return rank[x];
        }
    }

    public static class KruskalsAlgorithm
    {
        public static List<KruskalsEdge> Kruskals_MST(List<KruskalsEdge> edges, List<int> vertices, bool smallest)
        {
            //empty result list
            List<KruskalsEdge> result = new List<KruskalsEdge>();

            //making set
            DisjointSet set = new DisjointSet(100);
            foreach (int vertex in vertices)
                set.MakeSet(vertex);

            //sorting the edges order by weight ascending
            var sortedEdge = edges.OrderBy(x => x.Weight).ToList();
            if (smallest)
                sortedEdge.Reverse();

            foreach (KruskalsEdge edge in sortedEdge)
            {
                //adding edge to result if both vertices do not belong to same set
                //both vertices in same set means it can have cycles in tree
                if (set.FindSet(edge.Vertex1) != set.FindSet(edge.Vertex2))
                {
                    result.Add(edge);
                    set.Union(edge.Vertex1, edge.Vertex2);
                }
            }
            return result;
        }

        public static List<KruskalsEdge> Kruskals_MST(List<KruskalsEdge> edges, List<int> vertices, bool smallest, float bias)
        {
            //empty result list
            List<KruskalsEdge> result = new List<KruskalsEdge>();

            //making set
            DisjointSet set = new DisjointSet(100);
            foreach (int vertex in vertices)
                set.MakeSet(vertex);

            //sorting the edges order by weight ascending
            var sortedEdge = edges.OrderBy(x => x.Weight).ToList();
            if (smallest)
                sortedEdge.Reverse();

            foreach (KruskalsEdge edge in sortedEdge)
            {
                //adding edge to result if both vertices do not belong to same set
                //both vertices in same set means it can have cycles in tree
                if (set.FindSet(edge.Vertex1) != set.FindSet(edge.Vertex2))
                {
                    result.Add(edge);
                    set.Union(edge.Vertex1, edge.Vertex2);
                }
                else if (Random.Range(0, 100) <= bias)
                {
                    result.Add(edge);
                    set.Union(edge.Vertex1, edge.Vertex2);
                }
            }
            return result;
        }
    }
}
