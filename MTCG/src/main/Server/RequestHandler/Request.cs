using System;
using System.Collections.Generic;

namespace MTCG.src.main.Server.RQ
{
    public class Request
    {
        private String HTTPMethod;
        private String requestPath;
        private String requestVersion;
        private String PostContent;
        private Dictionary<String, String> headers;

        public Request()
        {
            this.headers = new Dictionary<String, String>();
        }
        public String getHTTPMethod() {
            return HTTPMethod;
        }
        public String getPostContent()
        {
            return PostContent;
        }
        public void setPostContent(String PostContent)
        {
            this.PostContent = PostContent;
        }
        public void setHTTPMethod(String HTTPMethod)
        {
            this.HTTPMethod = HTTPMethod;
        }
        public String getRequestPath()
        {
            return requestPath;
        }
        public void setRequestPath(String requestPath)
        {
            this.requestPath = requestPath;
        }
        public String getRequestVersion()
        {
            return requestVersion;
        }
        public void setRequestVersion(String requestVersion)
        {
            this.requestVersion = requestVersion;
        }


        public String GetHeader(String header)
        {
            if (headers.ContainsKey(header))
            {
                return headers[header];
            }

            return "notFound";
        }

        public void AddHeader(String header, String value)
        {
            headers[header] = value;
        }

        public Dictionary<String, String> GetHeaderMap()
        {
            return headers;
        }

        public override String ToString()
        {
            return $"Request{{HTTPMethod='{HTTPMethod}', requestPath='{requestPath}', requestVersion='{requestVersion}', postContent='{PostContent}', headers={{{string.Join(", ", headers)}}}}}";
        }
    }


}
