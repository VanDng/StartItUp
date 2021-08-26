using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CommonImplementation.System
{
    public static class Network
    {
        static public bool IsInternetRecentlyConnected(string target = null)
        {
            bool isConnected = false;

            try
            {
                using (var client = new WebClient())
                {
                    string url = "http://www.google.com.vn";

                    if (!string.IsNullOrEmpty(target))
                    {
                        url = target;
                    }

                    using (client.OpenRead(url))
                    {
                        isConnected = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return isConnected;
        }
    }
}
