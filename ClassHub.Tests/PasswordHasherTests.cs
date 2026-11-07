using ClassHub.Services;

namespace ClassHub.Tests;

public sealed class PasswordHasherTests
{
    private const string Password = "ClassHub-Test-Password";

    [Fact]
    public void HashIsVersionedSaltedAndDoesNotContainPlaintext()
    {
        var first = PasswordHasher.Hash(Password);
        var second = PasswordHasher.Hash(Password);
        Assert.NotEqual(first, second);
        Assert.DoesNotContain(Password, first);
        var fields = first.Split('$');
        Assert.Equal(5, fields.Length);
        Assert.Equal("v1", fields[0]);
        Assert.Equal("pbkdf2-sha256", fields[1]);
        Assert.Equal("600000", fields[2]);
        Assert.Equal(16, Convert.FromBase64String(fields[3]).Length);
        Assert.Equal(32, Convert.FromBase64String(fields[4]).Length);
        Assert.True(PasswordHasher.IsValidHash(first));
        Assert.True(PasswordHasher.Verify(Password, first));
        Assert.False(PasswordHasher.Verify("Incorrect-Password", first));
    }

    [Theory]
    [InlineData(11)]
    [InlineData(129)]
    public void NewPasswordsOutsideLengthBoundsAreRejected(int length) =>
        Assert.Throws<ArgumentException>(() => PasswordHasher.ValidateNewPassword(new string('x', length)));

    [Theory]
    [InlineData(12)]
    [InlineData(128)]
    public void NewPasswordsAtLengthBoundsAreAccepted(int length) =>
        PasswordHasher.ValidateNewPassword(new string('x', length));

    [Theory]
    [InlineData("")]
    [InlineData("plaintext-password")]
    [InlineData("v1$pbkdf2-sha256$600000$not-base64$not-base64")]
    [InlineData("v2$pbkdf2-sha256$600000$AAAAAAAAAAAAAAAAAAAAAA==$AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=")]
    [InlineData("v1$pbkdf2-sha256$1$AAAAAAAAAAAAAAAAAAAAAA==$AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=")]
    [InlineData("v1$pbkdf2-sha256$2147483647$AAAAAAAAAAAAAAAAAAAAAA==$AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=")]
    [InlineData("v1$pbkdf2-sha256$600000$AA==$AA==")]
    public void MalformedOrUnsupportedHashesFailClosed(string hash)
    {
        Assert.False(PasswordHasher.IsValidHash(hash));
        Assert.False(PasswordHasher.Verify(Password, hash));
    }
}
