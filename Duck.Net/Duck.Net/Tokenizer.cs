using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duck.Net
{

    class Token
    {
        public String Symbol { get; set; }
        public String Regex { get; set; }

        public Token(String symbol, String regex)
        {
            this.Symbol = symbol;
            this.Regex = regex;
        }
    }
    class Tokenizer
    {
        public Dictionary<String, Token> TokenList { get; set; }

        public Tokenizer()
        {
            this.TokenList = new Dictionary<String, Token>();
            this.TokenList.Add("LEFT_CIRCLE_BRACKET", new Token("(", "(\\()"));
            this.TokenList.Add("RIGHT_CIRCLE_BRACKET", new Token(")", "(\\()"));
            this.TokenList.Add("ASSIGN", new Token("=", "(\\()"));
        }
    }
}
