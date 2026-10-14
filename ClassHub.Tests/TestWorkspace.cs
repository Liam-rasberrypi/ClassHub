using System.Text.Json;
using ClassHub.Models;

namespace ClassHub.Tests;

internal sealed class TestWorkspace : IDisposable
{
    public string DirectoryPath { get; } = Path.Combine(Path.GetTempPath(), "ClassHub.Tests", Guid.NewGuid().ToString("N"));
    public string AccountPath => Path.Combine(DirectoryPath, "accounts.json");
    public string NoticePath => Path.Combine(DirectoryPath, "notices.json");

    public TestWorkspace() => Directory.CreateDirectory(DirectoryPath);

    public void WriteAccounts(params Account[] accounts) =>
        File.WriteAllText(AccountPath, JsonSerializer.Serialize(accounts));

    public void WriteNotices(params NoticeItem[] notices) =>
        File.WriteAllText(NoticePath, JsonSerializer.Serialize(notices));

    public void Dispose() => Directory.Delete(DirectoryPath, recursive: true);
}

internal sealed class ManualTimeProvider : TimeProvider
{
    private long _timestamp;
    public override long TimestampFrequency => TimeSpan.TicksPerSecond;
    public override long GetTimestamp() => _timestamp;
    public override DateTimeOffset GetUtcNow() => DateTimeOffset.UnixEpoch.AddTicks(_timestamp);
    public void Advance(TimeSpan duration) => _timestamp += duration.Ticks;
}
