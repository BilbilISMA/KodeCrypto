using KodeCrypto.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KodeCrypto.Infrastructure.Persistence.EntityConfigurations
{
	public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
    {
        public void Configure(EntityTypeBuilder<ApiKey> builder)
        {
            builder.ToTable(nameof(ApiKey));
            builder.HasKey(apiKey => apiKey.Id); 

            builder.Property(apiKey => apiKey.UserId)
                .IsRequired();

            builder.Property(apiKey => apiKey.ProviderId)
                .IsRequired();

            builder.Property(apiKey => apiKey.Key)
                .IsRequired();

            builder.Property(apiKey => apiKey.Secret)
                .IsRequired();

            builder.HasOne(apiKey => apiKey.User)
                .WithMany(user => user.ApiKeys)
                .HasForeignKey(apiKey => apiKey.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
        }
    }

}

