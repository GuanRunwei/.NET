using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
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
    public class UserController : Controller
    {
        #region 数据库连接
        private DbEntity db = new DbEntity();
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
        #endregion
        #region 登录
        [HttpPost]
        public HttpResponseMessage Login(string Phonenumber, string Password)
        {
            User loginUser = db.Users.Where(s => s.Phonenumber == Phonenumber).FirstOrDefault();
            if (loginUser == null)
                return ApiResponse.Invalid("Phonenumber", "帐号不存在");
            if (Password != loginUser.Password)
                return ApiResponse.Invalid("Password", "密码错误");
            else
                return ApiResponse.Ok(new

                {
                    loginUser.Id,
                    loginUser.Username,
                    loginUser.Phonenumber,
                    loginUser.Password,
                    loginUser.CreateTime,
                    loginUser.ImageUrl
                });
        }
        #endregion
        #region 注册
        [HttpPost]
        public HttpResponseMessage Regist(string Phonenumber, string Password, string Username)
        {
            if (Phonenumber.Length != 11)
                return ApiResponse.Invalid("Phonenumber", "手机号不正确!");
            if (!Regex.IsMatch(Phonenumber, "^(13[0-9]|14[579]|15[0-3,5-9]|16[6]|17[0135678]|18[0-9]|19[89])\\d{8}$"))
                return ApiResponse.Invalid("Phonenumber", "手机号不正确!");
            if (Password.Count() < 6)
                return ApiResponse.Invalid("Password", "密码长度须大于等于6位！");
            if (db.Users.Where(s => s.Phonenumber == Phonenumber).Count() > 0)
                return ApiResponse.Invalid("Phonenumber", "手机号已存在！");
            if (db.Users.Where(s => s.Username == Username).Count() > 0)
                return ApiResponse.Invalid("Username", "用户名已存在！");
            User user = new User
            {
                Username = Username,
                Phonenumber = Phonenumber,
                Password = Password,
                CreateTime = DateTime.Now
            };
            db.Users.Add(user);
            db.SaveChanges();
            return ApiResponse.Ok(new
            {
                user.Username,
                user.Password,
                user.Phonenumber,
                user.CreateTime
            });
        }
        #endregion
        //#region 上传头像
        //public HttpResponseMessage UploadImage(int Id, HttpPostedFileBase Image)
        //{
        //    User user = db.Users.FirstOrDefault(s => s.Id == Id);
        //    try
        //    {
        //        string dictionary = null;
        //        string type = new FileInfo(Image.FileName).Extension.Substring(1).ToLower();
        //        string filename = Image.FileName;
        //        if (type == "jpg" || type == "gif" || type == "jpeg" || type == "png" || type == "bmp")//判断文件格式
        //        {
        //            string savePath = HttpContext.Current.Server.MapPath("~/Files/");//(系统兼容)指定上传文件在服务器上的保存路径
        //            if (!Directory.Exists(savePath))//检查服务器上是否存在这个物理路径
        //            {
        //                Directory.CreateDirectory(savePath);//如果不存在则创建 
        //            }
        //            dictionary = savePath + filename + DateTime.Now.ToString("yyyyMMddHHmmssfff");
        //            Image.SaveAs(HttpContext.Current.Server.MapPath(dictionary));
        //            user.ImageUrl = dictionary;
        //        }
        //        else
        //        {
        //            return ApiResponse.BadRequest("文件格式不正确");
        //        }

        //        return ApiResponse.Ok(new { url = dictionary });
        //    }
        //    catch (Exception ex)
        //    {
        //        return ApiResponse.BadRequest(ex.Message);
        //    }
        //}
        //#endregion

        #region 用户反馈
        [HttpPost]
        public HttpResponseMessage SendFeedback(int UserId, string Feedback)
        {
            User user = db.Users.FirstOrDefault(s => s.Id == UserId);
            try
            {
                user.Feedback = Feedback;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                db.Entry(user).State = EntityState.Unchanged;
                return ApiResponse.BadRequest(Message.EditFailure);
            }
            return ApiResponse.Ok("反馈成功！");
        }
        #endregion
        #region 业务指南
        public IActionResult GetGuideList()
        {
            List<Guide> guides = db.Guides.ToList();
            return Json(new
            {
                datas = guides.Select(s => new
                {
                    s.Id,
                    GuideTime = s.GuideTime.ToShortDateString(),
                    s.Title,
                    s.Content,
                    //PicUrl1 = HttpContext.Current.Server.MapPath(s.PicUrl1),
                    //PicUrl2 = HttpContext.Current.Server.MapPath(s.PicUrl2),
                    //PicUrl3 = HttpContext.Current.Server.MapPath(s.PicUrl3),
                })
            });
        }
        #endregion


        //[Route("Users")]
        public IActionResult GetUsers()
        {
            List<User> users = db.Users.ToList();
            return Json(new
            {
                datas = users.Select(s => new
                {
                    s.Id,
                    s.Username,
                    s.Phonenumber,
                    s.Password,
                    s.CreateTime,
                    s.ImageUrl
                })
            });
        }

    }
}