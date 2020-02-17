using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regexer
{
    public class TemplateToken : Token
    {

        //public bool IsNegative { get; set; }

        private string key { get; set; }
        
        private TemplateStorage Storage { get; set; }

        private Token _token { get; set; }
        //private TemplateStorage Storage;

        public TemplateToken(string key, TemplateStorage storage)
            : base(TokenType.TemplateToken, key)
        {
            this.Storage = storage;
            this.key = key;
        }

        public override bool Matches(string input, ref int cursor)
        {
            Token _token = Storage.Get(key);
            if (_token.Matches(input, ref cursor))
                return true;
            
            return false;
        }

        public override bool CanBacktrack(string input, ref int cursor)
        {
            /*
            if (_root.CanBacktrack(input, ref cursor))
                return true;
            */
            return false;
        }

        protected override string printContent()
        {
            return "TemplateToken(";
            //return _token.ToString();
        }
    }
}
