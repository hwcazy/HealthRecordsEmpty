using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiXinYY.Content.Bean
{
    public class OpenPat
    {
        string _access_token;
        int _expires_in;
        string _refresh_token;
        string _openid;
        string _scope;
        string _errcode;
        string _errmsg;
        string _hints;

        public string errcode
        {
            get { return _errcode; }
            set { _errcode = value; }
        }
        public string errmsg
        {
            get { return _errmsg; }
            set { _errmsg = value; }
        }
        public string hints
        {
            get { return _hints; }
            set { _hints = value; }
        }
        public string access_token
        {
            get { return _access_token; }
            set { _access_token = value; }
        }

        public int expires_in
        {
            get { return _expires_in; }
            set { _expires_in = value; }
        }

        public string refresh_token
        {
            get { return _refresh_token; }
            set { _refresh_token = value; }
        }


        public string openid
        {
            get { return _openid; }
            set { _openid = value; }
        }

        public string scope
        {
            get { return _scope; }
            set { _scope = value; }
        }

    }

}