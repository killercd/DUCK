using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Duck.Net
{
    public enum Token
    {
        LEFT_CIRCLE_BRACKET,
        RIGHT_CIRCLE_BRACKET,
        ASSIGN,
        EQUALS,
        IF,
        THEN,
        ELSE,
        ELSEIF,
        OR,
        AND,
        NOT,
        SPACE,
        QUOTE,
        NEWLINE,
        EOF,
        NOTEQUALS,
        MAJOREQUALS,
        MINOREQUALS,
        MAJOR,
        MINOR,
        PLUS,
        MINORMINOR,
        PLUSPLUS,
        STRING,
        NUMBER,
        VAR,
        ENDIF
    }
    //class Token
    //{

    //    public Dictionary<String, Symbol> TokenList;

    //    public Token()
    //    {
    //        this.TokenList = new Dictionary<String, Symbol>();
    //        TokenList.Add("(",Symbol.LEFT_CIRCLE_BRACKET);
    //        TokenList.Add(")",Symbol.RIGHT_CIRCLE_BRACKET);
    //        TokenList.Add("=",Symbol.ASSIGN);
    //        TokenList.Add("==",Symbol.EQUALS);
    //        TokenList.Add("IF",Symbol.IF);
    //        TokenList.Add("THEN",Symbol.THEN);
    //        TokenList.Add("ELSE",Symbol.ELSE);
    //        TokenList.Add("ELSE IF",Symbol.ELSEIF);
    //        TokenList.Add("OR", Symbol.OR);
    //        TokenList.Add("AND", Symbol.AND);
    //        TokenList.Add("NOT", Symbol.NOT);
    //        TokenList.Add(" ", Symbol.SPACE);
    //    }


    //}
    class Tokenizer
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public int Cursor { get; set; }
        public String ActualLine { get; set; }
        public StreamReader Stream { get; set; }
        public String Value { get; set; }
        public Tokenizer(StreamReader source)
        {
            this.Cursor = 0;
            this.Stream = source;
            ActualLine = string.Empty;
            Value = string.Empty;
        }
        public bool nextLine()
        {
            String methodName = "nextLine";
            try
            {
                ActualLine = Stream.ReadLine();
                Cursor = 0;
                return (ActualLine != null);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }

        }
        private bool catchExpression(String str, String regex)
        {
            Regex r = new Regex(regex, RegexOptions.IgnoreCase);
            return r.IsMatch (str);
        }
       
        public Token getNextToken()
        {
            String methodName = "getNextToken";

            try
            {


                log.Info(String.Format("{0} - {1} START", this.GetType().Name, methodName));
                String line = ActualLine;
                if (Cursor < line.Length)
                {

                    if (catchExpression(line.Substring(Cursor), @"^(if(\s+))"))
                    {
                        Cursor += 2;
                        return Token.IF;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(then(\s*))$"))
                    {
                        Cursor += 4;
                        return Token.THEN;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(else if(\s+))"))
                    {
                        Cursor += 6;
                        return Token.ELSEIF;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(else(\s*))$"))
                    {
                        Cursor += 4;
                        return Token.ELSE;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(end if(\s*))$"))
                    {
                        Cursor += 6;
                        return Token.ENDIF;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\()"))
                    {
                        Cursor++;
                        return Token.LEFT_CIRCLE_BRACKET;
                    }


                    else if (catchExpression(line.Substring(Cursor), @"^(\))"))
                    {
                        Cursor++;
                        return Token.RIGHT_CIRCLE_BRACKET;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\=\=)"))
                    {
                        Cursor += 2;
                        return Token.EQUALS;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\!\=)"))
                    {
                        Cursor += 2;
                        return Token.NOTEQUALS;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\>\=)"))
                    {
                        Cursor += 2;
                        return Token.MAJOREQUALS;
                    }

                    else if (catchExpression(line.Substring(Cursor), @"^(\<\=)"))
                    {
                        Cursor += 2;
                        return Token.MINOREQUALS;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\+\+)"))
                    {
                        Cursor += 2;
                        return Token.PLUSPLUS;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\-\-)"))
                    {
                        Cursor += 2;
                        return Token.MINORMINOR;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\>)"))
                    {
                        Cursor++;
                        return Token.MAJOR;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\>)"))
                    {
                        Cursor++;
                        return Token.MAJOR;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\<)"))
                    {
                        Cursor++;
                        return Token.MINOR;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\+)"))
                    {
                        Cursor++;
                        return Token.PLUS;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\-)"))
                    {
                        Cursor++;
                        return Token.PLUS;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\=)"))
                    {
                        Cursor++;
                        return Token.ASSIGN;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\s)"))
                    {
                        Cursor++;
                        return Token.SPACE;
                    }
                    else if (catchExpression(line.Substring(Cursor), "^(\".*\")"))
                    {
                        
                        Value = Helper.extractStringContent(line.Substring(Cursor));
                        Cursor+=Value.Length+2;
                        return Token.STRING;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^([0-9])+"))
                    {
                        Regex r = new Regex(@"^([0-9])+");
                        Value = r.Match(line.Substring(Cursor)).Value;
                        Cursor += Value.Length;
                        return Token.NUMBER;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^([a-zA-Z]+([0-9a-zA-Z]*))"))
                    {
                        Regex r = new Regex(@"^([a-zA-Z]+([0-9a-zA-Z]*))");
                       
                        Value = r.Match(line.Substring(Cursor)).Value;
                        Cursor += Value.Length;
                        return Token.VAR;

                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\n)"))
                    {
                     
                        Cursor++;
                        return Token.NEWLINE;

                    }
                }
                else
                {
                    return Token.NEWLINE;
                }
                throw new Exception("Invalid token");
            }
            catch (Exception ex)
            {
                log.Error(String.Format("{0} - {1} ERROR", this.GetType().Name, methodName), ex);
                throw ex;
            }
        }
    }
}
