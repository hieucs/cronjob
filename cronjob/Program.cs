using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

namespace cronjob
{
    internal class Program
    {
        static List<ItemTruyen> DanhsachtruyenDahoanThanh;
        static void Main(string[] args)
        {
            DanhsachtruyenDahoanThanh = new List<ItemTruyen>();

            Parallel.For(1, 652, i =>
            {
                if (i == 1)
                {
                    DanhSachTruyenFull("https://truyenfull.vn/danh-sach/truyen-full/");
                }
                else
                {
                    DanhSachTruyenFull("https://truyenfull.vn/danh-sach/truyen-full/trang-" + i + "/");
                }
            });
            Parallel.For(0, DanhsachtruyenDahoanThanh.Count, i =>
            {
                DanhsachtruyenDahoanThanh[i].ID = i;
                var itemtruyen = new ItemTruyenDetail();
                itemtruyen.ID = i;
                itemtruyen.TenTruyen = DanhsachtruyenDahoanThanh[i].Title;
                itemtruyen.LinkTruyen = DanhsachtruyenDahoanThanh[i].Link;
                itemtruyen.Chapters = new List<Chapter>();
                
                database.SaveData(itemtruyen);
                //              Thread.Sleep(1000);
                //                getNoiDungTruyen(DanhsachtruyenDahoanThanh[i].Link, i, DanhsachtruyenDahoanThanh[i].Title);

            });
        }
        public static void DanhSachTruyenFull(string link)
        {
            WebClient client = new WebClient();


            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36 Edg/108.0.1462.54");
            Console.WriteLine(link);

            var htmlDoc = new HtmlDocument();
            Thread.Sleep(1000);
            htmlDoc.LoadHtml(Encoding.UTF8.GetString(client.DownloadData(link)));
            foreach (HtmlNode nodes in htmlDoc.DocumentNode.SelectNodes("//*[@id=\"list-page\"]/div[1]/div[2]"))
            {
                Console.WriteLine(nodes.ChildNodes.Count);
                Parallel.For(1, nodes.ChildNodes.Count, i =>
                {
                    var item = new ItemTruyen();
                    item.Title = nodes.ChildNodes[i].SelectSingleNode(".//h3").InnerText;
                    // item.LinkCover = nodes.ChildNodes[i].SelectSingleNode(".//img").Attributes["src"].Value;
                    item.Link = nodes.ChildNodes[i].SelectSingleNode(".//a").Attributes["href"].Value;

                    DanhsachtruyenDahoanThanh.Add(item);
                });
            }
        }
        public static void getNoiDungTruyen(string linkTruyen, int IDTruyen, string TenTruyen)
        {
            WebClient client = new WebClient();

            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36 Edg/108.0.1462.54");
            var itemtruyen = new ItemTruyenDetail();
            itemtruyen.ID = IDTruyen;
            itemtruyen.TenTruyen = TenTruyen;
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(Encoding.UTF8.GetString(client.DownloadData(linkTruyen)));
            if(htmlDoc.Text=="")
            {
                return;
            }
            itemtruyen.Mota = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"truyen\"]/div[1]/div[1]/div[3]/div[2]").InnerText;
            itemtruyen.TacGia = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"truyen\"]/div[1]/div[1]/div[2]/div[2]/div[1]").InnerText.Replace("Tác giả:","");
            itemtruyen.TheLoai = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"truyen\"]/div[1]/div[1]/div[2]/div[2]/div[2]").InnerText.Replace("Thể loại:", "");
            var Nguon = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"truyen\"]/div[1]/div[1]/div[2]/div[2]/div[3]").InnerText;
            itemtruyen.Nguon = Nguon.Contains("Trạng thái") ? "" : Nguon.Replace("Nguồn:", "");
            var TrangThai = Nguon.Contains("Trạng thái") ? Nguon.Replace("Trạng thái:", "") :
                htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"truyen\"]/div[1]/div[1]/div[2]/div[2]/div[4]").InnerText.Replace("Trạng thái:", "");
            itemtruyen.TrangThai = TrangThai;
            itemtruyen.LinkImageCover = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"truyen\"]/div[1]/div[1]/div[2]/div[1]/div/img").Attributes["src"].Value;
            itemtruyen.Chapters = new List<Chapter>();
            ///lấy chapter trang đầu tiên
            ////////
            if (htmlDoc.DocumentNode.SelectNodes("//*[@id=\"list-chapter\"]/div[2]") != null)
            {
                foreach (HtmlNode nodes in htmlDoc.DocumentNode.SelectNodes("//*[@id=\"list-chapter\"]/div[2]"))
                {
                    for (int i = 0; i < nodes.ChildNodes.Count; i++)
                    {
                        for (int j = 0; j < nodes.ChildNodes[i].ChildNodes.Count; j++)
                        {
                            for (int k = 0; k < nodes.ChildNodes[i].ChildNodes[j].ChildNodes.Count; k++)
                            {
                                var nodess = nodes.ChildNodes[i].ChildNodes[j].ChildNodes[k];

                                var itemChapter = new Chapter();
                                itemChapter.IDTruyen = IDTruyen;
                                itemChapter.link = nodess.SelectSingleNode(".//a").Attributes["href"].Value;
                                itemChapter.Name = nodess.SelectSingleNode(".//a").InnerText;
                                itemtruyen.Chapters.Add(itemChapter);
                            }
                        }

                    }
                }
            }
            var totalPage = htmlDoc.GetElementbyId("total-page").Attributes["Value"].Value.ToInt32();
            if (totalPage > 1)
            {
                for (int t = 2; t <= totalPage; t++)
                {
                    WebClient client2 = new WebClient();
                    client2.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36 Edg/108.0.1462.54");

                    var htmlDoc2 = new HtmlDocument();
                    htmlDoc2.LoadHtml(Encoding.UTF8.GetString(client2.DownloadData(linkTruyen+ "/trang-"+t.ToString()+"/")));
                    foreach (HtmlNode nodes2 in htmlDoc2.DocumentNode.SelectNodes("//*[@id=\"list-chapter\"]/div[2]"))
                    {

                        for (int i = 0; i < nodes2.ChildNodes.Count; i++)
                        {
                            for (int j = 0; j < nodes2.ChildNodes[i].ChildNodes.Count; j++)
                            {
                                for (int k = 0; k < nodes2.ChildNodes[i].ChildNodes[j].ChildNodes.Count; k++)
                                {
                                    var nodess = nodes2.ChildNodes[i].ChildNodes[j].ChildNodes[k];

                                    var itemChapter = new Chapter();
                                    itemChapter.IDTruyen = IDTruyen;
                                    itemChapter.link = nodess.SelectSingleNode(".//a").Attributes["href"].Value;
                                    itemChapter.Name = nodess.SelectSingleNode(".//a").InnerText;
                                    itemtruyen.Chapters.Add(itemChapter);
                                }
                            }

                        }
                    }
                }
            }

            /////////
            
            for (int i = 0; i < itemtruyen.Chapters.Count; i++)
            {
                itemtruyen.Chapters[i].IDChapter = i;
            }
            //Parallel.For(0, itemtruyen.Chapters.Count, i =>
            //{
            //    WebClient clientNoiDung = new WebClient();
            //    clientNoiDung.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36 Edg/108.0.1462.54");
            //    var htmlDocNoiDung = new HtmlDocument();
            //    Thread.Sleep(1500);
            //    htmlDocNoiDung.LoadHtml(Encoding.UTF8.GetString(clientNoiDung.DownloadData(itemtruyen.Chapters[i].link)));
            //    itemtruyen.Chapters[i].Detail = htmlDocNoiDung.DocumentNode.SelectSingleNode("//*[@id=\"chapter-c\"]").InnerText;
            //});

            database.SaveData(itemtruyen);
        }

        private static void ClientNoiDung_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            var htmlDocNoiDung = new HtmlDocument();
            var noidung = Encoding.UTF8.GetString(e.Result);
            htmlDocNoiDung.LoadHtml(noidung);
            var a = htmlDocNoiDung.DocumentNode.SelectSingleNode("//*[@id=\"chapter-c\"]").InnerText;
        }
        public static string RemoveUnicode(string s)
        {
            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(stFormD[ich]);
            }
            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }
    }

    public class ItemTruyen
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string LinkCover { get; set; }
    }
    public class ItemTruyenDetail
    {
        public int ID { get; set; }
        public string TenTruyen { get; set; } = "";
        public string LinkImageCover { get; set; } = "";
        public string TacGia { get; set; } = "";
        public string TheLoai { get; set; } = "";
        public string Nguon { get; set; } = "";
        public string TrangThai { get; set; } = "";
        public string Mota { get; set; } = "";
        public List<Chapter> Chapters { get; set; }
        public string LinkTruyen { get; set; } = "";
    }
    public class DetailTruyen
    {
        public int IDChapter { get; set; }
        public int IDTruyen { get; set; }
        public string Detail { get; set; }
    }
    public class Chapter
    {
        public int IDTruyen { get; set; }
        public int IDChapter { get; set; }
        public string Name { get; set; }
        public string link { get; set; }
        public string Detail { get; set; }
    }
}