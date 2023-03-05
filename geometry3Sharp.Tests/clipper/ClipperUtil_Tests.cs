using g3;
using gs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Sutro.Clipper.Tests
{
    [TestClass]
    public class ClipperUtil_Tests
    {
        [TestMethod]
        public void Difference_CanCancel()
        {
            // Arrange
            int nGrid = 50;
            int nCircles = nGrid * nGrid;
            int nSegsPerCircle = 36;

            var seed = new GeneralPolygon2d(Polygon2d.MakeCircle(0.4, nSegsPerCircle));
            var inner = Polygon2d.MakeCircle(0.2, nSegsPerCircle);
            inner.Reverse();
            seed.AddHole(inner, false, false);

            var subject = new List<GeneralPolygon2d>();
            var minuend = new List<GeneralPolygon2d>();
            for (int i = 0; i < nGrid; i++)
            {
                for (int j = 0; j < nGrid; j++)
                {
                    var s = new GeneralPolygon2d(seed);
                    s.Translate(new Vector2d(i, j));
                    subject.Add(s);

                    var o = new GeneralPolygon2d(s);
                    o.Translate(new Vector2d(0.1, 0));
                    minuend.Add(o);
                }
            }

            var cancellationTokenSource = new CancellationTokenSource();

            Console.WriteLine($"Subtracting list of {nCircles} polygons...");

            // Act
            var watch = new Stopwatch();
            Func<List<GeneralPolygon2d>> action = () => ClipperUtil.Difference(subject, minuend, -1, cancellationTokenSource.Token);

            // Run without cancelling
            watch.Start();
            var result = action.Invoke();
            watch.Stop();
            long executionTime = watch.ElapsedMilliseconds;

            // Assert we got a real result
            Assert.AreNotEqual(0, result.Count);
            Console.WriteLine($"Operation completed in {executionTime / 1000d:0.00} seconds.");

            // Run with cancelling
            watch.Reset();
            watch.Start();
            var task = Task.Factory.StartNew(action);
            cancellationTokenSource.CancelAfter(100);
            task.Wait();
            var cancelledResult = task.Result;
            watch.Stop();
            long cancellationTime = watch.ElapsedMilliseconds;

            // Assert cancelled result is an empty list
            Console.WriteLine($"Operation cancelled after {cancellationTime / 1000d:0.00} seconds.");
            Assert.AreEqual(0, cancelledResult.Count);
            Assert.IsTrue(cancellationTime < executionTime / 2d, "Cancelled run should be less than 1/2 the ellapsed time of the completed run");
        }
    }
}