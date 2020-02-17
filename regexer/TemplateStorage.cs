using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regexer
{
    public class TemplateStorage
    {
        public Dictionary<string, string> Templates { get;  set; }    ///< Collection of templates as specified in the pattern
        public Dictionary<string, Token> Hash { get; set; }

        public TemplateStorage()
        {
            this.Templates = new Dictionary<string, string>( );
            this.Hash = new Dictionary<string, Token>( );
        }


        private Token tokenize(string key)
        {
            Token root;
            string pattern = Templates[key];

            try
            {
                root = Token.Tokenize(pattern, this);
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ParsingException("invalid pattern: " + pattern, ex);
            }

            Hash[key] = root;
            return root;
        }

        public void Set(string key, string value)
        {
            Templates[key] = value;
        }

        public Token Get(string key)
        {
            return Hash.ContainsKey(key) ? Hash[key] : tokenize(key);
        }

        /** Returns the matched group with the given name.
            * 
            *  Throws InvalidOperationException if not such group exists.
            
        public string this[string name]
        {
            set { Templates[name] = value; }
        }
*/

        /** Returns the matched group with the given index.
            
        public Token this[string index]
        {
            get { return Hash.ContainsKey(index) ? Hash[index] : tokenize(index); }
        }
        */
    }
}
