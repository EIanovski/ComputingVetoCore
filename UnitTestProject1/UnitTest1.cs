using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComputingVetoCore;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SolverFoundation.Common;
using System.Threading.Tasks;
using FibonacciHeap;

namespace Tests
{
    [TestClass]
    public class MaxFlowTests
    {
        [TestMethod]
        public void TestOutDegree()
        {
            MPM.Graph g = new MPM.Graph();
            g.AddEdge(0, 1, 2);
            g.AddEdge(0, 2, 3);
            g.AddEdge(1, 2, 1);

            Assert.AreEqual(2, g.OutDegree(0));
            Assert.AreEqual(0, g.OutDegree(2));
        }

        [TestMethod]
        public void TestInDegree()
        {
            MPM.Graph g = new MPM.Graph();
            g.AddEdge(0, 1, 2);
            g.AddEdge(0, 2, 3);
            g.AddEdge(1, 2, 1);

            Assert.AreEqual(2, g.InDegree(2));
            Assert.AreEqual(0, g.InDegree(0));
        }

        [TestMethod]
        public void TestAddEdge()
        {
            MPM.Graph g = new MPM.Graph();
            g.AddEdge(0, 1, 2);
            g.AddEdge(0, 2, 3);
            g.AddEdge(1, 2, 1);

            Assert.AreEqual(2, g.OutDegree(0));
            Assert.AreEqual(0, g.InDegree(0));
            Assert.IsTrue(g.HasEdge(0, 1, 2));
            Assert.IsTrue(g.HasEdge(0, 2, 3));

            Assert.AreEqual(1, g.OutDegree(1));
            Assert.AreEqual(1, g.InDegree(1));
            Assert.IsTrue(g.HasEdge(1, 2, 1));

            Assert.AreEqual(2, g.InDegree(2));
            Assert.AreEqual(0, g.OutDegree(2));
        }

        [TestMethod]
        public void TestAddEdgeWithFlow()
        {
            MPM.Graph g = new MPM.Graph();
            g.AddEdge(0, 1, 2, 3);
            g.AddEdge(0, 2, 3, 4);
            g.AddEdge(1, 2, 1, 0);

            Assert.AreEqual(2, g.OutDegree(0));
            Assert.AreEqual(0, g.InDegree(0));
            Assert.IsTrue(g.HasEdge(0, 1, 2, 3));
            Assert.IsTrue(g.HasEdge(0, 2, 3, 4));

            Assert.AreEqual(1, g.OutDegree(1));
            Assert.AreEqual(1, g.InDegree(1));
            Assert.IsTrue(g.HasEdge(1, 2, 1, 0));

            Assert.AreEqual(2, g.InDegree(2));
            Assert.AreEqual(0, g.OutDegree(2));
        }

        [TestMethod]
        public void TestAddFlow()
        {
            // First graph https://upload.wikimedia.org/wikipedia/commons/3/37/Dinic_algorithm_G1.svg
            // Second graph https://upload.wikimedia.org/wikipedia/commons/5/56/Dinic_algorithm_G2.svg
            MPM.Graph g = new MPM.Graph();
            g.AddEdge(MPM.SOURCE, 1, 10, 0);
            g.AddEdge(MPM.SOURCE, 2, 10, 0);
            g.AddEdge(1, 2, 2, 0);
            g.AddEdge(1, 3, 4, 0);
            g.AddEdge(1, 4, 8, 0);
            g.AddEdge(2, 4, 9, 0);
            g.AddEdge(3, MPM.SINK, 10, 0);
            g.AddEdge(4, 3, 6, 0);
            g.AddEdge(4, MPM.SINK, 10, 0);

            Dictionary<int, Dictionary<int, int>> flow = new Dictionary<int, Dictionary<int, int>>();
            flow[MPM.SOURCE] = new Dictionary<int, int>
            {
                {1, 10 },
                {2, 4 },
            };
            flow[1] = new Dictionary<int, int>
            {
                { 3,4 },
                {4,6 }
            };
            flow[2] = new Dictionary<int, int>
            {
                {4,4 }
            };
            flow[3] = new Dictionary<int, int>
            {
                {MPM.SINK, 4 }
            };
            flow[4] = new Dictionary<int, int>
            {
                {MPM.SINK,10 }
            };

            g.AddFlow(flow);

            Assert.IsTrue(g.HasEdge(MPM.SOURCE, 1, 10, 10));
            Assert.IsTrue(g.HasEdge(MPM.SOURCE, 2, 10, 4));
            Assert.IsTrue(g.HasEdge(1, 2, 2, 0));
            Assert.IsTrue(g.HasEdge(1, 3, 4, 4));
            Assert.IsTrue(g.HasEdge(1, 4, 8, 6));
            Assert.IsTrue(g.HasEdge(2, 4, 9, 4));
            Assert.IsTrue(g.HasEdge(3, MPM.SINK, 10, 4));
            Assert.IsTrue(g.HasEdge(4, 3, 6, 0));
            Assert.IsTrue(g.HasEdge(4, MPM.SINK, 10, 10));
        }

        [TestMethod]
        public void TestAddBackFlow()
        {
            MPM.Graph G = new MPM.Graph();
            G.AddEdge(0, 1, 10, 10);

            Dictionary<int, Dictionary<int, int>> flow = new Dictionary<int, Dictionary<int, int>>
            {
                { 1, new Dictionary<int, int> { { 0, 5 } } }
            };
            G.AddFlow(flow);
            Assert.IsTrue(G.HasEdge(0, 1, 10, 5));
        }

        [TestMethod]
        public void TestAddFlow2()
        {
            // First graph https://upload.wikimedia.org/wikipedia/commons/5/56/Dinic_algorithm_G2.svg
            // Second graph https://upload.wikimedia.org/wikipedia/commons/7/71/Dinic_algorithm_G3.svg
            MPM.Graph g = new MPM.Graph();
            g.AddEdge(MPM.SOURCE, 1, 10, 10);
            g.AddEdge(MPM.SOURCE, 2, 10, 4);
            g.AddEdge(1, 2, 2, 0);
            g.AddEdge(1, 3, 4, 4);
            g.AddEdge(1, 4, 8, 6);
            g.AddEdge(2, 4, 9, 4);
            g.AddEdge(3, MPM.SINK, 10, 4);
            g.AddEdge(4, 3, 6, 0);
            g.AddEdge(4, MPM.SINK, 10, 10);

            Dictionary<int, Dictionary<int, int>> flow = new Dictionary<int, Dictionary<int, int>>();
            flow[MPM.SOURCE] = new Dictionary<int, int>
            {
                {2, 5 },
            };
            flow[2] = new Dictionary<int, int>
            {
                {4,5 }
            };
            flow[3] = new Dictionary<int, int>
            {
                {MPM.SINK, 5 }
            };
            flow[4] = new Dictionary<int, int>
            {
                {3,5 }
            };

            g.AddFlow(flow);

            Assert.IsTrue(g.HasEdge(MPM.SOURCE, 1, 10, 10));
            Assert.IsTrue(g.HasEdge(MPM.SOURCE, 2, 10, 9));
            Assert.IsTrue(g.HasEdge(1, 2, 2, 0));
            Assert.IsTrue(g.HasEdge(1, 3, 4, 4));
            Assert.IsTrue(g.HasEdge(1, 4, 8, 6));
            Assert.IsTrue(g.HasEdge(2, 4, 9, 9));
            Assert.IsTrue(g.HasEdge(3, MPM.SINK, 10, 9));
            Assert.IsTrue(g.HasEdge(4, 3, 6, 5));
            Assert.IsTrue(g.HasEdge(4, MPM.SINK, 10, 10));
        }

        [TestMethod]
        public void TestRemoveNode()
        {
            MPM.Graph G = new MPM.Graph();

            G.AddEdge(1, 0, 0);
            G.AddEdge(2, 0, 0);
            G.AddEdge(1, 2, 0);
            G.AddEdge(0, 3, 0);
            G.AddEdge(0, 4, 0);
            G.AddEdge(4, 3, 0);

            G.RemoveNode(0);

            Assert.IsTrue(G.HasEdge(1, 2, 0));
            Assert.IsTrue(G.HasEdge(4, 3, 0));
            Assert.IsFalse(G.HasEdge(1, 0, 0));
            Assert.IsFalse(G.HasEdge(2, 0, 0));
            Assert.IsFalse(G.HasEdge(0, 3, 0));
            Assert.IsFalse(G.HasEdge(0, 4, 0));
        }

        [TestMethod]
        public void TestResidualGraphWikipedia1()
        {
            // Flow graph https://upload.wikimedia.org/wikipedia/commons/3/37/Dinic_algorithm_G1.svg
            // Residual graph https://upload.wikimedia.org/wikipedia/commons/f/fd/Dinic_algorithm_Gf1.svg
            MPM.Graph g = new MPM.Graph();
            g.AddEdge(MPM.SOURCE, 1, 10, 0);
            g.AddEdge(MPM.SOURCE, 2, 10, 0);
            g.AddEdge(1, 2, 2, 0);
            g.AddEdge(1, 3, 4, 0);
            g.AddEdge(1, 4, 8, 0);
            g.AddEdge(2, 4, 9, 0);
            g.AddEdge(3, MPM.SINK, 10, 0);
            g.AddEdge(4, 3, 6, 0);
            g.AddEdge(4, MPM.SINK, 10, 0);

            MPM.Graph resG = g.GetResidualGraph();

            Assert.AreEqual(2, resG.OutDegree(MPM.SOURCE));
            Assert.AreEqual(0, resG.InDegree(MPM.SOURCE));
            Assert.IsTrue(resG.HasEdge(MPM.SOURCE, 1, 10));
            Assert.IsTrue(resG.HasEdge(MPM.SOURCE, 2, 10));

            Assert.AreEqual(3, resG.OutDegree(1));
            Assert.AreEqual(1, resG.InDegree(1));
            Assert.IsTrue(resG.HasEdge(1, 2, 2));
            Assert.IsTrue(resG.HasEdge(1, 3, 4));
            Assert.IsTrue(resG.HasEdge(1, 4, 8));

            Assert.AreEqual(1, resG.OutDegree(2));
            Assert.AreEqual(2, resG.InDegree(2));
            Assert.IsTrue(resG.HasEdge(2, 4, 9));

            Assert.AreEqual(1, resG.OutDegree(3));
            Assert.AreEqual(2, resG.InDegree(3));
            Assert.IsTrue(resG.HasEdge(3, MPM.SINK, 10));

            Assert.AreEqual(2, resG.OutDegree(4));
            Assert.AreEqual(2, resG.InDegree(4));
            Assert.IsTrue(resG.HasEdge(4, MPM.SINK, 10));
            Assert.IsTrue(resG.HasEdge(4, 3, 6));

            Assert.AreEqual(2, resG.InDegree(MPM.SINK));
            Assert.AreEqual(0, resG.OutDegree(MPM.SINK));
        }

