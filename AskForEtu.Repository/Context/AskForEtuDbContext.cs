
using AskForEtu.Core.Entity;
using AskForEtu.Core.Entity.Base;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AskForEtu.Repository.Context
{
    public class AskForEtuDbContext : DbContext
    {
        public AskForEtuDbContext(DbContextOptions<AskForEtuDbContext> options)
            : base(options) { }

        DbSet<User> Users { get; set; }
        DbSet<Token> Tokens { get; set; }
        DbSet<PasswordReset> PasswordResets { get; set; }
        DbSet<Question> Questions { get; set; }
        DbSet<Comment> Comments { get; set; }
        DbSet<DisLike> DisLikes { get; set; }
        DbSet<Like> Likes { get; set; }
        DbSet<Major> Majors { get; set; }
        DbSet<Faculty> Faculties { get; set; }
        DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName).IsUnique();

            modelBuilder.Entity<User>()
                .HasQueryFilter(u => !u.IsDeleted && u.IsActive);

            modelBuilder.Entity<Question>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<Comment>()
                .HasQueryFilter(u => !u.IsDeleted);

            var faculties = new List<Faculty>
            {
                new()
                {
                    Id = 1,
                    Name = "Muhendislik Fakültesi",
                    Majors = new List<Major>
                    {
                        new Major
                        {
                            Id = 1,
                            Name = "Bilgisayar Mühendisliği"
                        },
                        new Major
                        {
                            Id = 2,
                            Name = "Makine Mühendisliği"
                        },
                        new Major
                        {
                            Id = 3,
                            Name = "Elektrik Elektronik Mühendisliği"
                        },
                        new Major
                        {
                            Id = 4,
                            Name = "Endüstri Mühendisliği"
                        }
                    }
                },
                new()
                {
                    Id = 2,
                    Name = "Edebiyat Fakültesi",
                    Majors = new List<Major>
                    {
                        new Major
                        {
                            Id = 8,
                            Name = "Türk Dili ve Edebiyatı"
                        },
                        new Major
                        {
                            Id = 9,
                            Name = "İngiliz Dili ve Edebiyatı"
                        },
                        new Major
                        {
                            Id = 10,
                            Name = "Psikoloji"
                        }
                    }
                }
            };

            modelBuilder.Entity<Faculty>().HasData(faculties);

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }
        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is EntityBase && (x.State == EntityState.Added
                                                       | x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var now = DateTime.Now;

                if (entity.State == EntityState.Added)
                {
                    ((EntityBase)entity.Entity).CreatedDate = now;
                    ((EntityBase)entity.Entity).IsDeleted = false;
                }
                if (entity.State == EntityState.Modified)
                    ((EntityBase)entity.Entity).UpdatedDate = now;
            }
        }
    }
}
