using System.Data;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Http.HttpResults;
using ReviewAnythingAPI.Enums;

namespace ReviewAnythingAPI.DTOs.ReviewDTOs;
public class CommentVoteResponseDto
{
    public int CommentId { get; set; }
    public int UserVote { get; set; }
    public ActionType ActionType { get; set; }
    //public int TotalVotes { get; set; }
}