        [TestMethod]
        public void TestEmptyFlow()
        {
            MPM.Graph g = new MPM.Graph();
            g.AddEdge(MPM.SOURCE, 1, 10);
            g.AddEdge(MPM.SOURCE, 2, 10);
            g.AddEdge(1, 2, 2);
            g.AddEdge(1, 3, 4);
            g.AddEdge(1, 4, 8);
            g.AddEdge(2, 4, 9);
            g.AddEdge(3, MPM.SINK, 10);
            g.AddEdge(4, 3, 6);
            g.AddEdge(4, MPM.SINK, 10);

            Dictionary<int, Dictionary<int, int>> flow = g.GetEmptyFlow();
            Assert.IsTrue(flow[MPM.SOURCE].Keys.Count == 2);
            Assert.IsTrue(flow[MPM.SOURCE][1] == 0);
            Assert.IsTrue(flow[MPM.SOURCE][2] == 0);

            Assert.IsTrue(flow[1].Keys.Count == 3);
            Assert.IsTrue(flow[1][2] == 0);
            Assert.IsTrue(flow[1][3] == 0);
            Assert.IsTrue(flow[1][4] == 0);

            Assert.IsTrue(flow[2].Keys.Count == 1);
            Assert.IsTrue(flow[2][4] == 0);

            Assert.IsTrue(flow[3].Keys.Count == 1);
            Assert.IsTrue(flow[3][MPM.SINK] == 0);

            Assert.IsTrue(flow[4].Keys.Count == 2);
            Assert.IsTrue(flow[4][3] == 0);
            Assert.IsTrue(flow[4][MPM.SINK] == 0);
        }

        [TestMethod]
        public void TestPushingAndPullingCreatesValidFlow()
        {
            MPM.Graph g = new MPM.Graph();
            g.AddEdge(MPM.SOURCE, 1, 10);
            g.AddEdge(MPM.SOURCE, 2, 10);
            g.AddEdge(1, 2, 2);
            g.AddEdge(1, 3, 4);
            g.AddEdge(1, 4, 8);
            g.AddEdge(2, 4, 9);
            g.AddEdge(3, MPM.SINK, 10);
            g.AddEdge(4, 3, 6);
            g.AddEdge(4, MPM.SINK, 10);

            IEnumerable<int> vertices = new int[]
            {
                MPM.SOURCE, 1, 2, 3, 4, MPM.SINK
            };
            MPM.Graph levelG = g.GetLevelGraph();

            Dictionary<int, Dictionary<int, int>> flow = levelG.GetEmptyFlow();
            levelG.Push(1, 10, flow, new List<int>());
            levelG.Pull(1, 10, flow, new List<int>());
            Assert.IsTrue(MPM.IsFlowValid(vertices, flow));
            levelG.Push(2, 4, flow, new List<int>());
            levelG.Pull(2, 4, flow, new List<int>());
            Assert.IsTrue(MPM.IsFlowValid(vertices, flow));
        }

        [TestMethod]
        public void TestUnbalancedFlow()
        {
            MPM.Graph G = new MPM.Graph();
            G.AddEdge(MPM.SOURCE, 1, 5, 5);
            G.AddEdge(MPM.SOURCE, 2, 5, 5);
            G.AddEdge(1, 3, 5, 5);
            G.AddEdge(2, 3, 5, 5);
            G.AddEdge(3, MPM.SINK, 5, 5);
            Assert.IsFalse(G.IsFlowValid());
        }

        [TestMethod]
        public void TestUnbalancedFlow2()
        {
            IEnumerable<int> nodes = new int[]
            {
                MPM.SOURCE, 1, 2, 3, MPM.SINK
            };
            Dictionary<int, Dictionary<int, int>> flow = new Dictionary<int, Dictionary<int, int>>();
            flow[MPM.SOURCE] = new Dictionary<int, int>();
            flow[MPM.SOURCE][1] = 5;
            flow[MPM.SOURCE][2] = 5;
            flow[1] = new Dictionary<int, int>();
            flow[1][3] = 5;
            flow[2] = new Dictionary<int, int>();
            flow[2][3] = 5;
            flow[3] = new Dictionary<int, int>();
            flow[3][MPM.SINK] = 5;
            Assert.IsFalse(MPM.IsFlowValid(nodes, flow));
        }

        [TestMethod]
        public void TestFlowViolatesCap()
        {
            MPM.Graph G = new MPM.Graph();
            G.AddEdge(MPM.SOURCE, 1, 5, 5);
            G.AddEdge(MPM.SOURCE, 2, 5, 5);
            G.AddEdge(1, 3, 5, 5);
            G.AddEdge(2, 3, 5, 5);
            G.AddEdge(3, MPM.SINK, 10, 5);
            Assert.IsFalse(G.IsFlowValid());
        }

        [TestMethod]
        public void TestFlowIsFine2()
        {
            IEnumerable<int> nodes = new int[]
            {
                MPM.SOURCE, 1, 2, 3, MPM.SINK
            };
            Dictionary<int, Dictionary<int, int>> flow = new Dictionary<int, Dictionary<int, int>>();
            flow[MPM.SOURCE] = new Dictionary<int, int>();
            flow[MPM.SOURCE][1] = 5;
            flow[MPM.SOURCE][2] = 5;
            flow[1] = new Dictionary<int, int>();
            flow[1][3] = 5;
            flow[2] = new Dictionary<int, int>();
            flow[2][3] = 5;
            flow[3] = new Dictionary<int, int>();
            flow[3][MPM.SINK] = 10;
            Assert.IsTrue(MPM.IsFlowValid(nodes, flow));
        }

        [TestMethod]
        public void TestFlowIsFine()
        {
            MPM.Graph G = new MPM.Graph();
            G.AddEdge(MPM.SOURCE, 1, 5, 5);
            G.AddEdge(MPM.SOURCE, 2, 5, 5);
            G.AddEdge(1, 3, 5, 5);
            G.AddEdge(2, 3, 5, 5);
            G.AddEdge(3, MPM.SINK, 10, 10);
            Assert.IsTrue(G.IsFlowValid());
        }

        [TestMethod]
        public void TestWikipediaExample()
        {
            // Flow graph https://upload.wikimedia.org/wikipedia/commons/3/37/Dinic_algorithm_G1.svg

            MPM.Graph g = new MPM.Graph();
            g.AddEdge(MPM.SOURCE, 1, 10);
            g.AddEdge(MPM.SOURCE, 2, 10);
            g.AddEdge(1, 2, 2);
            g.AddEdge(1, 3, 4);
            g.AddEdge(1, 4, 8);
            g.AddEdge(2, 4, 9);
            g.AddEdge(3, MPM.SINK, 10);
            g.AddEdge(4, 3, 6);
            g.AddEdge(4, MPM.SINK, 10);
            int answer = 19;
            Assert.AreEqual(answer, MPM.MaxFlow(g));
        }

        [TestMethod]
        public void TestLevelGraphWikipedia1()
        {
            // Residual graph https://upload.wikimedia.org/wikipedia/commons/f/fd/Dinic_algorithm_Gf1.svg
            // Level graph https://upload.wikimedia.org/wikipedia/commons/8/80/Dinic_algorithm_GL1.svg
            MPM.Graph g = new MPM.Graph();
            g.AddEdge(MPM.SOURCE, 1, 10);
            g.AddEdge(MPM.SOURCE, 2, 10);
            g.AddEdge(1, 2, 2);
            g.AddEdge(1, 3, 4);
            g.AddEdge(1, 4, 8);
            g.AddEdge(2, 4, 9);
            g.AddEdge(3, MPM.SINK, 10);
            g.AddEdge(4, 3, 6);
            g.AddEdge(4, MPM.SINK, 10);

            MPM.Graph levelG = g.GetLevelGraph();
            Assert.AreEqual(2, levelG.OutDegree(MPM.SOURCE));
            Assert.AreEqual(0, levelG.InDegree(MPM.SOURCE));
            Assert.IsTrue(levelG.HasEdge(MPM.SOURCE, 1, 10));
            Assert.IsTrue(levelG.HasEdge(MPM.SOURCE, 2, 10));

            Assert.AreEqual(2, levelG.OutDegree(1));
            Assert.AreEqual(1, levelG.InDegree(1));
            Assert.IsTrue(levelG.HasEdge(1, 3, 4));
            Assert.IsTrue(levelG.HasEdge(1, 4, 8));

            Assert.AreEqual(1, levelG.OutDegree(2));
            Assert.AreEqual(1, levelG.InDegree(2));
            Assert.IsTrue(levelG.HasEdge(2, 4, 9));

            Assert.AreEqual(1, levelG.OutDegree(3));
            Assert.AreEqual(1, levelG.InDegree(3));
            Assert.IsTrue(levelG.HasEdge(3, MPM.SINK, 10));

            Assert.AreEqual(1, levelG.OutDegree(4));
            Assert.AreEqual(2, levelG.InDegree(4));
            Assert.IsTrue(levelG.HasEdge(4, MPM.SINK, 10));

            Assert.AreEqual(0, levelG.OutDegree(MPM.SINK));
            Assert.AreEqual(2, levelG.InDegree(MPM.SINK));
        }

