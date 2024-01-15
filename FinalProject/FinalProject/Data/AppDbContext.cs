using FinalProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FinalProject.Data
{
	public class AppDbContext : DbContext
	{
        public AppDbContext(DbContextOptions options) : base(options)
        {
       
        }

		public DbSet<User> Users { get; set; }

		public DbSet<Category> Categories { get; set; }
		public DbSet<CategoryContainer> categoryContainers  { get; set; }

		public List<CategoryContainer> GetCategories()
		{
			return categoryContainers.ToList();
		}

		public List<User> GetUser()
		{
			var adminRole = Role.Admin; // Replace UserRole with the actual enum name

			return Users.Where(u => u.Role == adminRole).ToList();
		}
		public DbSet<Product> Products { get; set; }

		public DbSet<FeedbackForProduct> FeedbackForProducts { get; set; }

		public DbSet<FeedbackForWeb> FeedbackForWebs { get; set; }

		public DbSet<Payment> Payments { get; set; }
		public DbSet<Cart> Cart { get; set; }
		public DbSet<Order> Orders { get; set; }
        public DbSet<SecretCodeForProduct> SecretCodeForProducts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>(entity =>
			{
				entity.HasMany(u => u.FeedbackForProducts)
					.WithOne(f => f.User)
					.HasForeignKey(f => f.userID)
					.IsRequired();

				entity.HasMany(u => u.FeedbackForWebs)
					.WithOne(f => f.User)
					.HasForeignKey(f => f.userID)
					.IsRequired();
			});

			modelBuilder.Entity<Product>(entity =>
			{

				entity.HasMany(u => u.FeedbackForProducts)
					.WithOne(f => f.Product)
					.HasForeignKey(f => f.productID)
					.IsRequired();
			});

			modelBuilder.Entity<Category>(entity =>
			{
				entity.HasMany(u => u.Products)
					.WithOne(f => f.Category)
					.HasForeignKey(f => f.categoryID)
					.IsRequired();
			});

			modelBuilder.Entity<User>()
				.HasMany(u => u.carts)
				.WithOne(f => f.User)
				.HasForeignKey(f => f.UserId)
				.OnDelete(DeleteBehavior.Restrict);


			modelBuilder.Entity<Product>()
				.HasMany(c => c.carts)
				.WithOne(p => p.product)
				.HasForeignKey(p => p.productId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<User>()
				.HasMany(c => c.orders)
				.WithOne(p => p.user)
				.HasForeignKey(p => p.userId);

			modelBuilder.Entity<CategoryContainer>()
				.HasMany(c => c.Categories)
				.WithOne(c => c.CategoryContainer)
				.HasForeignKey(c => c.CategoryContainerID);


			modelBuilder.Entity<Order>()
				.HasMany(c => c.Carts)
				.WithOne(c => c.Order)
				.HasForeignKey(c => c.OrderId);


			modelBuilder.Entity<Product>()
                .HasMany(c => c.secretCodeForProducts)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId);

        }
    }
}
