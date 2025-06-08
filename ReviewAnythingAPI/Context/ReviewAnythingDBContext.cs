using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ReviewAnythingAPI.Context;

public class ReviewAnythingDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
{
 public ReviewAnythingDbContext(DbContextOptions<ReviewAnythingDbContext> options) : base(options)
 { }
 
 public DbSet<Category> Categories {get;set;}
 public DbSet<Item> Items {get;set;}
 public DbSet<Review> Reviews {get;set;}
 public DbSet<Tag> Tags {get;set;}
 public DbSet<Comment> Comments {get;set;}
 public DbSet<Follow> Follows {get;set;}
 public DbSet<ReviewVote> ReviewVotes {get;set;}
 public DbSet<CommentVote> CommentVotes {get;set;}
 public DbSet<ReportReason> ReportReasons {get;set;}
 public DbSet<StatusReport> StatusReports {get;set;}
 public DbSet<Report> Reports {get;set;}
 public DbSet<ReviewTag> ReviewTags {get;set;}

 protected override void OnModelCreating(ModelBuilder modelBuilder)
 {
  base.OnModelCreating(modelBuilder);
  
  // Category Constrains
  modelBuilder.Entity<Category>().HasIndex(c => c.CategoryName).IsUnique();
  
  // Items Constraints
  modelBuilder.Entity<Item>().HasOne(item => item.Creator).WithMany(user => user.UserItems).HasForeignKey(item => item.CreatedByUserId).OnDelete(DeleteBehavior.SetNull);
  
  modelBuilder.Entity<Item>().HasOne(item => item.ItemCategory).WithMany().HasForeignKey(item => item.CategoryId).OnDelete(DeleteBehavior.Restrict);

  modelBuilder.Entity<Item>().HasIndex(i => i.CategoryId);

  modelBuilder.Entity<Item>().HasIndex(i => i.ItemName).IsUnique();
  
  // Reviews Constraints
  modelBuilder.Entity<Review>().HasOne(review => review.Creator).WithMany(creator => creator.UserReviews).HasForeignKey(review => review.UserId).OnDelete(DeleteBehavior.SetNull);
  
  modelBuilder.Entity<Review>().HasOne(review => review.ReviewItem).WithMany(i => i.ItemReviews).HasForeignKey(review => review.ItemId).OnDelete(DeleteBehavior.Restrict);

  modelBuilder.Entity<Review>().ToTable(t => t.HasCheckConstraint("CHK_Rating", "[Rating] BETWEEN 1 AND 5"));
  
  modelBuilder.Entity<Review>().HasIndex(r => r.UserId);
  modelBuilder.Entity<Review>().HasIndex(r => r.ItemId);
  modelBuilder.Entity<Review>().HasIndex(r => r.Rating);
  
  // Tags Constraints
  modelBuilder.Entity<Tag>().HasIndex(t => t.TagName).IsUnique();
  
  // ReviewTags Constraints
  modelBuilder.Entity<ReviewTag>().HasKey(rt => new { rt.ReviewId, rt.TagId });
  
  modelBuilder.Entity<ReviewTag>().HasOne(rt => rt.TagReview).WithMany(r => r.ReviewTags).HasForeignKey(rt => rt.ReviewId).OnDelete(DeleteBehavior.Cascade);
  
  modelBuilder.Entity<ReviewTag>().HasOne(rt => rt.Tag).WithMany(t => t.ReviewTags).HasForeignKey(rt => rt.TagId).OnDelete(DeleteBehavior.Restrict);
  
  // Comments Constraints
  modelBuilder.Entity<Comment>().HasOne(c => c.Review).WithMany(r => r.ReviewComments).HasForeignKey(c => c.ReviewId).OnDelete(DeleteBehavior.Cascade);
  
  modelBuilder.Entity<Comment>().HasOne(c => c.User).WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.SetNull);

  modelBuilder.Entity<Comment>().HasIndex(c => c.ReviewId);
  modelBuilder.Entity<Comment>().HasIndex(c => c.UserId);
  
  // Follows Constraints
  modelBuilder.Entity<Follow>().HasKey(f => new { f.FollowerUserId, f.FollowingUserId });
  
  modelBuilder.Entity<Follow>().HasOne(f => f.Follower).WithMany(u => u.UserFollows).HasForeignKey(f => f.FollowerUserId).OnDelete(DeleteBehavior.Cascade);
  
  modelBuilder.Entity<Follow>().HasOne(f => f.Following).WithMany(u => u.UserFollowings).HasForeignKey(f => f.FollowingUserId).OnDelete(DeleteBehavior.Restrict);
  
  // ReviewVotes Constraints
  modelBuilder.Entity<ReviewVote>().HasKey(rv => new { rv.UserId, rv.ReviewId });
  
  modelBuilder.Entity<ReviewVote>().HasOne(rv => rv.User).WithMany().HasForeignKey(rv => rv.UserId).OnDelete(DeleteBehavior.Cascade);
  
  modelBuilder.Entity<ReviewVote>().HasOne(rv => rv.Review).WithMany(r => r.ReviewVotes).HasForeignKey(rv => rv.ReviewId).OnDelete(DeleteBehavior.Cascade);

  modelBuilder.Entity<ReviewVote>().ToTable(t => t.HasCheckConstraint("CHK_VoteType", "[VoteType] IN (-1, 1)"));
  
  modelBuilder.Entity<ReviewVote>().HasIndex(rv => rv.ReviewId);
  
  // CommentVotes Constraints
  modelBuilder.Entity<CommentVote>().HasKey(cm => new { cm.UserId, cm.CommentId });
  
  modelBuilder.Entity<CommentVote>().HasOne(cm => cm.User).WithMany().HasForeignKey(cm => cm.UserId).OnDelete(DeleteBehavior.Cascade);
  
  modelBuilder.Entity<CommentVote>().HasOne(cm => cm.Comment).WithMany().HasForeignKey(cm => cm.CommentId).OnDelete(DeleteBehavior.Cascade);
  
  modelBuilder.Entity<CommentVote>().ToTable(t => t.HasCheckConstraint("CHK_CommentVoteType", "[VoteType] IN (-1, 1)"));
  
  // Report Constraints
  modelBuilder.Entity<Report>().HasOne(r => r.StatusReportName).WithMany().HasForeignKey(r => r.StatusReportId).OnDelete(DeleteBehavior.Restrict);
  
  modelBuilder.Entity<Report>().HasOne(r => r.ReportReasonName).WithMany().HasForeignKey(r => r.ReportReasonId).OnDelete(DeleteBehavior.Restrict);
  
  modelBuilder.Entity<Report>().HasOne(r => r.UserCreator).WithMany().HasForeignKey(r => r.ReporterUserId).OnDelete(DeleteBehavior.Cascade);
  
  modelBuilder.Entity<Report>().HasOne(r => r.ReviewerUser).WithMany().HasForeignKey(r => r.ReviewedByUserId).OnDelete(DeleteBehavior.Restrict);

  modelBuilder.Entity<Report>()
   .ToTable(t => t.HasCheckConstraint("CHK_ReportItemType", "[ReportedItemType] = 'Review' OR [ReportedItemType] = 'Comment'"));

  modelBuilder.Entity<Report>().HasIndex(r => r.StatusReportId);
 }
}