        [TestMethod]
        public void TestCapacity()
        {
            // Level graph https://upload.wikimedia.org/wikipedia/commons/9/97/Dinic_algorithm_GL2.svg
            MPM.Graph g = new MPM.Graph();
            g.AddEdge(MPM.SOURCE, 1, 10, 10);
            g.AddEdge(MPM.SOURCE, 2, 10, 4);
            g.AddEdge(1, 2, 2, 0);
            g.AddEdge(1, 3, 4, 4);
            g.AddEdge(1, 4, 8, 6);
            g.AddEdge(2, 4, 9, 4);
            g.AddEdge(3, MPM.SINK, 10, 4);
            g.AddEdge(4, 3, 6, 0);
            g.AddEdge(4, MPM.SINK, 10, 10);

            MPM.Graph resG = g.GetResidualGraph();
            MPM.Graph levelG = resG.GetLevelGraph();

            Assert.AreEqual(0, levelG.Capacity(1));
            Assert.AreEqual(5, levelG.Capacity(2));
            Assert.AreEqual(6, levelG.Capacity(3));
            Assert.AreEqual(5, levelG.Capacity(4));
        }

        [TestMethod]
        public void TestCapacity2()
        {
            int[,] data = new int[,] {
                { MPM.SOURCE, 2, 8 },
                { MPM.SOURCE, 4, 7 },
                { MPM.SOURCE, 6, 10 },
                { MPM.SOURCE, 7, 12 },
                { 2, 8, 4 },
                { 2, 9, 3 },
                { 2, 11, 8 },
                { 3, 11, 2 },
                { 3, 12, 2 },
                { 4, 9, 2 },
                { 4, 10, 3 },
                { 4, 12, 5 },
                { 6, 8, 4 },
                { 6, 9, 2 },
                { 7, 8, 3 },
                { 7, 9, 6 },
                { 7, 10, 4 },
                { 8, MPM.SINK, 7 },
                { 9, MPM.SINK, 6 },
                { 9, 3, 4 },
                { 10, MPM.SINK, 4 },
                { 11, MPM.SINK, 9 },
                { 12, MPM.SINK, 15 }
            };
            MPM.Graph flowGraph = DoubleArrayToGraph(data);
            MPM.Graph levelGraph = flowGraph.GetLevelGraph();
            Assert.AreEqual(4, levelGraph.Capacity(10));
        }


        [TestMethod]
        public void TestResidualGraphWikipedia2()
        {
            // Flow graph https://upload.wikimedia.org/wikipedia/commons/5/56/Dinic_algorithm_G2.svg
            // Residual graph https://upload.wikimedia.org/wikipedia/commons/4/43/Dinic_algorithm_Gf2.svg
            MPM.Graph g = new MPM.Graph();
            g.AddEdge(MPM.SOURCE, 1, 10, 10);
            g.AddEdge(MPM.SOURCE, 2, 10, 4);
            g.AddEdge(1, 2, 2, 0);
            g.AddEdge(1, 3, 4, 4);
            g.AddEdge(1, 4, 8, 6);
            g.AddEdge(2, 4, 9, 4);
            g.AddEdge(3, MPM.SINK, 10, 4);
            g.AddEdge(4, 3, 6, 0);
            g.AddEdge(4, MPM.SINK, 10, 10);

            MPM.Graph resG = g.GetResidualGraph();
            Assert.AreEqual(1, resG.OutDegree(MPM.SOURCE));
            Assert.AreEqual(2, resG.InDegree(MPM.SOURCE));
            Assert.IsTrue(resG.HasEdge(MPM.SOURCE, 2, 6));

            Assert.AreEqual(3, resG.OutDegree(1));
            Assert.AreEqual(2, resG.InDegree(1));
            Assert.IsTrue(resG.HasEdge(1, MPM.SOURCE, 10));
            Assert.IsTrue(resG.HasEdge(1, 2, 2));
            Assert.IsTrue(resG.HasEdge(1, 4, 2));

            Assert.AreEqual(2, resG.OutDegree(2));
            Assert.AreEqual(3, resG.InDegree(2));
            Assert.IsTrue(resG.HasEdge(2, MPM.SOURCE, 4));
            Assert.IsTrue(resG.HasEdge(2, 4, 5));

            Assert.AreEqual(2, resG.OutDegree(3));
            Assert.AreEqual(2, resG.InDegree(3));
            Assert.IsTrue(resG.HasEdge(3, MPM.SINK, 6));
            Assert.IsTrue(resG.HasEdge(3, 1, 4));

            Assert.AreEqual(3, resG.OutDegree(4));
            Assert.AreEqual(3, resG.InDegree(4));
            Assert.IsTrue(resG.HasEdge(4, 2, 4));
            Assert.IsTrue(resG.HasEdge(4, 1, 6));
            Assert.IsTrue(resG.HasEdge(4, 3, 6));

            Assert.AreEqual(2, resG.OutDegree(MPM.SINK));
            Assert.AreEqual(1, resG.InDegree(MPM.SINK));
            Assert.IsTrue(resG.HasEdge(MPM.SINK, 3, 4));
            Assert.IsTrue(resG.HasEdge(MPM.SINK, 4, 10));
        }

        [TestMethod]
        public void TestLevelGraphWikipedia2()
        {
            // Residual graph https://upload.wikimedia.org/wikipedia/commons/4/43/Dinic_algorithm_Gf2.svg
            // Level graph https://upload.wikimedia.org/wikipedia/commons/9/97/Dinic_algorithm_GL2.svg
            MPM.Graph g = new MPM.Graph();
            g.AddEdge(MPM.SOURCE, 1, 10, 10);
            g.AddEdge(MPM.SOURCE, 2, 10, 4);
            g.AddEdge(1, 2, 2, 0);
            g.AddEdge(1, 3, 4, 4);
            g.AddEdge(1, 4, 8, 6);
            g.AddEdge(2, 4, 9, 4);
            g.AddEdge(3, MPM.SINK, 10, 4);
            g.AddEdge(4, 3, 6, 0);
            g.AddEdge(4, MPM.SINK, 10, 10);

            MPM.Graph resG = g.GetResidualGraph();
            MPM.Graph levelG = resG.GetLevelGraph();

            Assert.AreEqual(1, levelG.OutDegree(MPM.SOURCE));
            Assert.AreEqual(0, levelG.InDegree(MPM.SOURCE));

            Assert.IsTrue(levelG.HasEdge(MPM.SOURCE, 2, 6));

            Assert.AreEqual(0, levelG.OutDegree(1));
            Assert.AreEqual(1, levelG.InDegree(1));


            Assert.AreEqual(1, levelG.OutDegree(2));
            Assert.AreEqual(1, levelG.InDegree(2));
            Assert.IsTrue(levelG.HasEdge(2, 4, 5));

            Assert.AreEqual(1, levelG.OutDegree(3));
            Assert.AreEqual(1, levelG.InDegree(3));
            Assert.IsTrue(levelG.HasEdge(3, MPM.SINK, 6));

            Assert.AreEqual(2, levelG.OutDegree(4));
            Assert.AreEqual(1, levelG.InDegree(4));
            Assert.IsTrue(levelG.HasEdge(4, 3, 6));
            Assert.IsTrue(levelG.HasEdge(4, 1, 6));

            Assert.AreEqual(0, levelG.OutDegree(MPM.SINK));
            Assert.AreEqual(1, levelG.InDegree(MPM.SINK));
        }

        [TestMethod]
        public void TestCountFlow()
        {
            MPM.Graph g = new MPM.Graph();
            g.AddEdge(MPM.SOURCE, 1, 10, 10);
            g.AddEdge(MPM.SOURCE, 2, 10, 9);
            g.AddEdge(1, 2, 2, 0);
            g.AddEdge(1, 3, 4, 4);
            g.AddEdge(1, 4, 8, 6);
            g.AddEdge(2, 4, 9, 9);
            g.AddEdge(3, MPM.SINK, 10, 9);
            g.AddEdge(4, 3, 6, 5);
            g.AddEdge(4, MPM.SINK, 10, 10);
            Assert.AreEqual(19, g.CountFlow());
        }

        [TestMethod]
        public void TestResidualGraphWikipedia3()
        {
            // Flow graph https://upload.wikimedia.org/wikipedia/commons/7/71/Dinic_algorithm_G3.svg
            // Residual graph https://upload.wikimedia.org/wikipedia/commons/2/20/Dinic_algorithm_Gf3.svg
            MPM.Graph g = new MPM.Graph();
            g.AddEdge(MPM.SOURCE, 1, 10, 10);
            g.AddEdge(MPM.SOURCE, 2, 10, 9);
            g.AddEdge(1, 2, 2, 0);
            g.AddEdge(1, 3, 4, 4);
            g.AddEdge(1, 4, 8, 6);
            g.AddEdge(2, 4, 9, 9);
            g.AddEdge(3, MPM.SINK, 10, 9);
            g.AddEdge(4, 3, 6, 5);
            g.AddEdge(4, MPM.SINK, 10, 10);

            MPM.Graph resG = g.GetResidualGraph();
            Assert.AreEqual(1, resG.OutDegree(MPM.SOURCE));
            Assert.AreEqual(2, resG.InDegree(MPM.SOURCE));
            Assert.IsTrue(resG.HasEdge(MPM.SOURCE, 2, 1));

            Assert.AreEqual(3, resG.OutDegree(1));
            Assert.AreEqual(2, resG.InDegree(1));
            Assert.IsTrue(resG.HasEdge(1, MPM.SOURCE, 10));
            Assert.IsTrue(resG.HasEdge(1, 2, 2));
            Assert.IsTrue(resG.HasEdge(1, 4, 2));

            Assert.AreEqual(1, resG.OutDegree(2));
            Assert.AreEqual(3, resG.InDegree(2));
            Assert.IsTrue(resG.HasEdge(2, MPM.SOURCE, 9));

            Assert.AreEqual(3, resG.OutDegree(3));
            Assert.AreEqual(2, resG.InDegree(3));
            Assert.IsTrue(resG.HasEdge(3, MPM.SINK, 1));
            Assert.IsTrue(resG.HasEdge(3, 1, 4));
            Assert.IsTrue(resG.HasEdge(3, 4, 5));

            Assert.AreEqual(3, resG.OutDegree(4));
            Assert.AreEqual(3, resG.InDegree(4));
            Assert.IsTrue(resG.HasEdge(4, 2, 9));
            Assert.IsTrue(resG.HasEdge(4, 1, 6));
            Assert.IsTrue(resG.HasEdge(4, 3, 1));

            Assert.AreEqual(2, resG.OutDegree(MPM.SINK));
            Assert.AreEqual(1, resG.InDegree(MPM.SINK));
            Assert.IsTrue(resG.HasEdge(MPM.SINK, 3, 9));
            Assert.IsTrue(resG.HasEdge(MPM.SINK, 4, 10));
        }

