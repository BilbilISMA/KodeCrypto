using KodeCrypto.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KodeCrypto.Infrastructure.Persistence.EntityConfigurations
{
    public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable(nameof(Order));

            builder.HasKey(order => order.Id);

            builder.Property(order => order.UserId)
                .IsRequired();

            builder.Property(order => order.Type)
                .IsRequired();

            builder.Property(order => order.OrderType)
                .IsRequired();

            builder.Property(order => order.Pair)
                .IsRequired();

            builder.Property(order => order.Price)
                .IsRequired();

            builder.Property(order => order.Volume)
                .IsRequired();

            builder.Property(order => order.Reference)
                .IsRequired();

            builder.HasOne(order => order.User)
                .WithMany(user => user.Orders)
                .HasForeignKey(order => order.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}