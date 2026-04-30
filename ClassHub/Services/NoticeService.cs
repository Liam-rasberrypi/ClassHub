// Services/NoticeService.cs
// 公告服务：负责公告读取、发布、置顶、删除
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClassHub.Helpers;
using ClassHub.Models;

namespace ClassHub.Services
{
    public interface INoticeService
    {
        IReadOnlyList<NoticeItem> GetAll();
        NoticeItem Add(string title, string content, bool isPinned);
        void TogglePin(Guid noticeId);
        void Delete(Guid noticeId);
    }

    public class NoticeService : INoticeService
    {
        private readonly List<NoticeItem> _items;
        private readonly string _storagePath;

        public NoticeService()
        {
            _storagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "notices.json");
            _items = LocalStorageHelper.LoadJson<List<NoticeItem>>(_storagePath) ?? new List<NoticeItem>();
        }

        public IReadOnlyList<NoticeItem> GetAll()
        {
            return _items
                .OrderByDescending(x => x.IsPinned)
                .ThenByDescending(x => x.CreatedAt)
                .ToList();
        }

        public NoticeItem Add(string title, string content, bool isPinned)
        {
            var notice = new NoticeItem
            {
                Title = title.Trim(),
                Content = content.Trim(),
                IsPinned = isPinned,
                CreatedAt = DateTime.Now
            };

            _items.Add(notice);
            Save();
            return notice;
        }

        public void TogglePin(Guid noticeId)
        {
            var target = _items.FirstOrDefault(x => x.Id == noticeId);
            if (target == null)
            {
                return;
            }

            target.IsPinned = !target.IsPinned;
            Save();
        }

        public void Delete(Guid noticeId)
        {
            var target = _items.FirstOrDefault(x => x.Id == noticeId);
            if (target == null)
            {
                return;
            }

            _items.Remove(target);
            Save();
        }

        private void Save()
        {
            LocalStorageHelper.SaveJson(_storagePath, _items);
        }
    }
}
