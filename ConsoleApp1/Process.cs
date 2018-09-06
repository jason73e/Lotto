using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ScrapURL
{
    public class Process
    {
        public void Start()
        {
            AddURLs();
            List<URLData> lsURLs = GetURLsToProcess();
            ProcessURLS(lsURLs);
            List<URLData> lsProcessedURLs = GetURLsToMine();
            MineURLS(lsProcessedURLs);

        }

        private void MineURLS(List<URLData> lsProcessedURLs)
        {
            foreach(URLData d in lsProcessedURLs)
            {
               Result r = ParseResult(d.result);
            }
        }

        private Result ParseResult(string result)
        {
            Result r = new Result();
            String source = WebUtility.HtmlDecode(result);
            HtmlDocument resultat = new HtmlDocument();
            resultat.LoadHtml(source);
            List<HtmlNode> toftitle = resultat.DocumentNode.Descendants().Where(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("block_content"))).ToList();
            return r;
        }

        private void ProcessURLS(List<URLData> lsURLs)
        {
            foreach(URLData d in lsURLs)
            {
                WebClient client = new WebClient();
                try
                {
                    d.result = client.DownloadString(d.url);
                    d.Processed = true;
                    d.tsProcessed = DateTime.Now;
                }
                catch(Exception e)
                {
                    d.result = e.Message;
                    d.Processed = true;
                    d.tsProcessed = DateTime.Now;
                    d.Mined = true;
                    d.MinedDate = DateTime.Now;
                }
                SaveURLData(d);
            }
        }

        private void SaveURLData(URLData d)
        {
            LottoEntities db = new LottoEntities();
            URLData data = db.URLDatas.Find(d.ID);
            if(data!=null)
            {
                data.Mined = d.Mined;
                data.MinedDate = d.MinedDate;
                data.Processed = d.Processed;
                data.result = d.result;
                data.tsAdded = d.tsAdded;
                data.tsProcessed = d.tsProcessed;
                data.url = d.url;
                data.DrawingDate = d.DrawingDate;
                data.Game = d.Game;
                db.SaveChanges();
            }
        }

        private List<URLData> GetURLsToMine()
        {
            LottoEntities db = new LottoEntities();
            List<URLData> ls = db.URLDatas.Where(x => x.Processed == true && x.Mined==false).ToList();
            return ls;
        }

        private List<URLData> GetURLsToProcess()
        {
            LottoEntities db = new LottoEntities();
            List<URLData> ls = db.URLDatas.Where(x => x.Processed == false).ToList();
            return ls;
        }

        private void AddURLs()
        {
            LottoEntities db = new LottoEntities();
            DateTime dtDefaultStart = Convert.ToDateTime("1/1/2000");
            string sPowerballBase = "https://www.coloradolottery.com/en/games/powerball/drawings/";
            string sMegaBase = "https://www.coloradolottery.com/en/games/megamillions/drawings/";
            string sLottoBase = "https://www.coloradolottery.com/en/games/lotto/drawings/";
            DateTime dtEnd = DateTime.Now.AddDays(-1);
            DateTime dtDrawing = dtDefaultStart;
            int iCounter = 0;
            List<URLData> lsURLs = new List<URLData>();

            if (db.URLDatas.Where(x=>x.Game=="Powerball").Any())
            {
                dtDrawing = db.URLDatas.Where(x => x.Game == "Powerball").OrderByDescending(x => x.DrawingDate).First().DrawingDate.Value.AddDays(1);
            }
            while(dtDrawing < dtEnd && iCounter<1000)
            {
                URLData d = new URLData();
                d.DrawingDate = dtDrawing;
                d.Game = "Powerball";
                d.ID = Guid.NewGuid();
                d.Mined = false;
                d.Processed = false;
                d.tsAdded = DateTime.Now;
                d.url = sPowerballBase + dtDrawing.ToString("yyyy-MM-dd") + "/";
                lsURLs.Add(d);
                iCounter++;
                dtDrawing = dtDrawing.AddDays(1);
            }
            db.URLDatas.AddRange(lsURLs);
            db.SaveChanges();
            iCounter = 0;
            lsURLs.Clear();

            if (db.URLDatas.Where(x => x.Game == "Megamillions").Any())
            {
                dtDrawing = db.URLDatas.Where(x => x.Game == "Megamillions").OrderByDescending(x => x.DrawingDate).First().DrawingDate.Value.AddDays(1);
            }
            while (dtDrawing < dtEnd && iCounter < 1000)
            {
                URLData d = new URLData();
                d.DrawingDate = dtDrawing;
                d.Game = "Megamillions";
                d.ID = Guid.NewGuid();
                d.Mined = false;
                d.Processed = false;
                d.tsAdded = DateTime.Now;
                d.url = sMegaBase + dtDrawing.ToString("yyyy-MM-dd") + "/";
                lsURLs.Add(d);
                iCounter++;
                dtDrawing = dtDrawing.AddDays(1);
            }
            db.URLDatas.AddRange(lsURLs);
            db.SaveChanges();
            iCounter = 0;
            lsURLs.Clear();

            if (db.URLDatas.Where(x => x.Game == "lotto").Any())
            {
                dtDrawing = db.URLDatas.Where(x => x.Game == "lotto").OrderByDescending(x => x.DrawingDate).First().DrawingDate.Value.AddDays(1);
            }
            while (dtDrawing < dtEnd && iCounter < 1000)
            {
                URLData d = new URLData();
                d.DrawingDate = dtDrawing;
                d.Game = "lotto";
                d.ID = Guid.NewGuid();
                d.Mined = false;
                d.Processed = false;
                d.tsAdded = DateTime.Now;
                d.url = sLottoBase + dtDrawing.ToString("yyyy-MM-dd") + "/";
                lsURLs.Add(d);
                iCounter++;
                dtDrawing = dtDrawing.AddDays(1);
            }
            db.URLDatas.AddRange(lsURLs);
            db.SaveChanges();
            iCounter = 0;
            lsURLs.Clear();
        }
    }
}
