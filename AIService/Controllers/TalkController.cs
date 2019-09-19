using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AIService.Helper;
using AIService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TalkController : Controller
    {
        #region 数据库连接
        private DbEntity db = new DbEntity();
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
        #endregion
        #region 发说说
        [HttpPost]
        public HttpResponseMessage SendTalk(int UserId, string Content)
        {
            Talk talk = new Talk();
            try
            {
                talk.UserId = UserId;
                talk.Content = Content;
                talk.TalkTime = DateTime.Now;
                talk.User = db.Users.FirstOrDefault(s => s.Id == UserId);
                db.Talks.Add(talk);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return ApiResponse.BadRequest(ex.Message);
            }
            return ApiResponse.Ok(new
            {
                talk.Id,
                talk.UserId,
                talk.User.Username,
                talk.Content,
                talk.Praise,
                talk.Comments
            });
        }
        #endregion
        #region 评论
        [HttpPost]
        public HttpResponseMessage SendComment(int UserId, int TalkId, string Point)
        {
            Comment comment = new Comment();
            try
            {
                string tempName = db.Users.FirstOrDefault(s => s.Id == UserId).Username;
                comment.UserId = UserId;
                comment.TalkId = TalkId;
                comment.Point = Point;
                comment.CommentTime = DateTime.Now;
                comment.Commenter = tempName;
                db.Comments.Add(comment);
                db.SaveChanges();
                db.Talks.FirstOrDefault(s => s.Id == TalkId).Comments.Add(comment);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return ApiResponse.BadRequest(ex.Message);
            }
            return ApiResponse.Ok(new
            {
                comment.Id,
                comment.TalkId,
                comment.UserId,
                comment.Point,
                comment.CommentTime,
                comment.Commenter
            });
        }
        #endregion
        #region 点赞
        [HttpPost]
        public HttpResponseMessage SendPraise(int TalkId, int UserId)
        {
            Talk talk = db.Talks.FirstOrDefault(s => s.Id == TalkId);
            PraiseRecord praiseRecord = db.PraiseRecords.FirstOrDefault(s => s.UserId == UserId);
            if (praiseRecord == null)
            {
                try
                {
                    talk.Praise++;
                    PraiseRecord record = new PraiseRecord
                    {
                        UserId = UserId,
                    };
                    record.Count++;
                    db.PraiseRecords.Add(record);
                    db.Entry(talk).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    db.Entry(talk).State = EntityState.Unchanged;
                    return ApiResponse.BadRequest(Message.EditFailure);
                }
                return ApiResponse.Ok(Message.EditSuccess);
            }
            return ApiResponse.Invalid("TalkId", "您已点过赞!");
        }
        #endregion
        #region 取消点赞
        [HttpPost]
        public HttpResponseMessage CancelPraise(int TalkId)
        {
            Talk talk = db.Talks.FirstOrDefault(s => s.Id == TalkId);
            try
            {
                talk.Praise--;
                db.Entry(talk).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                db.Entry(talk).State = EntityState.Unchanged;
                return ApiResponse.BadRequest(Message.EditFailure);
            }
            return ApiResponse.Ok("取消赞成功!");
        }
        #endregion
        #region 删除说说
        [HttpPost]
        public HttpResponseMessage DeleteTalk(int TalkId, int UserId)
        {
            Talk talk = db.Talks.FirstOrDefault(s => s.Id == TalkId && s.UserId == UserId);
            List<Comment> comments = db.Comments.Where(s => s.TalkId == TalkId).ToList();
            try
            {
                db.Talks.Remove(talk);
                for (int i = 0; i < comments.Count(); i++)
                {
                    db.Comments.Remove(comments[i]);
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return ApiResponse.BadRequest(ex.Message);
            }
            return ApiResponse.Ok("删除成功！");
        }
        #endregion
        #region 删除评论
        [HttpPost]
        public HttpResponseMessage DeleteComment(int CommentId, int UserId)
        {
            Comment comment = db.Comments.FirstOrDefault(s => s.Id == CommentId && s.UserId == UserId);
            try
            {
                db.Comments.Remove(comment);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return ApiResponse.BadRequest(ex.Message);
            }
            return ApiResponse.Ok("删除成功！");
        }
        #endregion
        #region 获取论坛消息
        [HttpGet]
        public IActionResult GetTalkList()
        {
            List<Talk> talks = db.Talks.OrderByDescending(s => s.TalkTime).ToList();
            return Json(new
            {
                datas = talks.Select(s => new
                {
                    s.Id,
                    s.User.Username,
                    s.UserId,
                    s.Content,
                    s.Praise,
                    TalkTime = s.TalkTime.ToLongDateString() + " " + s.TalkTime.ToShortTimeString(),
                    CommentNumber = s.Comments.Count(),
                    commentData = s.Comments.Where(t => t.TalkId == s.Id).Select(t => new
                    {
                        t.Id,
                        CommentUserId = t.UserId,
                        t.Commenter,
                        t.Point,
                        CommentTime = t.CommentTime.ToLongDateString() + " " + t.CommentTime.ToShortTimeString()
                    })
                })
            });
        }
        #endregion

    }
}