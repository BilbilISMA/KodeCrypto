using KodeCrypto.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AccountBalanceConfiguration : IEntityTypeConfiguration<AccountBalance>
{
    public void Configure(EntityTypeBuilder<AccountBalance> builder)
    {
        builder.ToTable(nameof(AccountBalance));

        builder.HasKey(accountBalance => accountBalance.Id);

        builder.Property(accountBalance => accountBalance.UserId)
            .IsRequired();

        builder.Property(accountBalance => accountBalance.ProviderId)
            .IsRequired();

        builder.Property(accountBalance => accountBalance.SyncDate)
            .IsRequired();

        builder.Property(accountBalance => accountBalance.Data)
            .IsRequired();

        builder.HasOne(accountBalance => accountBalance.User)
            .WithMany(user => user.AccountBalances)
            .HasForeignKey(accountBalance => accountBalance.UserId)
            .OnDelete(DeleteBehavior.Cascade); 
    }
}
