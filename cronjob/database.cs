using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebService;
using QA.API_WebService;
using System.ComponentModel;
using System.Xml.Linq;

namespace cronjob
{
    public static class database
    {
        public static void SaveData(ItemTruyenDetail item)
        {
            var query = new RequestCollection();

            query += DataQuery.Create("TruyenOnline", "ItemSave", new
            {
                ID = item.ID,
                Title = item.TenTruyen,
                Description = item.Mota,
                Author = item.TacGia,
                Cat = item.TheLoai,
                Source = item.Nguon,
                Status = item.TrangThai,
                LinkImageCover = item.LinkImageCover,
                keywordsSeo = "",
                descriptionSeo = "",
                LinkTruyen = item.LinkTruyen
            });
            for (int i = 0; i < item.Chapters.Count; i++)
            {
                query += DataQuery.Create("TruyenOnline", "ChapterSave", new
                {
                    IDChapter = item.Chapters[i].IDChapter,
                    IDItem = item.Chapters[i].IDTruyen,
                    Name = item.Chapters[i].Name,
                    Link = item.Chapters[i].link
                });

            }

            var ds = Database.ProcessRequest(query);
        }
    }
}