        [TestMethod]
        public void TestLevelGraphWikipedia3()
        {
            // Residual graph https://upload.wikimedia.org/wikipedia/commons/2/20/Dinic_algorithm_Gf3.svg
            // Level graph https://upload.wikimedia.org/wikipedia/commons/9/95/Dinic_algorithm_GL3.svg
            MPM.Graph g = new MPM.Graph();
            g.AddEdge(MPM.SOURCE, 1, 10, 10);
            g.AddEdge(MPM.SOURCE, 2, 10, 9);
            g.AddEdge(1, 2, 2, 0);
            g.AddEdge(1, 3, 4, 4);
            g.AddEdge(1, 4, 8, 6);
            g.AddEdge(2, 4, 9, 9);
            g.AddEdge(3, MPM.SINK, 10, 9);
            g.AddEdge(4, 3, 6, 5);
            g.AddEdge(4, MPM.SINK, 10, 10);

            MPM.Graph resG = g.GetResidualGraph();
            MPM.Graph levelG = resG.GetLevelGraph();

            Assert.AreEqual(1, levelG.OutDegree(MPM.SOURCE));
            Assert.AreEqual(0, levelG.InDegree(MPM.SOURCE));
            Assert.IsTrue(levelG.HasEdge(MPM.SOURCE, 2, 1));

            Assert.AreEqual(0, levelG.OutDegree(2));
            Assert.AreEqual(1, levelG.InDegree(2));
        }

        [TestMethod]
        public void TestMultiGraphFlow()
        {
            MPM.Graph G = new MPM.Graph();
            G.AddEdge(MPM.SOURCE, 1, 5, augmentExisting: true);
            G.AddEdge(MPM.SOURCE, 1, 5, augmentExisting: true);
            G.AddEdge(1, MPM.SINK, 7, augmentExisting: true);
            Assert.AreEqual(7, MPM.MaxFlow(G));
        }

        [TestMethod]
        public void TestPrioritiesUpdated()
        {
            MPM.Graph G = new MPM.Graph();
            G.AddEdge(MPM.SOURCE, 1, 2);
            G.AddEdge(MPM.SOURCE, 2, 3);
            G.AddEdge(1, 3, 2);
            G.AddEdge(2, 3, 1);
            G.AddEdge(2, 4, 2);
            G.AddEdge(3, 5, 1);
            G.AddEdge(4, 5, 2);
            G.AddEdge(5, MPM.SINK, 3);

            FibonacciHeap<int, int> h = new FibonacciHeap<int, int>(0);
            var one = new FibonacciHeapNode<int, int>
                (1, G.Capacity(1));
            var two = new FibonacciHeapNode<int, int>
                (2, G.Capacity(2));
            var three = new FibonacciHeapNode<int, int>
                (3, G.Capacity(3));
            var four = new FibonacciHeapNode<int, int>
                (4, G.Capacity(4));
            var five = new FibonacciHeapNode<int, int>
                (5, G.Capacity(5));
            var pointersToHeap = new Dictionary<int, FibonacciHeapNode<int, int>>
            {
                {1, one },
                {2, two },
                {3, three },
                {4, four },
                {5, five }
            };
            foreach (FibonacciHeapNode<int, int> node in pointersToHeap.Values)
            {
                h.Insert(node);
            }
            MPM.SaturateNode(h, G, G, G.GetEmptyFlow(), pointersToHeap);
            Assert.AreEqual(2, five.Key);
            Assert.AreEqual(2, four.Key);
            Assert.AreEqual(2, two.Key);
            Assert.AreEqual(0, one.Key);
        }

        [TestMethod]
        public void GitHubExample()
        {
            int[,] data = new int[,] {
                { MPM.SOURCE, 2, 16 },
                { MPM.SOURCE, 3, 16 },
                { 2, 14, 16 },
                { 2, 13, 16 },
                { 2, 12, 16 },
                { 2, 11, 16 },
                { 2, 10, 16 },
                { 2, 9, 16 },
                { 2, 8, 16 },
                { 3, 5, 16 },
                { 3, 6, 16 },
                { 3, 8, 16 },
                { 3, 14, 16 },
                { 3, 13, 16 },
                { 3, 12, 16 },
                { 3, 4, 16 },
                { 4, MPM.SINK, 3 },
                { 5, MPM.SINK, 3 },
                { 6, MPM.SINK, 3 },
                { 7, MPM.SINK, 3 },
                { 8, MPM.SINK, 3 },
                { 9, MPM.SINK, 3 },
                { 10, MPM.SINK, 3 },
                { 11, MPM.SINK, 3 },
                { 12, MPM.SINK, 3 },
                { 13, MPM.SINK, 3 },
                { 14, MPM.SINK, 3 }
            };
            MPM.Graph flowGraph = DoubleArrayToGraph(data);
            int answer = 30;
            Assert.AreEqual(answer, MPM.MaxFlow(flowGraph));
        }

        [TestMethod]
        public void Medium01WithLoop()
        {

            int[,] data = new int[,] {
                { MPM.SOURCE, 2, 8 },
                { MPM.SOURCE, 4, 7 },
                { MPM.SOURCE, 6, 10 },
                { MPM.SOURCE, 7, 12 },
                { 2, 8, 4 },
                { 2, 9, 3 },
                { 2, 11, 8 },
                { 3, 11, 2 },
                { 3, 12, 2 },
                { 4, 9, 2 },
                { 4, 10, 3 },
                { 4, 12, 5 },
                { 12, 4, 5 },
                { 6, 8, 4 },
                { 6, 9, 2 },
                { 7, 8, 3 },
                { 7, 9, 6 },
                { 7, 10, 4 },
                { 8, MPM.SINK, 7 },
                { 9, MPM.SINK, 6 },
                { 9, 3, 4 },
                { 10, MPM.SINK, 4 },
                { 11, MPM.SINK, 9 },
                { 12, MPM.SINK, 15 }
            };
            MPM.Graph flowGraph = DoubleArrayToGraph(data);
            int answer = 33;
            Assert.AreEqual(answer, MPM.MaxFlow(flowGraph));
        }

        [TestMethod]
        public void Medium01()
        {

            int[,] data = new int[,] {
                { MPM.SOURCE, 2, 8 },
                { MPM.SOURCE, 4, 7 },
                { MPM.SOURCE, 6, 10 },
                { MPM.SOURCE, 7, 12 },
                { 2, 8, 4 },
                { 2, 9, 3 },
                { 2, 11, 8 },
                { 3, 11, 2 },
                { 3, 12, 2 },
                { 4, 9, 2 },
                { 4, 10, 3 },
                { 4, 12, 5 },
                { 6, 8, 4 },
                { 6, 9, 2 },
                { 7, 8, 3 },
                { 7, 9, 6 },
                { 7, 10, 4 },
                { 8, MPM.SINK, 7 },
                { 9, MPM.SINK, 6 },
                { 9, 3, 4 },
                { 10, MPM.SINK, 4 },
                { 11, MPM.SINK, 9 },
                { 12, MPM.SINK, 15 }
            };
            MPM.Graph flowGraph = DoubleArrayToGraph(data);
            int answer = 33;
            Assert.AreEqual(answer, MPM.MaxFlow(flowGraph));
        }

        public MPM.Graph DoubleArrayToGraph(int[,] data)
        {
            var flowGraph = new MPM.Graph();
            for (int i = 0; i < data.GetLength(0); i++)
            {
                flowGraph.AddEdge(data[i, 0], data[i, 1], data[i, 2]);
            }
            return flowGraph;
        }

        [TestMethod]
        public void Small01()
        {
            int[,] data = new int[,] {
                { MPM.SOURCE, 2, 256 },
                { MPM.SOURCE, 3, 256 },
                { 2, 6, 256 },
                { 6, MPM.SINK, 256 },
                { 3, 7, 128 },
                { 7, MPM.SINK, 256 },
                { 2, 5, 128 },
                { 5, 7, 128 },
                { 3, 4, 128 },
                { 4, 6, 128 }
            };
            MPM.Graph flowGraph = DoubleArrayToGraph(data);
            int answer = 512;
            Assert.AreEqual(answer, MPM.MaxFlow(flowGraph));
        }

        [TestMethod]
        public void ResidualGraphWithLoop()
        {
            MPM.Graph G = new MPM.Graph();
            G.AddEdge(0, 1, 2, 2);
            G.AddEdge(1, 2, 3, 2);
            G.AddEdge(2, 1, 1, 0);
            G.AddEdge(2, 3, 2, 2);

            MPM.Graph resG = G.GetResidualGraph();
            Assert.IsTrue(resG.HasEdge(2, 1, 3));
        }

        [TestMethod]
        public void Small05WithLoop()
        {
            int[,] data = new int[,] {
                { MPM.SOURCE, 1, 1 },
                { MPM.SOURCE, 2, 2 },
                { 1, 3, 1 },
                { 2, 3, 2 },
                { 2, 4, 1 },
                { 3, MPM.SINK, 2 },
                { 4, MPM.SINK, 1 },
                {4, 5, 1 },
                {5,6,1 },
                {6,2,1 }
            };
            MPM.Graph G = DoubleArrayToGraph(data);
            int answer = 3;
            Assert.AreEqual(answer, MPM.MaxFlow(G));
        }

        [TestMethod]
        public void Small05()
        {
            int[,] data = new int[,] {
                { MPM.SOURCE, 1, 1 },
                { MPM.SOURCE, 2, 2 },
                { 1, 3, 1 },
                { 2, 3, 2 },
                { 2, 4, 1 },
                { 3, MPM.SINK, 2 },
                { 4, MPM.SINK, 1 }
            };
            MPM.Graph G = DoubleArrayToGraph(data);
            int answer = 3;
            Assert.AreEqual(answer, MPM.MaxFlow(G));
        }
    }

    [TestClass]

