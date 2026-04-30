// NoticeItem.cs
// 公告数据模型
using System;

namespace ClassHub.Models
{
    public class NoticeItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public bool IsPinned { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public override string ToString()
        {
            var pinPrefix = IsPinned ? "[置顶] " : string.Empty;
            return $"{pinPrefix}{Title} ({CreatedAt:MM-dd HH:mm})";
        }
    }
}

