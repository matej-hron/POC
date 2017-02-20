﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace hackerrank
{
    public class BotTheCleanerLarge
    {
        public struct Point
        {
            public int X { get; set; }
            public int Y { get; set; }

            public static Point From(int x, int y)
            {
                return new Point
                {
                    X = x,
                    Y = y
                };
            }

            public override string ToString()
            {
                return $"{X}, {Y}";
            }


        }


        public static void next_move(int posr, int posc, int dimh, int dimw, String[] board)
        {
            Console.Write(GetNextCommand(posr, posc, board));
        }

        public static string GetNextCommand(int posr, int posc, String[] board)
        {
            var botPosition = Point.From(posr, posc);
            var n = board[0].Length;
            //var nearestThrash = GetNearestThrash(board, n, botPosition);
            var nextThrash = GetShortestPath(botPosition, ExtractThrashCoords(board, n).ToArray()).Item1.Skip(1).FirstOrDefault();
            var distance = GetDist(botPosition, nextThrash);

            if (distance == 0)
                return "CLEAN";

            var delta = Point.From(nextThrash.X - botPosition.X, nextThrash.Y - botPosition.Y);

            var upOrDown = delta.X > 0 ? "DOWN" : "UP";
            var leftOrRight = delta.Y > 0 ? "RIGHT" : "LEFT";

            if (delta.X != 0)
                return upOrDown;

            if (delta.Y != 0)
                return leftOrRight;

            return "";

        }

        public static int GetDist(Point p1, Point p2)
            => Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);

        public static IEnumerable<Point> ExtractThrashCoords(String[] board, int n)
        {
            var boardLine = string.Join("", board);
            return boardLine.SelectMany((ch, i) => ch == 'd' ? ToEnumerable(IndexToCoords(n, i)) : Enumerable.Empty<Point>());
        }


        public static Tuple<Point[], int> GetShortestPath(Point origin, Point[] points)
        {
            if (points.Length == 0)
                return Tuple.Create(new Point[] {}, 0);

            if (points.Length == 1)
                return Tuple.Create(new[] {origin, points[0]}, GetDist(origin, points[0]));

            int min = int.MaxValue;
            Point[] path = {};

            foreach (var p in points)
            {
                var rest = GetShortestPath(p, points.Where(actual => actual.X != p.X || actual.Y != p.Y).ToArray());
                var d = GetDist(origin, p) + rest.Item2;

                if (d < min)
                {
                    min = d;
                    path = rest.Item1;
                }
            }

            var res = new List<Point> {origin};
            res.AddRange(path);
            return Tuple.Create(res.ToArray(), min);
        }
        

        public static IEnumerable<T> ToEnumerable<T>(T item)
        {
            yield return item;
        } 

        public static Point IndexToCoords(int n, int pos)
        {
            return Point.From(pos / n, pos % n);
        }


        public static Point GetNearestThrash(String[] board, int n, Point @from)
        {
            var thrash = ExtractThrashCoords(board, n);

            var p = Point.From(-1,-1);
            var min = Int32.MaxValue;

            foreach (var t in thrash)
            {
                var d = GetDist(@from, t);
                
                if (d < min)
                {
                    min = d;
                    p = t;
                }
            }

            return p;
        }
    }

    [TestFixture]
    public class BobTheCleanerLargeTestFixture
    {


        [Test]
        public void GetShortestPathTest()
        {
            var origin = BotTheCleanerLarge.Point.From(0, 0);
            var points = new BotTheCleanerLarge.Point[] {};

            var res = BotTheCleanerLarge.GetShortestPath(origin, points);

            Assert.That(res.Item2, Is.EqualTo(0));
        }

        [Test]
        public void GetShortestPath2()
        {
            var origin = BotTheCleanerLarge.Point.From(0, 0);
            var points = new[] { BotTheCleanerLarge.Point.From(1, 1) };

            var res = BotTheCleanerLarge.GetShortestPath(origin, points);

            Assert.That(res.Item2, Is.EqualTo(2));
        }


        [Test]
        public void GetShortestPath3()
        {
            var origin = BotTheCleanerLarge.Point.From(0, 0);
            var points = new[]
            {
                BotTheCleanerLarge.Point.From(1, 1),
                BotTheCleanerLarge.Point.From(2, 2)
            };

            var res = BotTheCleanerLarge.GetShortestPath(origin, points);

            Assert.That(res.Item2, Is.EqualTo(4));
            Assert.That(res.Item1.Skip(1), Is.EqualTo(new[]
            {
                //BotTheCleanerLarge.Point.From(0, 0),
                BotTheCleanerLarge.Point.From(1, 1),
                BotTheCleanerLarge.Point.From(2, 2),

            }));
        }

        [Test]
        public void GetShortestPath4()
        {
            var origin = BotTheCleanerLarge.Point.From(0, 0);
            var points = new[]
            {
                BotTheCleanerLarge.Point.From(3, 3),
                BotTheCleanerLarge.Point.From(2, 0),
                BotTheCleanerLarge.Point.From(1, 2),

            };
            var res = BotTheCleanerLarge.GetShortestPath(origin, points);

            Assert.That(res.Item2, Is.EqualTo(8));
            Assert.That(res.Item1.Skip(1), Is.EqualTo(new[]
            {
                //BotTheCleanerLarge.Point.From(0, 0),
                
                BotTheCleanerLarge.Point.From(2, 0),
                BotTheCleanerLarge.Point.From(1, 2),
                BotTheCleanerLarge.Point.From(3, 3),

            }));
        }

        //[TestCase(5, 5, 10)]
        [TestCase(10, 10, 10)]
        //[TestCase(20, 20, 10)]
        public void GetShortestPathCanWorkOnLargeGrid(int columns, int rows, int percentage)
        {
            var origin = BotTheCleanerLarge.Point.From(0, 0);
            var board = GenerateBigGrid(columns, rows, percentage);
            var thrash = BotTheCleanerLarge.ExtractThrashCoords(board, 10).ToArray();
            var res = BotTheCleanerLarge.GetShortestPath(origin, thrash);
            //var r = Visualise(res.Item1, columns, rows);
        }

        private string[] GenerateBigGrid(int columns, int rows, int probabilityOfDirt)
        {
            return Enumerable.Range(0, rows).
                Select(r => new string(Enumerable.Range(0, columns).
                    Select(c => ShouldGenerateDirt(probabilityOfDirt) ? 'd' : '-').ToArray())).ToArray();
        }

        private Random rnd = new Random();

        private bool ShouldGenerateDirt(int probabilityOfDirt) => rnd.Next(100+1) < probabilityOfDirt;
            

        [TestCase(1,1,2,2,2)]
        [TestCase(1,1,1,1,0)]
        [TestCase(1,1,-1,-1,4)]
        public void CanCalculateDistanceBetweenPoints(int x1, int y1, int x2, int y2, int expected)
        {
            Assert.That(BotTheCleaner.GetDist(BotTheCleaner.Point.From(x1, y1), BotTheCleaner.Point.From(x2, y2)), Is.EqualTo(expected));
        }

        [Test]
        public void CanExtractThrashCoords()
        {
            Assert.That(BotTheCleaner.ExtractThrashCoords(grid, 5), Is.EquivalentTo(new[]
            {
                BotTheCleaner.Point.From(0, 4),
                BotTheCleaner.Point.From(1, 1),
                BotTheCleaner.Point.From(1, 4),
                BotTheCleaner.Point.From(2, 2),
                BotTheCleaner.Point.From(2, 3),
                BotTheCleaner.Point.From(3, 2),
                BotTheCleaner.Point.From(4, 4)
            }));
        }

        [TestCase(5, 6, 1, 1)]
        [TestCase(5, 9, 1, 4)]
        public void CanCalculatePosFromIndex(int n, int pos, int x, int y)
        {
            Assert.That(BotTheCleaner.IndexToCoords(n, pos), Is.EqualTo(BotTheCleaner.Point.From(x, y)));
        }

        [Test]
        public void CanFindNearestThrash()
        {
            var p = BotTheCleaner.GetNearestThrash(grid, 5, BotTheCleaner.Point.From(0, 0));
            Assert.That(
                p, 
                Is.EqualTo(BotTheCleaner.Point.From(1, 1)));
        }

        [TestCase(0, 0, "DOWN")]
        [TestCase(1, 1, "CLEAN")]
        public void BotEmitsCorrectAction(int x, int y, string expected)
        {
            Assert.That(BotTheCleaner.GetNextCommand(x, y, grid), Is.EqualTo(expected));
        }

        private readonly string[] grid = new[]
      {
            "b---d",
            "-d--d",
            "--dd-",
            "--d--",
            "----d",
        };
    }
}
