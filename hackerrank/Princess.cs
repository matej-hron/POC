using System;
using System.Linq;
using NUnit.Framework;

namespace hackerrank
{
    public class Princess
    {
        public static void DisplayPathtoPrincess(int n, String[] grid)
        {
            Console.Write(string.Join("\n", GetPathToPrincess(n, grid)));
        }

        public static string[] GetPathToPrincess(int n, String[] grid)
        {
            var princessCoords = GetCoordinates(n, grid, 'p');
            var botCoords = GetCoordinates(n, grid, 'm');
            var delta = Tuple.Create(princessCoords.Item1 - botCoords.Item1, princessCoords.Item2 - botCoords.Item2);
            var upOrDown = delta.Item1 > 0 ? "DOWN" : "UP";
            var leftOrRight = delta.Item2 > 0 ? "RIGHT" : "LEFT";
            return Enumerable.Repeat(upOrDown, Math.Abs(delta.Item1))
                .Concat(Enumerable.Repeat(leftOrRight, Math.Abs(delta.Item2)))
                .ToArray();

        }

        public static Tuple<int, int> GetCoordinates(int n, String[] grid, char who)
        {
            var line = string.Join(string.Empty, grid);
            int index = line.IndexOf(who);

            return Tuple.Create(index / n, index % n);
        }
    }

    [TestFixture]
    public class TestFixture
    {
        private readonly string[] grid = new[] {"---", "-m-", "p--"};

        [Test]
        public void CanFindCoordinates()
        {
            Assert.That(Princess.GetCoordinates(3, grid, 'm'), Is.EqualTo(Tuple.Create(1,1)));
            Assert.That(Princess.GetCoordinates(3, grid, 'p'), Is.EqualTo(Tuple.Create(2,0)));
        }

        [Test]
        public void CanFindPrincess()
        {
            Assert.That(Princess.GetPathToPrincess(3, grid), Is.EquivalentTo(new[] {"DOWN", "LEFT"}));
        }

        [Test]
        public void CanFindPrincess2()
        {
            string[] grid = new[]
            {
                "p----",
                "-----",
                "--m--",
                "-----",
                "-----",
            };
            Assert.That(Princess.GetPathToPrincess(5, grid), Is.EquivalentTo(new[] { "UP", "UP", "LEFT", "LEFT" }));
        }

    }
}
