using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace Demo.Tests
{
    public class TestBaseControlList
    {
        private const int ExpectedResponseTime = 5; // milliseconds
        private const int ResourceCount = 10000;
        private const int PrincipalCount = 100;
        private const int OperationCount = 10;

        private static BaseControlList CreateMassiveAcl()
        {
            // Create an ACL with 1000 resources + 10 operations per resource + 100 principals per operation.

            var list = new BaseControlList();

            for (var i = 0; i < ResourceCount; i++)
            {
                for (var j = 0; j < OperationCount; j++)
                {
                    for (var k = 0; k < PrincipalCount; k++)
                    {
                        var principal = string.Format("Principal {0:D4}", k);
                        var operation = string.Format("Operation {0:D4}", j);
                        var resource = string.Format("Resource {0:D4}", i);
                        list.Include(principal, operation, resource);
                    }
                }
            }

            return list;
        }

        [Test]
        public static void Contains_InvalidResource_ReturnsFalse()
        {
            var list = new BaseControlList();
            list.Include("Administrators", "Edit", "Resource 0001");

            Assert.IsFalse(list.Contains("Resource 0000"));
        }

        [Test]
        public static void Contains_ValidResource_ReturnsTrue()
        {
            var list = new BaseControlList();
            list.Include("Administrators", "Edit", "Resource 0001");

            Assert.IsTrue(list.Contains("Resource 0001"));
        }

        [Test]
        public static void Grant_NullPrincipalOrOperationOrResource_ThrowsException()
        {
            var list = new BaseControlList();
            Assert.Throws<ArgumentNullException>(delegate { list.Include("Administrators", null, null); });
            Assert.Throws<ArgumentNullException>(delegate { list.Include(null, "Edit", null); });
            Assert.Throws<ArgumentNullException>(delegate { list.Include(null, null, "Resource 0001"); });
        }

        [Test]
        public static void Grant_OneResourceWithTwoOperations_Success()
        {
            var list = new BaseControlList();
            list.Include("Administrators", "Read", "Resource 0001");
            list.Include("Administrators", "Write", "Resource 0001");
            Assert.IsTrue(list.IsIncluded(new[] {"Administrators"}, "Read", "Resource 0001"));
            Assert.IsTrue(list.IsIncluded(new[] {"Administrators"}, "Write", "Resource 0001"));
        }

        [Test]
        public static void IsGranted_AcceptablePerformance_Success()
        {
            var acl = CreateMassiveAcl();
            var results = new StringBuilder();
            results.AppendLine("TestNumber,Principal,Operation,Resource,Granted,Milliseconds");

            var random = new Random();

            for (var i = 0; i < 5000; i++)
            {
                var randomPrincipal = CreateRandomName(random, "Principal", 1, (int)(1.5 * PrincipalCount));
                var randomOperation = CreateRandomName(random, "Operation", 1, (int)(1.5 * OperationCount));
                var randomResource = CreateRandomName(random, "Resource", 1, (int)(1.5 * ResourceCount));

                var watch = Stopwatch.StartNew();
                var isGranted = acl.IsIncluded(new[] {randomPrincipal}, randomOperation, randomResource);
                watch.Stop();

                Assert.LessOrEqual(watch.Elapsed.TotalMilliseconds, ExpectedResponseTime);

                results.AppendFormat("{0},{1},{2},{3},{4},{5}"
                    , i + 1
                    , randomPrincipal
                    , randomOperation
                    , randomResource
                    , isGranted ? "Y" : "N"
                    , watch.Elapsed.TotalMilliseconds);

                results.AppendLine();
            }

            const string physicalPath = @"D:\Apps\Daniel\Source\Demo\App_Data\Performance-Results.csv";
            if (Directory.Exists(Path.GetDirectoryName(physicalPath)))
                File.WriteAllText(physicalPath, results.ToString());
        }

        private static string CreateRandomName(Random random, string prefix, int min, int max)
        {
            return string.Format(prefix + " {0:D4}", random.Next(min, max));
        }

        [Test]
        public static void IsGranted_CaseInsensitivity_IsTrue()
        {
            var list = new BaseControlList();
            list.Include("Administrators", "Edit", "Resource 0001");

            Assert.IsTrue(list.IsIncluded(new[] {"administrators"}, "edit", "resource 0001"));
        }

        [Test]
        public static void IsGranted_InvalidPrincipalAndOperationAndResource_ReturnsFalse()
        {
            var list = new BaseControlList();
            list.Include("Administrators", "Edit", "Resource 0001");

            Assert.IsFalse(list.IsIncluded(new[] {"Programmers"}, "Delete", "Resource 0000"));
        }

        [Test]
        public static void IsGranted_InvalidPrincipalOrOperationOrResource_ReturnsFalse()
        {
            var list = new BaseControlList();
            list.Include("Administrators", "Edit", "Resource 0001");

            Assert.IsFalse(list.IsIncluded(new[] {"Administrators"}, "Edit", "Resource 0000"));
            Assert.IsFalse(list.IsIncluded(new[] {"Administrators"}, "Delete", "Resource 0001"));
            Assert.IsFalse(list.IsIncluded(new[] {"Programmers"}, "Edit", "Resource 0001"));
        }

        [Test]
        public static void IsGranted_NullPrincipalOrOperationOrResource_ReturnsFalse()
        {
            var list = new BaseControlList();
            list.Include("Administrators", "Edit", "Resource 0001");

            Assert.IsFalse(list.IsIncluded(null, "Delete", "Resource 0001"));
            Assert.IsFalse(list.IsIncluded(new[] {"Programmers"}, "Edit", null));
            Assert.IsFalse(list.IsIncluded(new[] {"Programmers", null}, null, "Resource 0001"));
            Assert.IsTrue(list.IsIncluded(new[] {null, "Administrators"}, "Edit", "Resource 0001"));
        }

        [Test]
        public static void IsGranted_LeftAndRightPadding_IsIgnored()
        {
            var list = new BaseControlList();
            list.Include("Administrators  ", "Edit   ", "Resource 0001    ");

            Assert.IsTrue(list.IsIncluded(new[] {" Administrators "}, " Edit ", " Resource 0001 "));
        }

        [Test]
        public static void IsGranted_ValidPrincipalAndOperationAndResource_ReturnsTrue()
        {
            var list = new BaseControlList();
            list.Include("Administrators", "Edit", "Resource 0001");

            Assert.IsTrue(list.IsIncluded(new[] {"Administrators"}, "Edit", "Resource 0001"));
        }
    }
}