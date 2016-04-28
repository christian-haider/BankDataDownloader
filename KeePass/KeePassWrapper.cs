using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Keys;
using KeePassLib.Serialization;

namespace KeePass
{
    public class KeePassWrapper : IDisposable
    {
        public string Path { get; private set; }
        public PwDatabase Database { get; private set; }

        private KeePassWrapper()
        {

        }

        public static KeePassWrapper OpenWithPassword(string path, string password)
        {
            var wrapper = new KeePassWrapper
            {
                Path = path,
                Database = new PwDatabase()
            };
            var io = IOConnectionInfo.FromPath(path);
            var masterpass = new KcpPassword(password);
            var compositeKey = new CompositeKey();
            compositeKey.AddUserKey(masterpass);
            wrapper.Database.Open(io, compositeKey, new NullStatusLogger());
            return wrapper;
        }

        /// <summary>
        /// finds the entry for the unique title
        /// </summary>
        /// <param name="title">Must be uniqu in the whole database</param>
        /// <returns></returns>
        public PwEntry GetEntryByTitle(string title)
        {
            
            return GetAllEntries().SingleOrDefault(entry => entry.GetTitle()?.Equals(title) ?? false);
        }

        public PwEntry GetEntryByUuid(string uuidHex)
        {
            var uuid = new PwUuid(SoapHexBinary.Parse(uuidHex).Value);
            return GetAllEntries().SingleOrDefault(entry => entry.Uuid.Equals(uuid));
        }

        public List<PwEntry> GetAllEntries()
        {
            return PwGroup.GetFlatEntryList(Database.RootGroup.GetFlatGroupList()).ToList();
        } 

        public void Dispose()
        {
            Database.Close();
        }
    }
}