    public class ParallelTests
    {
        [TestMethod]
        public void TestParallelAdd()
        {
            double x = 0;
            Parallel.For(0, 1000, (i, state) =>
            {
                Utilities.ParallelAdd(ref x, 1);
            }
            );
            Assert.AreEqual(1000, x);
        }
    }

    [TestClass]
    public class ProfileTests
    {
        [TestMethod]
        public void ICProfileRightDimensions()
        {
            int numberOfAgents = 3;
            int numberOfCanidates = 4;
            Profile prof = Profile.GenerateICProfile(numberOfAgents, numberOfCanidates);
            Assert.AreEqual(prof.NumberOfCandidates, numberOfCanidates);
            Assert.AreEqual(prof.NumberOfVoters, numberOfAgents);

            numberOfAgents = 4;
            numberOfCanidates = 3;
            prof = Profile.GenerateICProfile(numberOfAgents, numberOfCanidates);
            Assert.AreEqual(prof.NumberOfCandidates, numberOfCanidates);
            Assert.AreEqual(prof.NumberOfVoters, numberOfAgents);
        }

       
        
       


        [TestMethod]
        public void TestAgentIChoice()
        {
            int[,] profileArray = { { 0, 1, 2, 3 }, { 1, 2, 0, 3 }, { 2, 0, 1, 3 } };
            Profile profile = new Profile(profileArray);

            Assert.AreEqual(2, profile.AgentsIthChoice(1, 1));
        }

 

       

        [TestMethod]
        public void ProfileFromArray()
        {
            int[,] profileArray = { { 0, 1, 2, 3 }, { 1, 2, 0, 3 }, { 2, 0, 1, 3 } };
            Profile profile = new Profile(profileArray);

            Assert.AreEqual(profileArray.GetLength(0), profile.NumberOfVoters);
            Assert.AreEqual(profileArray.GetLength(1), profile.NumberOfCandidates);

            for (int agent = 0; agent < profileArray.GetLength(0); agent++)
            {
                for (int candidate = 0; candidate < profileArray.GetLength(1); candidate++)
                {
                    Assert.AreEqual(profile.AgentsIthChoice(agent, candidate), profileArray[agent, candidate]);
                }
            }
        }

        [TestMethod]
        public void GetVoters()
        {
            int[,] profileArray = { { 0, 1, 2, 3 }, { 1, 2, 0, 3 }, { 2, 0, 1, 3 } };
            Profile profile = new Profile(profileArray);

            Assert.IsTrue(new HashSet<int>(profile.Voters).SetEquals(Enumerable.Range(0, profileArray.GetLength(0))));
        }

        [TestMethod]
        public void GetCandidates()
        {
            int[,] profileArray = { { 0, 1, 2, 3 }, { 1, 2, 0, 3 }, { 2, 0, 1, 3 } };
            Profile profile = new Profile(profileArray);

            Assert.IsTrue(new HashSet<int>(profile.Candidates).SetEquals(Enumerable.Range(0, profileArray.GetLength(1))));
        }

    }

   
    
    [TestClass]
    public class WorstCandidateTrackerTests
    {
        [TestMethod]
        public void CorrectIndexAfterRemoval()
        {
            var profile = new Profile(new int[,] { { 0, 1, 3, 2 }, { 1, 2, 0, 3 }, { 2, 0, 1, 3 } });
            var tracker = new WorstCandidateTracker(profile);

            tracker.RemoveWorstCandidate(2);
            Assert.AreEqual(tracker.IndexOfWorstCandidate(0), 3);
            Assert.AreEqual(tracker.IndexOfWorstCandidate(1), 2);
            Assert.AreEqual(tracker.IndexOfWorstCandidate(2), 2);

            tracker.RemoveWorstCandidate(0);
            Assert.AreEqual(tracker.IndexOfWorstCandidate(0), 1);
            Assert.AreEqual(tracker.IndexOfWorstCandidate(1), 2);
            Assert.AreEqual(tracker.IndexOfWorstCandidate(2), 2);

            tracker.RemoveWorstCandidate(1);
            Assert.AreEqual(tracker.IndexOfWorstCandidate(0), 1);
            Assert.AreEqual(tracker.IndexOfWorstCandidate(1), 0);
            Assert.AreEqual(tracker.IndexOfWorstCandidate(2), 2);
        }

        [TestMethod]
        public void CorrectCandidatesAfterRemoval()
        {
            var profile = new Profile(new int[,] { { 0, 1, 2, 3 }, { 1, 2, 0, 3 }, { 2, 0, 1, 3 } });
            var tracker = new WorstCandidateTracker(profile);

            var RemainingCandidates = new HashSet<int>(new int[] { 0, 1, 2, 3 });

            tracker.RemoveWorstCandidate(2);
            RemainingCandidates.Remove(3);
            Assert.IsTrue(RemainingCandidates.SetEquals(tracker.RemainingCandidates));

            tracker.RemoveWorstCandidate(0);
            RemainingCandidates.Remove(2);
            Assert.IsTrue(RemainingCandidates.SetEquals(tracker.RemainingCandidates));

            tracker.RemoveWorstCandidate(1);
            RemainingCandidates.Remove(0);
            Assert.IsTrue(RemainingCandidates.SetEquals(tracker.RemainingCandidates));
        }
    }

