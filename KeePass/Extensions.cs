using KeePassLib;

namespace KeePass
{
    public static class Extensions
    {
        public static string ToUtf8String(this byte[] bytes )
        {
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        public static string GetTitle(this PwEntry entry)
        {
            return entry.GetString("Title");
        }

        public static string GetUserName(this PwEntry entry)
        {
            return entry.GetString("UserName");
        }

        public static string GetPassword(this PwEntry entry)
        {
            return entry.GetString("Password");
        }

        public static string GetString(this PwEntry entry, string key)
        {
            return entry.Strings?.Get(key)?.ReadUtf8()?.ToUtf8String() ?? "";
        }
    }
}