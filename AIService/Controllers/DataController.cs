using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DataController : Controller
    {
        #region 数据库连接
        private DbEntity db = new DbEntity();
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
        #endregion
        #region 获取回答答案
        [HttpGet]
        public IActionResult GetAnswer(string Question, int UserId)
        {
            List<Knowledge> knowledges = db.Knowledges.ToList();
            List<SearchHistory> histories = db.SearchHistories.ToList();
            List<SearchHistory> repetitions = new List<SearchHistory>();
            SearchHistory searchHistory = null;
            for (int i = 0; i < histories.Count(); i++)
            {
                if (GetSimilarity(Question, histories[i].HistoricalText) > 0.85)
                    repetitions.Add(histories[i]);
            }
            if (db.SearchHistories.Where(s => s.HistoricalText == Question).Count() == 0 && db.SearchHistories.Where(s => s.HistoricalText.Contains(Question)).Count() == 0 && repetitions.Count() == 0)
            {
                searchHistory = new SearchHistory();
                searchHistory.SearchTime = DateTime.Now;
                searchHistory.HistoricalText = Question;
                searchHistory.UserId = UserId;
                searchHistory.User = db.Users.FirstOrDefault(s => s.Id == UserId);

            }
            if(repetitions.Count()==1)
            {                
                SearchHistory temp = repetitions.FirstOrDefault();
                temp.SearchTime = DateTime.Now;
                db.Entry(temp).State = EntityState.Modified;
                db.SaveChanges();

            }
            Dictionary<Knowledge, double> tempDatas = new Dictionary<Knowledge, double>();
            if (GetSimilarity("我转银行突然冻结了", Question) > 0.8 || GetSimilarity("我银行账户被冻结了", Question) > 0.8 || GetSimilarity("账户被冻结怎么办", Question) > 0.8 || GetSimilarity("账户被冻结", Question) > 0.8 || GetSimilarity("银转证转不了", Question) > 0.8)
                return Json(new
                {
                    datas = knowledges.Where(s => s.Id == 3).Select(s => new
                    {
                        s.Question,
                        s.Answer
                    })
                });
            for (int i = 0; i < knowledges.Count(); i++)
            {
                if (GetSimilarity(knowledges[i].Question, Question) > 0.4)
                {
                    tempDatas.Add(knowledges[i], GetSimilarity(knowledges[i].Question, Question));
                }
            }
            Dictionary<Knowledge, double> results = tempDatas.OrderByDescending(s => s.Value).ToDictionary(s => s.Key, s => s.Value);
            if (results.Count() > 0)
            {
                if (searchHistory != null)
                {
                    searchHistory.Answer = results.First().Key.Answer;
                    db.SearchHistories.Add(searchHistory);
                    db.SaveChanges();
                }
                if (results.First().Value > 0.85)
                {
                    return Json(new
                    {
                        datas = results.Take(1).Select(s => new
                        {
                            s.Key.Question,
                            s.Key.Answer
                        })
                    });
                }
                else if (results.Count() > 2)
                {
                    return Json(new
                    {
                        datas = results.Take(3).Select(s => new
                        {
                            s.Key.Question,
                            s.Key.Answer
                        })
                    });
                }
                else if (results.Count() <= 2 && results.Count() > 0)
                {
                    return Json(new
                    {
                        datas = results.Select(s => new
                        {
                            s.Key.Question,
                            s.Key.Answer
                        })
                    });
                }
            }
            return Json(new
            {
                datas = new ArrayList
                {
                    new { Question="旺财只知道金融证券哟，其他的去问小度好了！",Answer="" }
                }
            });
        }
        #endregion
        #region 获取名词解释
        [HttpGet]
        public IActionResult GetExplain(string Word, int UserId)
        {
            List<Dictionary> dictionaries = db.Dictionaries.ToList();
            Dictionary<Dictionary, double> results = new Dictionary<Dictionary, double>();
            List<SearchHistory> histories = db.SearchHistories.ToList();
            List<SearchHistory> repetitions = new List<SearchHistory>();
            List<WordsHistory> wordsHistories = db.WordsHistories.ToList();
            WordsHistory wordsHistory = null;
            List<WordsHistory> repeat = new List<WordsHistory>();
            SearchHistory searchHistory = null;
            for (int i = 0; i < histories.Count(); i++)
            {
                if (GetSimilarity(Word, histories[i].HistoricalText) > 0.85)
                    repetitions.Add(histories[i]);
            }
            for (int j = 0; j < wordsHistories.Count(); j++)
            {
                if (GetSimilarity(Word, wordsHistories[j].Word) > 0.85)
                    repeat.Add(wordsHistories[j]);
            }
            if (db.SearchHistories.Where(s => s.HistoricalText == Word).Count() == 0 && db.SearchHistories.Where(s => s.HistoricalText.Contains(Word)).Count() == 0 && repetitions.Count() == 0)
            {
                searchHistory = new SearchHistory();
                searchHistory.SearchTime = DateTime.Now;
                searchHistory.HistoricalText = Word;
                searchHistory.UserId = UserId;
                searchHistory.User = db.Users.FirstOrDefault(s => s.Id == UserId);
            }
            if (db.WordsHistories.Where(s => s.Word.Equals(Word)).Count() == 0 && db.WordsHistories.Where(s => s.Word.Contains(Word)).Count() == 0 && repeat.Count() == 0)
            {
                wordsHistory = new WordsHistory();
            }
            for (int i = 0; i < dictionaries.Count(); i++)
            {
                if (GetSimilarity(Word, dictionaries[i].Word) > 0.65)
                {
                    results.Add(dictionaries[i], GetSimilarity(Word, dictionaries[i].Word));
                }
            }
            Dictionary<Dictionary, double> list = results.OrderByDescending(s => s.Value).ToDictionary(s => s.Key, s => s.Value);
            if (list.Count() > 0)
            {
                if (searchHistory != null)
                {
                    searchHistory.Answer = list.First().Key.Explain;
                    db.SearchHistories.Add(searchHistory);
                    db.SaveChanges();
                }
                if (wordsHistory != null)
                {
                    wordsHistory.UserId = UserId;
                    wordsHistory.Explain = list.First().Key.Explain;
                    wordsHistory.Word = list.First().Key.Word;
                    wordsHistory.SearchTime = DateTime.Now;
                    db.WordsHistories.Add(wordsHistory);
                    db.SaveChanges();
                }
                if (list.First().Value == 1)
                {
                    return Json(new
                    {
                        datas = list.Take(1).Select(s => new
                        {
                            s.Key.Word,
                            s.Key.Explain
                        })
                    });
                }
                else if (list.Count() > 2)
                {
                    return Json(new
                    {
                        datas = list.Take(3).Select(s => new
                        {
                            s.Key.Word,
                            s.Key.Explain
                        })
                    });
                }
                else if (list.Count() <= 2 && list.Count() > 0)
                {
                    return Json(new
                    {
                        datas = list.Select(s => new
                        {
                            s.Key.Word,
                            s.Key.Explain
                        })
                    });
                }
            }
            return Json(new { });
        }
        #endregion
        #region App首页新闻List
        public IActionResult GetNews()
        {
            List<New> news = db.News.OrderByDescending(s => s.IssueTime).Take(3).ToList();
            return Json(new
            {
                datas = news.Select(s => new
                {
                    s.Id,
                    IssueTime = s.IssueTime.ToShortDateString().ToString(),
                    s.Title,
                    s.Content,
                    s.Source,
                    s.PicUrl1,
                    s.NewsSource
                })
            });
        }
        #endregion
        #region 所有新闻
        public IActionResult GetAllNews()
        {
            List<New> news = db.News.OrderByDescending(s => s.IssueTime).ToList();
            return Json(new
            {
                datas = news.Select(s => new
                {
                    s.Id,
                    IssueTime = s.IssueTime.ToShortDateString().ToString(),
                    s.Title,
                    s.Source,
                    s.PicUrl1,
                    s.NewsSource,
                    s.Content
                })
            });
        }
        #endregion
        #region 资讯详情
        public IActionResult GetDetailNews(int Id)
        {
            New @new = db.News.FirstOrDefault(s => s.Id == Id);
            return Json(new
            {
                IssueTime = @new.IssueTime.ToShortDateString().ToString(),
                @new.Title,
                @new.Content,
                @new.Source,
                @new.PicUrl1
            });
        }
        #endregion
        #region 资讯推荐
        [HttpGet]
        public IActionResult GetNeedNews(string Question)
        {
            List<New> news = db.News.ToList();
            List<New> results = new List<New>();
            for (int
i = 0; i < news.Count(); i++)
            {
                if (GetSimilarity(Question, news[i].Title) > 0.6 || news[i].Content.Contains(Question))
                    results.Add(news[i]);
            }
            if (results.Count() > 0)
            {
                if (results.Count() > 4 && results.Count() < 11)
                    return Json(new
                    {
                        datas = results.Select(s => new
                        {
                            s.Id,
                            IssueTime = s.IssueTime.ToShortDateString().ToString(),
                            s.Title,
                            s.Source,
                            s.Content,
                            s.PicUrl1,
                            s.NewsSource
                        })
                    });
                if (results.Count() > 10)
                    return Json(new
                    {
                        datas = results.Take(10).Select(s => new
                        {
                            s.Id,
                            IssueTime = s.IssueTime.ToShortDateString().ToString(),
                            s.Title,
                            s.Source,
                            s.Content,
                            s.PicUrl1,
                            s.NewsSource
                        })
                    });
                if (results.Count() < 5 && results.Count() > 0)
                    return Json(new
                    {
                        datas = results.Select(s => new
                        {
                            s.Id,
                            IssueTime = s.IssueTime.ToShortDateString().ToString(),
                            s.Title,
                            s.Source,
                            s.Content,
                            s.PicUrl1,
                            s.NewsSource
                        })
                    });
            }
            return Json(new { });
        }
        #endregion
        #region 余弦定理文本相似度匹配
        public static double GetSimilarity(String doc1, String doc2)
        {
            if (doc1 != null && doc1.Trim().Length > 0 && doc2 != null
                    && doc2.Trim().Length > 0)
            {
                Dictionary<int, int[]> AlgorithmMap = new Dictionary<int, int[]>();
                //将两个字符串中的中文字符以及出现的总数封装到，AlgorithmMap中
                for (int i = 0; i < doc1.Length; i++)
                {
                    char d1 = doc1.ToCharArray()[i];
                    if (IsHanZi(d1))
                    {
                        int charIndex = GetGB2312Id(d1);
                        if (charIndex != -1)
                        {
                            int[] fq = null;
                            try
                            {
                                fq = AlgorithmMap[charIndex];
                            }
                            catch (Exception)
                            {
                            }
                            finally
                            {
                                if (fq != null && fq.Length == 2)
                                {
                                    fq[0]++;
                                }
                                else
                                {
                                    fq = new int[2];
                                    fq[0] = 1;
                                    fq[1] = 0;
                                    AlgorithmMap.Add(charIndex, fq);
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < doc2.Length; i++)
                {
                    char d2 = doc2.ToCharArray()[i];
                    if (IsHanZi(d2))
                    {
                        int charIndex = GetGB2312Id(d2);
                        if (charIndex != -1)
                        {
                            int[] fq = null;
                            try
                            {
                                fq = AlgorithmMap[charIndex];
                            }
                            catch (Exception)
                            {
                            }
                            finally
                            {
                                if (fq != null && fq.Length == 2)
                                {
                                    fq[1]++;
                                }
                                else
                                {
                                    fq = new int[2];
                                    fq[0] = 0;
                                    fq[1] = 1;
                                    AlgorithmMap.Add(charIndex, fq);
                                }
                            }
                        }
                    }
                }
                double sqdoc1 = 0;
                double sqdoc2 = 0;
                double denominator = 0;
                foreach (KeyValuePair<int, int[]> par in AlgorithmMap)
                {
                    int[] c = par.Value;
                    denominator += c[0] * c[1];
                    sqdoc1 += c[0] * c[0];
                    sqdoc2 += c[1] * c[1];
                }
                return denominator / Math.Sqrt(sqdoc1 * sqdoc2);
            }
            else
            {
                throw new Exception();
            }
        }
        public static bool IsHanZi(char ch)
        {
            // 判断是否汉字
            return (ch >= 0x4E00 && ch <= 0x9FA5);
        }
        /**
         * 根据输入的Unicode字符，获取它的GB2312编码或者ascii编码，
         * 
         * @param ch
         *            输入的GB2312中文字符或者ASCII字符(128个)
         * @return ch在GB2312中的位置，-1表示该字符不认识
         */
        public static short GetGB2312Id(char ch)
        {
            try
            {
                byte[] buffer = System.Text.Encoding.GetEncoding("gb2312").GetBytes(ch.ToString());
                if (buffer.Length != 2)
                {
                    // 正常情况下buffer应该是两个字节，否则说明ch不属于GB2312编码，故返回'?'，此时说明不认识该字符
                    return -1;
                }
                int b0 = (int)(buffer[0] & 0x0FF) - 161; // 编码从A1开始，因此减去0xA1=161
                int b1 = (int)(buffer[1] & 0x0FF) - 161; // 第一个字符和最后一个字符没有汉字，因此每个区只收16*6-2=94个汉字
                return (short)(b0 * 94 + b1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return -1;
        }
        #endregion
        #region 跳出用户可能会需要查找的名词解释
        [HttpGet]
        public IActionResult GetPossibleWords(int UserId)
        {
            List<SearchHistory> searchHistories = db.SearchHistories.Where(s => s.UserId == UserId).OrderByDescending(s => s.SearchTime).ToList();
            List<SearchHistory> results = new List<SearchHistory>();
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            if (searchHistories.Count() > 3)
            {
                results.Add(searchHistories[0]);
                results.Add(searchHistories[1]);
                results.Add(searchHistories[2]);
                return Json(new
                {
                    datas = results.Select(s => new
                    {
                        s.HistoricalText,
                        s.Answer
                    })
                });

            }
            if (searchHistories.Count() < 4 && searchHistories.Count() > 0)
            {
                for (int i = 0; i < searchHistories.Count(); i++)
                {
                    results.Add(searchHistories[i]);
                }
                return Json(new
                {
                    datas = results.Select(s => new
                    {
                        s.HistoricalText,
                        s.Answer
                    })
                });
            }
            return Json(new { });
        }
        #endregion
        #region 名词解释历史
        [HttpGet]
        public IActionResult GetWordRecords(int UserId)
        {
            List<WordsHistory> wordsHistories = db.WordsHistories.OrderByDescending(s => s.SearchTime).ToList();
            List<Dictionary> dictionaries = db.Dictionaries.ToList();
            if (wordsHistories.Count() > 14)
                return Json(new
                {
                    datas = wordsHistories.Take(15).Select(s => new
                    {
                        s.Word,
                        s.Explain
                    })
                });
            return Json(new
            {
                datas = dictionaries.Take(15).Select(s => new
                {
                    s.Word,
                    s.Explain
                })
            });
        }
        #endregion

    }
}