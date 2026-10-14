using System.Text.Json;
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
        private List<NoticeItem> _items;
        private readonly ISecurityService _security;
        private readonly string _storagePath;
        private readonly object _sync = new();

        // No constructor supplies an unauthenticated bypass.
        public NoticeService(ISecurityService securityService, string? storagePath = null)
        {
            _security = securityService ?? throw new ArgumentNullException(nameof(securityService));
            _storagePath = Path.GetFullPath(storagePath ??
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "notices.json"));
            _items = LocalStorageHelper.LoadJson<List<NoticeItem>>(_storagePath);
        }

        public IReadOnlyList<NoticeItem> GetAll()
        {
            lock (_sync)
            {
                // Copies prevent callers from editing notices through the read API.
                return _items.OrderByDescending(x => x.IsPinned)
                    .ThenByDescending(x => x.CreatedAt).Select(Clone).ToList();
            }
        }

        public NoticeItem Add(string title, string content, bool isPinned)
        {
            lock (_sync)
            {
                _security.DemandPermission(PermissionKey.NoticeEdit);
                if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
                    throw new ArgumentException("请填写公告标题和内容。");
                var notice = new NoticeItem
                {
                    Title = title.Trim(), Content = content.Trim(),
                    IsPinned = isPinned, CreatedAt = DateTime.Now
                };
                var updated = _items.Select(Clone).ToList();
                updated.Add(notice);
                Save(updated);
                _items = updated;
                return Clone(notice);
            }
        }

        public void TogglePin(Guid noticeId)
        {
            lock (_sync)
            {
                _security.DemandPermission(PermissionKey.NoticeEdit);
                var updated = _items.Select(Clone).ToList();
                var target = updated.FirstOrDefault(x => x.Id == noticeId);
                if (target == null) return;
                target.IsPinned = !target.IsPinned;
                Save(updated);
                _items = updated;
            }
        }

        public void Delete(Guid noticeId)
        {
            lock (_sync)
            {
                _security.DemandPermission(PermissionKey.NoticeEdit);
                var updated = _items.Select(Clone).ToList();
                if (updated.RemoveAll(x => x.Id == noticeId) == 0) return;
                Save(updated);
                _items = updated;
            }
        }

        private void Save(List<NoticeItem> items)
        {
            var directory = Path.GetDirectoryName(_storagePath)!;
            var temporaryPath = Path.Combine(directory, $".notices-{Guid.NewGuid():N}.tmp");
            try
            {
                Directory.CreateDirectory(directory);
                using (var stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write,
                    FileShare.None))
                {
                    JsonSerializer.Serialize(stream, items, new JsonSerializerOptions { WriteIndented = true });
                    stream.Flush(flushToDisk: true);
                }
                // Commit on disk before swapping memory; failed saves keep both intact.
                File.Move(temporaryPath, _storagePath, overwrite: true);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new IOException("无法保存公告，请检查数据目录的写入权限。", ex);
            }
            finally
            {
                try { File.Delete(temporaryPath); }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
        }

        private static NoticeItem Clone(NoticeItem item) => new()
        {
            Id = item.Id, Title = item.Title, Content = item.Content,
            IsPinned = item.IsPinned, CreatedAt = item.CreatedAt
        };
    }
}
