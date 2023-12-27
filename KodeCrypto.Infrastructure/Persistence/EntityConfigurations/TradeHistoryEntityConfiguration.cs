using KodeCrypto.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TradeHistoryConfiguration : IEntityTypeConfiguration<TradeHistory>
{
    public void Configure(EntityTypeBuilder<TradeHistory> builder)
    {
        builder.ToTable(nameof(TradeHistory));

        builder.HasKey(tradeHistory => tradeHistory.Id); 

        builder.Property(tradeHistory => tradeHistory.UserId)
            .IsRequired();

        builder.Property(tradeHistory => tradeHistory.TradeId)
            .IsRequired();

        builder.Property(tradeHistory => tradeHistory.Pair)
            .IsRequired();

        builder.Property(tradeHistory => tradeHistory.Time)
            .IsRequired();

        builder.Property(tradeHistory => tradeHistory.OrderType)
            .IsRequired();

        builder.Property(tradeHistory => tradeHistory.Price)
            .IsRequired();

        builder.Property(tradeHistory => tradeHistory.Cost)
            .IsRequired();

        builder.Property(tradeHistory => tradeHistory.Fee)
            .IsRequired();

        builder.Property(tradeHistory => tradeHistory.Margin)
            .IsRequired();

        builder.Property(tradeHistory => tradeHistory.Misc)
            .IsRequired();

        builder.HasOne(tradeHistory => tradeHistory.User)
            .WithMany(user => user.TradeHistories)
            .HasForeignKey(tradeHistory => tradeHistory.UserId)
            .OnDelete(DeleteBehavior.Cascade); 

    }
}
