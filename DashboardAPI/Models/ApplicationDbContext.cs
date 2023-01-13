using Microsoft.EntityFrameworkCore;

namespace DashboardAPI.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                    new Category
                    {
                        CategoryId = 1,
                        Name = "Salary",
                        Icon = "income-transaction-e-wallet.png",
                        Type = "Income",
                        Bookmark = 1,
                        Limit= 0,
                    },
                    new Category
                    {
                        CategoryId = 2,
                        Name = "Shopping",
                        Icon = "shopping231603436.png",
                        Type = "Expense",
                        Bookmark = 0,
                        Limit = 40000,
                    },
                    new Category
                    {
                        CategoryId = 3,
                        Name = "Electricity Bill",
                        Icon = "",
                        Type = "Expense",
                        Bookmark = 0,
                        Limit = 15000,
                    }
                );

            modelBuilder.Entity<Transaction>().HasData(
                    new Transaction
                    {
                        TransactionId = 1,
                        CategoryId = 1,
                        Title = "January Salary",
                        Amount = 75000,
                        Description = "This month's salary 🥳",
                        Date = DateTime.Today.AddDays(-4),
                    },
                    new Transaction
                    {
                        TransactionId = 2,
                        CategoryId = 2,
                        Title = "Puma Shoes",
                        Amount = 6000,
                        Description = "Just got a new pair of shoe 😎",
                        Date = DateTime.Today.AddDays(-3),
                    },
                    new Transaction
                    {
                        TransactionId = 3,
                        CategoryId = 2,
                        Title = "Park Avenue Perfume",
                        Amount = 800,
                        Description = "The perfume smeels good, doesn't it?",
                        Date = DateTime.Today.AddDays(-1),
                    },
                    new Transaction
                    {
                        TransactionId = 4,
                        CategoryId = 3,
                        Title = "Electricity Bill",
                        Amount = 3500,
                        Description = "Electricity Bill",
                        Date = DateTime.Today,
                    },
                    new Transaction
                    {
                        TransactionId = 5,
                        CategoryId = 2,
                        Title = "New Bike",
                        Amount = 30000,
                        Description = "Just got a bike😍",
                        Date = DateTime.Today,
                    },
                    new Transaction
                    {
                        TransactionId = 6,
                        CategoryId = 3,
                        Title = "Office Bill",
                        Amount = 2300,
                        Description = "Pay office electricity bill",
                        Date = DateTime.Today.AddDays(2),
                    },
                    new Transaction
                    {
                        TransactionId = 7,
                        CategoryId = 2,
                        Title = "BoAt Watch",
                        Amount = 700,
                        Description = "BoAt Watch",
                        Date = DateTime.Today.AddDays(4),
                    }
                );
        }

        public override int SaveChanges()
        {
            var now = DateTime.UtcNow;

            foreach (var changedEntity in ChangeTracker.Entries())
            {
                if (changedEntity.Entity is BaseEntity entity)
                {
                    switch (changedEntity.State)
                    {
                        case EntityState.Added:
                            entity.CreatedDate = now;
                            entity.UpdatedDate = now;
                            break;

                        case EntityState.Modified:
                            Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                            entity.UpdatedDate = now;
                            break;
                    }
                }
            }

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var AddedEntities = ChangeTracker.Entries().Where(E => E.State == EntityState.Added).ToList();

            AddedEntities.ForEach(E =>
            {
                E.Property("CreatedDate").CurrentValue = DateTime.Now;
            });

            var EditedEntities = ChangeTracker.Entries().Where(E => E.State == EntityState.Modified).ToList();

            EditedEntities.ForEach(E =>
            {
                E.Property("UpdatedDate").CurrentValue = DateTime.Now;
            });

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