    [TestClass]
    public class HungryAgentTests
    {
        [TestMethod]
        public void TwoAgentLottery()
        {
            var profile = new Profile(new int[,] { { 0, 1, 2, 3, 4, 5 }, { 5, 4, 3, 2, 1, 0 } });
            var lottery = VotingFunctions.FindVetoByConsumptionLottery(profile);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 2, 3 });
            Assert.IsTrue(actualWinner.SetEquals(lottery.Keys()));
            Assert.AreEqual(lottery[2], 0.5);
            Assert.AreEqual(lottery[3], 0.5);
        }

        [TestMethod]
        public void RationalNumbersAreImportant()
        {
            var profile = new Profile(new int[,] {
                { 2, 0, 3, 1 },
                { 2, 3, 0, 1 },
                { 2, 3, 0, 1 },
                { 0, 1, 3, 2} });
            var lottery = VotingFunctions.FindVetoByConsumptionLottery(profile);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 2, 3 });
            Assert.IsTrue(actualWinner.SetEquals(lottery.Keys()));
            Assert.AreEqual(lottery[2], 0.25);
            Assert.AreEqual(lottery[3], 0.75);
        }

        [TestMethod]
        public void OneIsANumber()
        {
            var profile = new Profile(new int[,] {
                { 0, 1 },
                { 1, 0 },
                { 0, 1 }});
            var lottery = VotingFunctions.FindVetoByConsumptionLottery(profile);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0 });
            Assert.IsTrue(actualWinner.SetEquals(lottery.Keys()));
            Assert.AreEqual(lottery[0], 1);

        }

        [TestMethod]
        public void TwoAgentsOneWinnerSimple()
        {
            var profile = new Profile(new int[,] { { 0, 1, 2, 3, 4, 5, 6 }, { 6, 5, 4, 3, 2, 1, 0 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoByConsumptionWinners(profile);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 3 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void TwoAgentsOneWinnerComplex()
        {
            var profile = new Profile(new int[,] { { 0, 1, 2, 3, 4, 5, 6 }, { 2, 5, 1, 0, 3, 4, 6 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoByConsumptionWinners(profile);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 1 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void TwoAgentsTwoWinnersSimple()
        {
            var profile = new Profile(new int[,] { { 0, 1, 2, 3, 4, 5 }, { 5, 4, 3, 2, 1, 0 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoByConsumptionWinners(profile);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 2, 3 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void TwoAgentsTwoWinnersComplex()
        {
            var profile = new Profile(new int[,] { { 0, 1, 2, 3, 4, 5 }, { 2, 5, 4, 0, 3, 1 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoByConsumptionWinners(profile);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0, 2 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void ThreeAgentsOneWinner()
        {
            var profile = new Profile(new int[,] {
                { 0, 1, 2, 3, 4, 5, 6 },
                { 6, 1, 4, 3, 2, 5, 0 },
                { 6, 2, 4, 3, 1, 5, 0 }
            });
            IEnumerable<int> winner = VotingFunctions.FindVetoByConsumptionWinners(profile);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 3 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void ThreeAgentsThreeWinnersLottery()
        {
            var profile = new Profile(new int[,] {
                { 0, 1, 2, 3, 4, 5, 6 },
                { 0, 5, 4, 3, 2, 1, 6 },
                { 5, 4, 2, 1, 0, 3, 6 }
            });
            var winner = VotingFunctions.FindVetoByConsumptionLottery(profile);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0, 2, 4 });
            Assert.IsTrue(actualWinner.SetEquals(winner.Keys()));

            Assert.AreEqual(winner[0], Rational.One / 3);
            Assert.AreEqual(winner[2], Rational.One / 3);
            Assert.AreEqual(winner[4], Rational.One / 3);
        }

        [TestMethod]
        public void ThreeAgentsTwoWinnersLottery()
        {
            var profile = new Profile(new int[,] {
                { 1, 2, 0 },
                { 1, 2, 0 },
                { 2, 0, 1 }
            });
            var winner = VotingFunctions.FindVetoByConsumptionLottery(profile);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 1, 2 });
            Assert.IsTrue(actualWinner.SetEquals(winner.Keys()));

            Assert.AreEqual(winner[1], Rational.One / 3);
            Assert.AreEqual(winner[2], Rational.One * 2 / 3);
        }

        [TestMethod]
        public void ThreeAgentsThreeWinners()
        {
            var profile = new Profile(new int[,] {
                { 0, 1, 2, 3, 4, 5, 6 },
                { 0, 5, 4, 3, 2, 1, 6 },
                { 5, 4, 2, 1, 0, 3, 6 }
            });
            IEnumerable<int> winner = VotingFunctions.FindVetoByConsumptionWinners(profile);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0, 2, 4 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void ManyAgentsFewCandidates()
        {
            var profile = new Profile(new int[,] {
                { 1, 0, 2 },
                { 1, 0, 2 },
                { 1, 0, 2 },
                { 1, 0, 2 },
                { 1, 0, 2 },
                { 0, 2, 1 },
                { 0, 2, 1 },
                { 0, 2, 1 },
                { 1, 2, 0 },
                { 1, 2, 0 }
            });
            IEnumerable<int> winners = VotingFunctions.FindVetoByConsumptionWinners(profile);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 1 });
            Assert.IsTrue(actualWinner.SetEquals(winners));
        }

        [TestMethod]
        public void EveryonesAWinner()
        {
            var profile = new Profile(new int[,] { { 0, 1, 2 }, { 1, 2, 0 }, { 2, 0, 1 } });
            IEnumerable<int> winners = VotingFunctions.FindVetoByConsumptionWinners(profile);

            HashSet<int> allCandidates = new HashSet<int>(profile.Candidates);
            Assert.IsTrue(allCandidates.SetEquals(winners));
        }

        [TestMethod]
        public void AlekseiHA1()
        {
            var profile = new Profile(new int[,] { { 0, 1, 2, 3, 4, 5, 6 }, { 6, 4, 2, 0, 1, 3, 5 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoByConsumptionWinners(profile);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiHA2()
        {
            var profile = new Profile(new int[,] { { 0, 1, 2, 3, 4, 5, 6 }, { 2, 6, 5, 4, 3, 1, 0 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoByConsumptionWinners(profile);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 2 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiHA3()
        {
            var profile = new Profile(new int[,] {
                { 0, 1, 2, 3, 4, 5, 6 },
                { 0, 1, 2, 3, 4, 5, 6 },
                { 0, 1, 2, 3, 4, 5, 6 },
                { 0, 2, 4, 3, 1, 5, 6 },
                { 0, 1, 4, 3, 2, 5, 6 },
                { 1, 2, 4, 3, 0, 5, 6 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoByConsumptionWinners(profile);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiHA4()
        {
            var profile = new Profile(new int[,] {
                { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                { 6, 7, 5, 3, 0, 8, 9, 10, 4, 2, 1 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoByConsumptionWinners(profile);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 5 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiHA5()
        {
            var profile = new Profile(new int[,] {
                { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                { 2, 0, 1, 3, 4, 5, 6, 7, 8, 9 },
                { 4, 5, 6, 7, 8, 9, 3, 2, 1, 0 }});
            IEnumerable<int> winner = VotingFunctions.FindVetoByConsumptionWinners(profile);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 3 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void ImplementationCounterexample()
        {
            var profile = new Profile(new int[,] {
                { 0, 2, 3, 1 },
                { 3, 0, 1, 2 },
                { 3, 0, 2, 1 }});
            IEnumerable<int> winner = VotingFunctions.FindVetoByConsumptionWinners(profile);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 3 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }
    }

    [TestClass]
    public class VetoCoreTests
    {
        [TestMethod]
        public void FourAgentsFiveCandidatesMFLP()
        {
            var profile = new Profile(new int[,] {
                { 4, 1, 2, 3, 0 },
                { 1, 4, 2, 3, 0 },
                { 3, 1, 4, 2, 0 },
                { 0, 2, 3, 4, 1 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.MaxFlowLP);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 4 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void FourAgentsFiveCandidatesMF()
        {
            var profile = new Profile(new int[,] {
                { 4, 1, 2, 3, 0 },
                { 1, 4, 2, 3, 0 },
                { 3, 1, 4, 2, 0 },
                { 0, 2, 3, 4, 1 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.MaxFlow);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 4 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void FourAgentsFiveCandidatesLP()
        {
            var profile = new Profile(new int[,] {
                { 4, 1, 2, 3, 0 },
                { 1, 4, 2, 3, 0 },
                { 3, 1, 4, 2, 0 },
                { 0, 2, 3, 4, 1 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.LinearProgramming);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 4 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void FourAgentsFiveCandidatesK()
        {
            var profile = new Profile(new int[,] {
                { 4, 1, 2, 3, 0 },
                { 1, 4, 2, 3, 0 },
                { 3, 1, 4, 2, 0 },
                { 0, 2, 3, 4, 1 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.Konig);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 4 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void FiveAgentsFiveCandidatesMFLP()
        {
            var profile = new Profile(new int[,] {
                { 4, 1, 2, 3, 0 },
                { 1, 4, 2, 3, 0 },
                { 3, 1, 4, 2, 0 },
                { 0, 2, 3, 4, 1 },
                {2, 0, 3, 1, 4 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.MaxFlowLP);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 1, 2, 3, 4 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void FiveAgentsFiveCandidatesMF()
        {
            var profile = new Profile(new int[,] {
                { 4, 1, 2, 3, 0 },
                { 1, 4, 2, 3, 0 },
                { 3, 1, 4, 2, 0 },
                { 0, 2, 3, 4, 1 },
                {2, 0, 3, 1, 4 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.MaxFlow);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 1, 2, 3, 4 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void FiveAgentsFiveCandidatesLP()
        {
            var profile = new Profile(new int[,] {
                { 4, 1, 2, 3, 0 },
                { 1, 4, 2, 3, 0 },
                { 3, 1, 4, 2, 0 },
                { 0, 2, 3, 4, 1 },
                {2, 0, 3, 1, 4 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.LinearProgramming);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 1, 2, 3, 4 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void FiveAgentsFiveCandidatesK()
        {
            var profile = new Profile(new int[,] {
                { 4, 1, 2, 3, 0 },
                { 1, 4, 2, 3, 0 },
                { 3, 1, 4, 2, 0 },
                { 0, 2, 3, 4, 1 },
                {2, 0, 3, 1, 4 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.Konig);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 1, 2, 3, 4 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void ThreeAgentsFiveCandidatesMFLP()
        {
            var profile = new Profile(new int[,] {
                { 1, 4, 2, 3, 0 },
                { 3, 1, 4, 2, 0 },
                { 0, 2, 3, 4, 1 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.MaxFlowLP);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 3, 4 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void ThreeAgentsFiveCandidatesMF()
        {
            var profile = new Profile(new int[,] {
                { 1, 4, 2, 3, 0 },
                { 3, 1, 4, 2, 0 },
                { 0, 2, 3, 4, 1 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.MaxFlow);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 3, 4 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void ThreeAgentsFiveCandidatesLP()
        {
            var profile = new Profile(new int[,] {
                { 1, 4, 2, 3, 0 },
                { 3, 1, 4, 2, 0 },
                { 0, 2, 3, 4, 1 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.LinearProgramming);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 3, 4 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void ThreeAgentsFiveCandidatesK()
        {
            var profile = new Profile(new int[,] {
                { 1, 4, 2, 3, 0 },
                { 3, 1, 4, 2, 0 },
                { 0, 2, 3, 4, 1 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.Konig);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 3, 4 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiCore1MFLP()
        {
            var profile = new Profile(new int[,] { { 0, 1, 2, 3, 4, 5, 6 }, { 6, 4, 2, 0, 1, 3, 5 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.MaxFlowLP);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0, 2 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiCore1MF()
        {
            var profile = new Profile(new int[,] { { 0, 1, 2, 3, 4, 5, 6 }, { 6, 4, 2, 0, 1, 3, 5 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.MaxFlow);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0, 2 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiCore1LP()
        {
            var profile = new Profile(new int[,] { { 0, 1, 2, 3, 4, 5, 6 }, { 6, 4, 2, 0, 1, 3, 5 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.LinearProgramming);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0, 2 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiCore1K()
        {
            var profile = new Profile(new int[,] { { 0, 1, 2, 3, 4, 5, 6 }, { 6, 4, 2, 0, 1, 3, 5 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.Konig);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0, 2 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void MaxFlowErrorMFLP()
        {
            Profile counterExample = new Profile(
                new int[,] {
                    { 6,0,5,8,2,4,9,1,7,3 },
                    { 2,1,7,9,8,0,6,4,5,3 },
                    { 1,4,2,6,8,7,0,3,5,9 },
                    { 1,4,3,7,2,9,5,8,0,6},
                    { 2,3,8,4,9,7,0,1,6,5},
                    {8,3,6,2,7,0,5,1,4,9 },
                    { 0,6,8,9,3,7,4,1,2,5 },
                    {4,3,9,2,7,8,0,6,5,1 },
                    {5,1,8,6,2,4,0,9,7,3 }
                });

            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                counterExample,
                VotingFunctions.CoreAlgorithm.MaxFlowLP);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0, 2, 4, 8 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void MaxFlowErrorMF()
        {
            Profile counterExample = new Profile(
                new int[,] {
                    { 6,0,5,8,2,4,9,1,7,3 },
                    { 2,1,7,9,8,0,6,4,5,3 },
                    { 1,4,2,6,8,7,0,3,5,9 },
                    { 1,4,3,7,2,9,5,8,0,6},
                    { 2,3,8,4,9,7,0,1,6,5},
                    {8,3,6,2,7,0,5,1,4,9 },
                    { 0,6,8,9,3,7,4,1,2,5 },
                    {4,3,9,2,7,8,0,6,5,1 },
                    {5,1,8,6,2,4,0,9,7,3 }
                });

            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                counterExample,
                VotingFunctions.CoreAlgorithm.MaxFlow);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0, 2, 4, 8 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void MaxFlowErrorLP()
        {
            Profile counterExample = new Profile(
                new int[,] {
                    { 6,0,5,8,2,4,9,1,7,3 },
                    { 2,1,7,9,8,0,6,4,5,3 },
                    { 1,4,2,6,8,7,0,3,5,9 },
                    { 1,4,3,7,2,9,5,8,0,6},
                    { 2,3,8,4,9,7,0,1,6,5},
                    {8,3,6,2,7,0,5,1,4,9 },
                    { 0,6,8,9,3,7,4,1,2,5 },
                    {4,3,9,2,7,8,0,6,5,1 },
                    {5,1,8,6,2,4,0,9,7,3 }
                });

            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                counterExample,
                VotingFunctions.CoreAlgorithm.LinearProgramming);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0, 2, 4, 8 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void MaxFlowErrorK()
        {
            Profile counterExample = new Profile(
                new int[,] {
                    { 6,0,5,8,2,4,9,1,7,3 },
                    { 2,1,7,9,8,0,6,4,5,3 },
                    { 1,4,2,6,8,7,0,3,5,9 },
                    { 1,4,3,7,2,9,5,8,0,6},
                    { 2,3,8,4,9,7,0,1,6,5},
                    {8,3,6,2,7,0,5,1,4,9 },
                    { 0,6,8,9,3,7,4,1,2,5 },
                    {4,3,9,2,7,8,0,6,5,1 },
                    {5,1,8,6,2,4,0,9,7,3 }
                });

            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                counterExample,
                VotingFunctions.CoreAlgorithm.Konig);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0, 2, 4, 8 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiCore2MFLP()
        {
            var profile = new Profile(new int[,] { { 0, 1, 2, 3, 4, 5, 6 }, { 2, 6, 5, 4, 3, 1, 0 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.MaxFlowLP);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 2 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiCore2MF()
        {
            var profile = new Profile(new int[,] { { 0, 1, 2, 3, 4, 5, 6 }, { 2, 6, 5, 4, 3, 1, 0 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.MaxFlow);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 2 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiCore2LP()
        {
            var profile = new Profile(new int[,] { { 0, 1, 2, 3, 4, 5, 6 }, { 2, 6, 5, 4, 3, 1, 0 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.LinearProgramming);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 2 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiCore2K()
        {
            var profile = new Profile(new int[,] { { 0, 1, 2, 3, 4, 5, 6 }, { 2, 6, 5, 4, 3, 1, 0 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.Konig);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 2 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiCore3MFLP()
        {
            var profile = new Profile(new int[,] {
                { 0, 1, 2, 3, 4, 5, 6 },
                { 0, 1, 2, 3, 4, 5, 6 },
                { 0, 1, 2, 3, 4, 5, 6 },
                { 0, 2, 4, 3, 1, 5, 6 },
                { 0, 1, 4, 3, 2, 5, 6 },
                { 1, 2, 4, 3, 0, 5, 6 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.MaxFlowLP);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0, 1, 2, 3 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiCore3MF()
        {
            var profile = new Profile(new int[,] {
                { 0, 1, 2, 3, 4, 5, 6 },
                { 0, 1, 2, 3, 4, 5, 6 },
                { 0, 1, 2, 3, 4, 5, 6 },
                { 0, 2, 4, 3, 1, 5, 6 },
                { 0, 1, 4, 3, 2, 5, 6 },
                { 1, 2, 4, 3, 0, 5, 6 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.MaxFlow);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0, 1, 2, 3 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiCore3LP()
        {
            var profile = new Profile(new int[,] {
                { 0, 1, 2, 3, 4, 5, 6 },
                { 0, 1, 2, 3, 4, 5, 6 },
                { 0, 1, 2, 3, 4, 5, 6 },
                { 0, 2, 4, 3, 1, 5, 6 },
                { 0, 1, 4, 3, 2, 5, 6 },
                { 1, 2, 4, 3, 0, 5, 6 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.LinearProgramming);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0, 1, 2, 3 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiCore3K()
        {
            var profile = new Profile(new int[,] {
                { 0, 1, 2, 3, 4, 5, 6 },
                { 0, 1, 2, 3, 4, 5, 6 },
                { 0, 1, 2, 3, 4, 5, 6 },
                { 0, 2, 4, 3, 1, 5, 6 },
                { 0, 1, 4, 3, 2, 5, 6 },
                { 1, 2, 4, 3, 0, 5, 6 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.Konig);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0, 1, 2, 3 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiCore4MFLP()
        {
            var profile = new Profile(new int[,] {
                { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                { 6, 7, 5, 3, 0, 8, 9, 10, 4, 2, 1 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.MaxFlowLP);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0, 3, 5 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiCore4MF()
        {
            var profile = new Profile(new int[,] {
                { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                { 6, 7, 5, 3, 0, 8, 9, 10, 4, 2, 1 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.MaxFlow);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0, 3, 5 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiCore4LP()
        {
            var profile = new Profile(new int[,] {
                { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                { 6, 7, 5, 3, 0, 8, 9, 10, 4, 2, 1 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.LinearProgramming);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0, 3, 5 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiCore4K()
        {
            var profile = new Profile(new int[,] {
                { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                { 6, 7, 5, 3, 0, 8, 9, 10, 4, 2, 1 } });
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.Konig);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 0, 3, 5 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiCore5MFLP()
        {
            var profile = new Profile(new int[,] {
                { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                { 2, 0, 1, 3, 4, 5, 6, 7, 8, 9 },
                { 4, 5, 6, 7, 8, 9, 3, 2, 1, 0 }});
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.MaxFlowLP);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 3 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiCore5MF()
        {
            var profile = new Profile(new int[,] {
                { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                { 2, 0, 1, 3, 4, 5, 6, 7, 8, 9 },
                { 4, 5, 6, 7, 8, 9, 3, 2, 1, 0 }});
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.MaxFlow);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 3 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiCore5LP()
        {
            var profile = new Profile(new int[,] {
                { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                { 2, 0, 1, 3, 4, 5, 6, 7, 8, 9 },
                { 4, 5, 6, 7, 8, 9, 3, 2, 1, 0 }});
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.LinearProgramming);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 3 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }

        [TestMethod]
        public void AlekseiCore5K()
        {
            var profile = new Profile(new int[,] {
                { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                { 2, 0, 1, 3, 4, 5, 6, 7, 8, 9 },
                { 4, 5, 6, 7, 8, 9, 3, 2, 1, 0 }});
            IEnumerable<int> winner = VotingFunctions.FindVetoCore(
                profile,
                VotingFunctions.CoreAlgorithm.Konig);

            HashSet<int> actualWinner = new HashSet<int>(new int[] { 3 });
            Assert.IsTrue(actualWinner.SetEquals(winner));
        }
    }

   
    [TestClass]
    public class BicliqueLPTests
    {
        [TestMethod]
        public void K_4_4()
        {
            List<List<int>> nonAdjacencyList = new List<List<int>>()
            {
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>()
            };
            int verticesOnLeft = 4;
            int verticesOnRight = 4;

            int bicliqueSize = Utilities.LargestBicliqueViaLinearProgramming(nonAdjacencyList, verticesOnLeft, verticesOnRight);

            Assert.AreEqual(bicliqueSize, 8);
        }

        [TestMethod]
        public void K_3_4()
        {
            List<List<int>> nonAdjacencyList = new List<List<int>>()
            {
                new List<int>(),
                new List<int>(),
                new List<int>()
            };
            int verticesOnLeft = 3;
            int verticesOnRight = 4;

            int bicliqueSize = Utilities.LargestBicliqueViaLinearProgramming(nonAdjacencyList, verticesOnLeft, verticesOnRight);

            Assert.AreEqual(bicliqueSize, 7);
        }

        [TestMethod]
        public void K_4_3()
        {
            List<List<int>> nonAdjacencyList = new List<List<int>>()
            {
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>()
            };
            int verticesOnLeft = 4;
            int verticesOnRight = 3;

            int bicliqueSize = Utilities.LargestBicliqueViaLinearProgramming(nonAdjacencyList, verticesOnLeft, verticesOnRight);

            Assert.AreEqual(bicliqueSize, 7);
        }

        [TestMethod]
        public void K_3_4Subgraph()
        {
            List<List<int>> nonAdjacencyList = new List<List<int>>()
            {
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>() {2,3 }
            };
            int verticesOnLeft = 4;
            int verticesOnRight = 4;

            int bicliqueSize = Utilities.LargestBicliqueViaLinearProgramming(nonAdjacencyList, verticesOnLeft, verticesOnRight);

            Assert.AreEqual(bicliqueSize, 7);
        }

        [TestMethod]
        public void K_2_3Subgraph()
        {
            List<List<int>> nonAdjacencyList = new List<List<int>>()
            {
                new List<int>() {0 },
                new List<int>() {0 },
                new List<int>() {0,1,2,3 },
                new List<int>() {1,2 }
            };
            int verticesOnLeft = 4;
            int verticesOnRight = 4;

            int bicliqueSize = Utilities.LargestBicliqueViaLinearProgramming(nonAdjacencyList, verticesOnLeft, verticesOnRight);

            Assert.AreEqual(bicliqueSize, 5);
        }

        [TestMethod]
        public void K_3_2Subgraph()
        {
            List<List<int>> nonAdjacencyList = new List<List<int>>()
            {
                new List<int>() {0,2 },
                new List<int>() {0,2 },
                new List<int>() {0 },
                new List<int>() {1,2 }
            };
            int verticesOnLeft = 4;
            int verticesOnRight = 4;

            int bicliqueSize = Utilities.LargestBicliqueViaLinearProgramming(nonAdjacencyList, verticesOnLeft, verticesOnRight);

            Assert.AreEqual(bicliqueSize, 5);
        }
    }

   
    [TestClass]
    public class BicliqueKonigTests
    {
        [TestMethod]
        public void K_4_4()
        {
            List<List<int>> nonAdjacencyList = new List<List<int>>()
            {
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>()
            };
            int verticesOnLeft = 4;
            int verticesOnRight = 4;

            int bicliqueSize = Utilities.LargestBicliqueViaKonig(nonAdjacencyList, verticesOnLeft, verticesOnRight);

            Assert.AreEqual(bicliqueSize, 8);
        }

        [TestMethod]
        public void K_3_4()
        {
            List<List<int>> nonAdjacencyList = new List<List<int>>()
            {
                new List<int>(),
                new List<int>(),
                new List<int>()
            };
            int verticesOnLeft = 3;
            int verticesOnRight = 4;

            int bicliqueSize = Utilities.LargestBicliqueViaKonig(nonAdjacencyList, verticesOnLeft, verticesOnRight);

            Assert.AreEqual(bicliqueSize, 7);
        }

        [TestMethod]
        public void K_4_3()
        {
            List<List<int>> nonAdjacencyList = new List<List<int>>()
            {
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>()
            };
            int verticesOnLeft = 4;
            int verticesOnRight = 3;

            int bicliqueSize = Utilities.LargestBicliqueViaKonig(nonAdjacencyList, verticesOnLeft, verticesOnRight);

            Assert.AreEqual(bicliqueSize, 7);
        }

        [TestMethod]
        public void K_3_4Subgraph()
        {
            List<List<int>> nonAdjacencyList = new List<List<int>>()
            {
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>() {2,3 }
            };
            int verticesOnLeft = 4;
            int verticesOnRight = 4;

            int bicliqueSize = Utilities.LargestBicliqueViaKonig(nonAdjacencyList, verticesOnLeft, verticesOnRight);

            Assert.AreEqual(bicliqueSize, 7);
        }

        [TestMethod]
        public void K_2_3Subgraph()
        {
            List<List<int>> nonAdjacencyList = new List<List<int>>()
            {
                new List<int>() {0 },
                new List<int>() {0 },
                new List<int>() {0,1,2,3 },
                new List<int>() {1,2 }
            };
            int verticesOnLeft = 4;
            int verticesOnRight = 4;

            int bicliqueSize = Utilities.LargestBicliqueViaKonig(nonAdjacencyList, verticesOnLeft, verticesOnRight);

            Assert.AreEqual(bicliqueSize, 5);
        }

        [TestMethod]
        public void K_3_2Subgraph()
        {
            List<List<int>> nonAdjacencyList = new List<List<int>>()
            {
                new List<int>() {0,2 },
                new List<int>() {0,2 },
                new List<int>() {0 },
                new List<int>() {1,2 }
            };
            int verticesOnLeft = 4;
            int verticesOnRight = 4;

            int bicliqueSize = Utilities.LargestBicliqueViaKonig(nonAdjacencyList, verticesOnLeft, verticesOnRight);

            Assert.AreEqual(bicliqueSize, 5);
        }
    }

    [TestClass]
    public class MoulinCoefficientsTests
    {
        [TestMethod]
        public void SimpleLessAgents()
        {
            (int, int) Coeff = Utilities.GetMoulinCoefficients(3, 10);

            Assert.IsTrue(Coeff.Item1 * 3 == Coeff.Item2 * 10 - 1);
            Assert.IsTrue(Coeff.Item2 > 3 * 1);
            Assert.IsTrue(Coeff.Item2 - 3 <= 3 * 1);
        }

        [TestMethod]
        public void SimpleLessCandidates()
        {
            (int, int) Coeff = Utilities.GetMoulinCoefficients(15, 4);

            Assert.IsTrue(Coeff.Item1 * 15 == Coeff.Item2 * 4 - 1);
            Assert.IsTrue(Coeff.Item2 > 15 * 1);
            Assert.IsTrue(Coeff.Item2 - 15 <= 15 * 1);
        }

        [TestMethod]
        public void CoPrimeLessCandidates()
        {
            (int, int) Coeff = Utilities.GetMoulinCoefficients(13, 11);

            Assert.IsTrue(Coeff.Item1 * 13 == Coeff.Item2 * 11 - 1);
            Assert.IsTrue(Coeff.Item2 > 13 * 1);
            Assert.IsTrue(Coeff.Item2 - 13 <= 13 * 1);
        }

        [TestMethod]
        public void CoPrimeLessAgents()
        {
            (int, int) Coeff = Utilities.GetMoulinCoefficients(7, 25);

            Assert.IsTrue(Coeff.Item1 * 7 == Coeff.Item2 * 25 - 1);
            Assert.IsTrue(Coeff.Item2 > 7 * 1);
            Assert.IsTrue(Coeff.Item2 - 7 <= 7 * 1);
        }

        [TestMethod]
        public void DivisorsLessCandidates()
        {
            (int, int) Coeff = Utilities.GetMoulinCoefficients(15, 10);

            Assert.IsTrue(Coeff.Item1 * 15 == Coeff.Item2 * 10 - 5);
            Assert.IsTrue(Coeff.Item2 > 15 * 5);
            Assert.IsTrue(Coeff.Item2 - 15 <= 15 * 5);
        }

        [TestMethod]
        public void DivisorsLessAgents()
        {
            (int, int) Coeff = Utilities.GetMoulinCoefficients(4, 16);

            Assert.IsTrue(Coeff.Item1 * 4 == Coeff.Item2 * 16 - 4);
            Assert.IsTrue(Coeff.Item2 > 4 * 4);
            Assert.IsTrue(Coeff.Item2 - 4 <= 4 * 4);
        }

        [TestMethod]
        public void ComplexLessCandidates()
        {

            (int, int) Coeff = Utilities.GetMoulinCoefficients(32, 28);

            Assert.IsTrue(Coeff.Item1 * 32 == Coeff.Item2 * 28 - 4);
            Assert.IsTrue(Coeff.Item2 > 32 * 4);
            Assert.IsTrue(Coeff.Item2 - 32 <= 32 * 4);
        }

        [TestMethod]
        public void ComplexLessAgents()
        {
            (int, int) Coeff = Utilities.GetMoulinCoefficients(15, 65);

            Assert.IsTrue(Coeff.Item1 * 15 == Coeff.Item2 * 65 - 5);
            Assert.IsTrue(Coeff.Item2 > 15 * 5);
            Assert.IsTrue(Coeff.Item2 - 15 <= 15 * 5);
        }
    }

    [TestClass]
    public class BlockingGraphTests
    {

        [TestMethod]
        public void NoClones()
        {
            Profile profile = new Profile(new int[,]
            { { 0, 1, 2, 3, 4 },
            { 2, 1, 3, 4, 0 },
            {0, 4, 3, 1, 2 },
            { 4, 3, 2, 1, 0 },
            });

            List<List<int>> nonAdjacencyList = Utilities.GetBipartiteComplementOfBlockingGraph(profile, 1, 1, 1);

            Assert.IsTrue(nonAdjacencyList[0].Contains(1));
            Assert.IsTrue(nonAdjacencyList[0].Contains(2));
            Assert.IsTrue(nonAdjacencyList[0].Contains(3));

            Assert.IsTrue(nonAdjacencyList[1].Contains(0));
            Assert.IsTrue(nonAdjacencyList[1].Contains(2));
            Assert.IsTrue(nonAdjacencyList[1].Contains(3));

            Assert.IsTrue(nonAdjacencyList[2].Contains(1));

            Assert.IsTrue(nonAdjacencyList[3].Contains(0));
        }

        [TestMethod]
        public void CandidateClones()
        {
            Profile profile = new Profile(new int[,]
            { { 0, 1, 2, 3, 4 },
            { 2, 1, 3, 4, 0 },
            {0, 4, 3, 1, 2 },
            { 4, 3, 2, 1, 0 },
            });

            List<List<int>> nonAdjacencyList = Utilities.GetBipartiteComplementOfBlockingGraph(profile, 1, 1, 2);

            Assert.IsTrue(nonAdjacencyList[0].Contains(2));
            Assert.IsTrue(nonAdjacencyList[0].Contains(3));
            Assert.IsTrue(nonAdjacencyList[0].Contains(4));
            Assert.IsTrue(nonAdjacencyList[0].Contains(5));
            Assert.IsTrue(nonAdjacencyList[0].Contains(6));
            Assert.IsTrue(nonAdjacencyList[0].Contains(7));

            Assert.IsTrue(nonAdjacencyList[1].Contains(0));
            Assert.IsTrue(nonAdjacencyList[1].Contains(1));
            Assert.IsTrue(nonAdjacencyList[1].Contains(4));
            Assert.IsTrue(nonAdjacencyList[1].Contains(5));
            Assert.IsTrue(nonAdjacencyList[1].Contains(6));
            Assert.IsTrue(nonAdjacencyList[1].Contains(7));

            Assert.IsTrue(nonAdjacencyList[2].Contains(2));
            Assert.IsTrue(nonAdjacencyList[2].Contains(3));

            Assert.IsTrue(nonAdjacencyList[3].Contains(0));
            Assert.IsTrue(nonAdjacencyList[3].Contains(1));
        }

        [TestMethod]
        public void VoterClones()
        {
            Profile profile = new Profile(new int[,]
            { { 0, 1, 2, 3, 4 },
            { 2, 1, 3, 4, 0 },
            {0, 4, 3, 1, 2 },
            { 4, 3, 2, 1, 0 },
            });

            List<List<int>> nonAdjacencyList = Utilities.GetBipartiteComplementOfBlockingGraph(profile, 1, 2, 1);

            Assert.IsTrue(nonAdjacencyList[0].Contains(1));
            Assert.IsTrue(nonAdjacencyList[0].Contains(2));
            Assert.IsTrue(nonAdjacencyList[0].Contains(3));
            Assert.IsTrue(nonAdjacencyList[1].Contains(1));
            Assert.IsTrue(nonAdjacencyList[1].Contains(2));
            Assert.IsTrue(nonAdjacencyList[1].Contains(3));

            Assert.IsTrue(nonAdjacencyList[2].Contains(0));
            Assert.IsTrue(nonAdjacencyList[2].Contains(2));
            Assert.IsTrue(nonAdjacencyList[2].Contains(3));
            Assert.IsTrue(nonAdjacencyList[3].Contains(0));
            Assert.IsTrue(nonAdjacencyList[3].Contains(2));
            Assert.IsTrue(nonAdjacencyList[3].Contains(3));

            Assert.IsTrue(nonAdjacencyList[4].Contains(1));
            Assert.IsTrue(nonAdjacencyList[5].Contains(1));

            Assert.IsTrue(nonAdjacencyList[6].Contains(0));
            Assert.IsTrue(nonAdjacencyList[7].Contains(0));
        }

        [TestMethod]
        public void BothClonesRightSizes()
        {
            Profile profile = new Profile(new int[,]
            { { 0, 1, 2, 3, 4 },
            { 2, 1, 3, 4, 0 },
            {0, 4, 3, 1, 2 },
            { 4, 3, 2, 1, 0 },
            });

            List<List<int>> nonAdjacencyList = Utilities.GetBipartiteComplementOfBlockingGraph(profile, 3, 2, 2);

            Assert.AreEqual(nonAdjacencyList.Count, 8);
            Assert.AreEqual(nonAdjacencyList[0].Count, 2);
            Assert.AreEqual(nonAdjacencyList[2].Count, 4);
            Assert.AreEqual(nonAdjacencyList[4].Count, 4);
            Assert.AreEqual(nonAdjacencyList[6].Count, 6);

        }
    }

   
    public class AuxiliaryFunctions
    {
        public static void AssertArraysEqual(double[] arr1, double[] arr2)
        {
            Assert.AreEqual(arr1.Length, arr2.Length, "Arrays of different lengths");

            for (int i = 0; i < arr1.Length; i++)
            {
                Assert.AreEqual(arr1[i], arr2[i]);
            }
        }

    }


}
