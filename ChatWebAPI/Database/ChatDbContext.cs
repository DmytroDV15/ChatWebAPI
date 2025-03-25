using ChatWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatWebAPI.Database
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {

        }

        public DbSet<RegistrationModel> RegisterModels { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMessageModel> ChatDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>()
                .HasMany(c => c.Users)
                .WithMany(u => u.Chats)
                .UsingEntity<Dictionary<string, object>>(
                    "ChatUser",
                    j => j.HasOne<RegistrationModel>().WithMany().HasForeignKey("UserId"),
                    j => j.HasOne<Chat>().WithMany().HasForeignKey("ChatId")
                );

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Creator)
                .WithMany(rm => rm.CreatedChats)
                .HasForeignKey(c => c.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatMessageModel>()
                .HasOne(cd => cd.Chat)
                .WithMany(c => c.ChatDetails)
                .HasForeignKey(cd => cd.ChatId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
