using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace EFInstanceMethodTest
{
    internal class Program
    {
        private class Executor
        {
            private string ImAnInstanceMethod(string? s) => s ?? "";

            private static string ImAStaticMethod(string? s) => s ?? "";

            public void DoInstanceThing()
            {
                using (var dx = new TestContext())
                {
                    var result = dx.SOME_TABLE.Select(t => ImAnInstanceMethod(t.Field)).FirstOrDefault();

                    Console.WriteLine(result);
                }
            }

            public void DoStaticThing()
            {
                using (var dx = new TestContext())
                {
                    var result = dx.SOME_TABLE.Select(t => ImAStaticMethod(t.Field)).FirstOrDefault();

                    Console.WriteLine(result);
                }
            }
        }


        static void Main(string[] args)
        {
            var exe = new Executor();

            Console.WriteLine("Calling a static method from final projection. Result:");
            RunAndDump(exe.DoStaticThing);

            Console.WriteLine("\r\nCalling an instance method from final projection. Result:");
            RunAndDump(exe.DoInstanceThing);
        }


        private static void RunAndDump(Action a)
        {
            try
            {
                a();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }



    internal class TestContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("<put your connection string here>", b => b.UseRelationalNulls(true));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("testing");

            modelBuilder.Entity<SOME_TABLE>().HasNoKey();
        }

        public virtual DbSet<SOME_TABLE> SOME_TABLE { get; set; }
    }

    internal class SOME_TABLE
    {
        [StringLength(25)]
        [Unicode(false)]
        public string? Field { get; set; }
    }

}
