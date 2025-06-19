using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.DTOs.CommentDTOs;
using ReviewAnythingAPI.DTOs.ReviewDTOs;
using ReviewAnythingAPI.Enums;
using ReviewAnythingAPI.HelperClasses.CustomExceptions;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<CommentService> _logger;
    private readonly IRepository<Review> _reviewRepository;
    private readonly ReviewAnythingDbContext _dbContext;
    private readonly ICommentVoteRepository _commentVoteRepository;

    public CommentService(ICommentRepository commentRepository, ILogger<CommentService> logger,
        IRepository<Review> reviewRepository, ReviewAnythingDbContext dbContext, ICommentVoteRepository commentVoteRepository)
    {
        _commentRepository = commentRepository;
        _logger = logger;
        _reviewRepository = reviewRepository;
        _dbContext = dbContext;
        _commentVoteRepository = commentVoteRepository;
    }

    public async Task<IEnumerable<CommentResponseDto>> GetAllCommentsByUserAsync(int userId)
    {
        if (userId < 0)
        {
            throw new ArgumentException("User id cannot be negative");
        }
        var comments = await _commentRepository.GetAllCommentsByUserIdAsync(userId);
        return comments;
    }

    public async Task<IEnumerable<CommentResponseDto>> GetAllCommentsByReviewAsync(int reviewId)
    {
        if (reviewId < 0)
        {
            throw new ArgumentException($"ReviewId cannot be negative");
        }
        var comments = await _commentRepository.GetAllCommentsByReviewIdAsync(reviewId);
        return comments;
    }

    public async Task<CommentResponseDto> CreateCommentAsync(CommentCreateRequestDto commentCreateRequestDto, int userId, string userName)
    {
        var reviewExists = await _reviewRepository.GetByIdAsync(commentCreateRequestDto.ReviewId);
        if (reviewExists == null)
        {
            throw new EntityNotFoundException($"review not found for the given reviewId {commentCreateRequestDto.ReviewId}");
        }

        Comment comment = new Comment
        {
            Content = commentCreateRequestDto.Content,
            ReviewId = commentCreateRequestDto.ReviewId,
            UserId = userId,
            CreationDate = DateTime.UtcNow,
            LastEditDate = DateTime.UtcNow
        };
        var commentInserted = await _commentRepository.CreateCommentAsync(comment);
        return commentInserted;
    }

    public async Task<CommentResponseDto> UpdateCommentAsync(CommentCreateRequestDto commentCreateRequestDto, int commentId, int userId, string userName)
    {
        Comment comment = await _commentRepository.GetByIdAsync(commentId);
        if (comment == null)
        {
            throw new EntityNotFoundException($"comment not found for the given commentId {commentId}");
        }

        if (comment.UserId != userId)
        {
            throw new UnauthorizedAccessException($"userId: {userId} does not own the given commentId {commentId}");
        }
        comment.Content = commentCreateRequestDto.Content;
        comment.LastEditDate = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return new CommentResponseDto
        {
            CommentId = comment.CommentId,
            Content = comment.Content,
            ReviewId = comment.ReviewId,
            LastEditDate = comment.LastEditDate,
        };
    }

    public async Task<CommentResponseDto> GetCommentByIdAsync(int commentId)
    {
        Comment comment = await _commentRepository.GetByIdAsync(commentId);
        if (comment == null)
        {
            throw new EntityNotFoundException($"comment not found for commentId {commentId}");
        }
        ;

        var userName = "";
        if (comment.UserId != null)
        {
            userName = await _dbContext.Users.Where(user => user.Id == comment.UserId).Select(u => u.UserName).FirstOrDefaultAsync();
        }

        return new CommentResponseDto
        {
            CommentId = comment.CommentId,
            Content = comment.Content,
            LastEditDate = comment.LastEditDate,
            ReviewId = comment.ReviewId,
        };

    }

    public async Task DeleteCommentByIdAsync(int commentId, int userId)
    {
        if (commentId <= 0)
        {
            throw new ArgumentException("commentId must be greater than 0", nameof(commentId));
        }
        var comment = await _commentRepository.GetByIdAsync(commentId);
        if (comment == null)
        {
            throw new KeyNotFoundException($"comment not found for the given commentId {commentId}");
        }
        ;

        if (comment.UserId != userId)
        {
            throw new UnauthorizedAccessException($"userId: {userId} is not authorized to delete comment commentId {commentId}");
        }
        await _commentRepository.DeleteAsyncById(commentId);
        await _dbContext.SaveChangesAsync();

    }

    public async Task<CommentVoteResponseDto> CommentVoteAsync(CommentVoteRequestDto commentVoteRequestDto, int userId)
    {
        var existingCommentVote = await _commentVoteRepository.GetByUserAndCommentIdAsync(userId, commentVoteRequestDto.CommentId);
        var response = new CommentVoteResponseDto
        {
            CommentId = commentVoteRequestDto.CommentId,
            UserVote = commentVoteRequestDto.VoteType,
        };
        if (existingCommentVote == null)
        {
            CommentVote newCommentVote = new CommentVote
            {
                UserId = userId,
                CommentId = commentVoteRequestDto.CommentId,
                VoteType = commentVoteRequestDto.VoteType,
                VoteDate = DateTime.UtcNow
            };
            await _commentVoteRepository.AddAsync(newCommentVote);
            response.ActionType = ActionType.Created;
        }
        else if (existingCommentVote.VoteType == commentVoteRequestDto.VoteType)
        {
            await _commentVoteRepository.DeleteAsyncByEntity(existingCommentVote);
            response.ActionType = ActionType.Deleted;
        }
        else
        {
            existingCommentVote.VoteType = commentVoteRequestDto.VoteType;
            existingCommentVote.VoteDate = DateTime.UtcNow;
            response.ActionType = ActionType.Updated;
        }
        await _dbContext.SaveChangesAsync();
        return response;
    }
}