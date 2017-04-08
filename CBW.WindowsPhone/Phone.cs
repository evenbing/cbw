using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBW.WindowsPhone
{
    public static class Phone
    {
        public static string ID { get; private set; }

        static Phone()
        {
            object uniqueID;
            if (Microsoft.Phone.Info.UserExtendedProperties.TryGetValue("ANID", out uniqueID))
            {
                ID = uniqueID.ToString();
            }
            else
            {
                if (Microsoft.Phone.Info.DeviceExtendedProperties.TryGetValue("DeviceUniqueId", out uniqueID) == true)
                {
                    byte[] bID = (byte[])uniqueID;
                    StringBuilder hex = new StringBuilder(bID.Length * 2);
                    foreach (byte b in bID)
                    {
                        hex.AppendFormat("{0:x2}", b);
                    }
                    ID = hex.ToString();
                }
                else
                {
                    ID = "test1";
                }
            }
        }
    }
}
