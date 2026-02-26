using CoworkerHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkerHub.Infrastructure.Configurations
{
    public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
    {
        public void Configure(EntityTypeBuilder<Workspace> builder)
        {
            builder.HasKey(w => w.Id);
            builder.Property(w => w.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(w => w.Name).IsUnique(); // Запрещаем одинаковые названия
        }
    }
}
