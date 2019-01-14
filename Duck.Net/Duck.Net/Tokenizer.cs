﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Duck.Net
{

    public enum TokenName
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
        ENDIF,
        UNKNOWN,
        MINUS,
        MINUSMINUS
    }
    public enum TokenType
    {
        TERMINAL,
        OPERATOR,
        CONTROL,
        NO_TOKEN,
        SYMBOL,
        VAR
    }


    public class Token
    {

        
        

        public TokenName Name { get; set;}
        public TokenType Type { get; set; }
        public String Value { get; set; }

        public Token()
        {
            this.Name = TokenName.UNKNOWN;
            this.Type = TokenType.NO_TOKEN;
        }
        public Token(TokenName name, TokenType type)
        {
            this.Name = name;
            this.Type = type;
        }
    }

   
 
    class Tokenizer
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public int Cursor { get; set; }
        public String ActualLine { get; set; }
        public StreamReader Stream { get; set; }
        
        public Tokenizer(StreamReader source)
        {
            this.Cursor = 0;
            this.Stream = source;
            ActualLine = string.Empty;
           
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
                        return new Token(TokenName.IF, TokenType.CONTROL);
                       
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(then(\s*))$"))
                    {
                        Cursor += 4;
                        return new Token(TokenName.THEN, TokenType.CONTROL);
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(else if(\s+))"))
                    {
                        Cursor += 6;
                        return new Token(TokenName.ELSEIF, TokenType.CONTROL);
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(else(\s*))$"))
                    {
                        Cursor += 4;
                        return new Token(TokenName.ELSE, TokenType.CONTROL);
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(end if(\s*))$"))
                    {
                        Cursor += 6;
                        return new Token(TokenName.ENDIF, TokenType.CONTROL);
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\()"))
                    {
                        Cursor++;
                        return new Token(TokenName.LEFT_CIRCLE_BRACKET, TokenType.SYMBOL);
                    }


                    else if (catchExpression(line.Substring(Cursor), @"^(\))"))
                    {
                        Cursor++;
                        return new Token(TokenName.RIGHT_CIRCLE_BRACKET, TokenType.SYMBOL);
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\=\=)"))
                    {
                        Cursor += 2;
                        return new Token(TokenName.EQUALS, TokenType.OPERATOR);
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\!\=)"))
                    {
                        Cursor += 2;
                        return new Token(TokenName.NOTEQUALS, TokenType.OPERATOR);
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\>\=)"))
                    {
                        Cursor += 2;
                        return new Token(TokenName.MAJOREQUALS, TokenType.OPERATOR);
                    }

                    else if (catchExpression(line.Substring(Cursor), @"^(\<\=)"))
                    {
                        Cursor += 2;
                        return new Token(TokenName.MINOREQUALS, TokenType.OPERATOR);
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\+\+)"))
                    {
                        Cursor += 2;
                        return new Token(TokenName.PLUSPLUS, TokenType.OPERATOR);
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\-\-)"))
                    {
                        Cursor += 2;
                        return new Token(TokenName.MINUSMINUS, TokenType.OPERATOR);
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\>)"))
                    {
                        Cursor++;
                        return new Token(TokenName.MAJOR, TokenType.OPERATOR);
                    }
                   
                    else if (catchExpression(line.Substring(Cursor), @"^(\<)"))
                    {
                        Cursor++;
                        return new Token(TokenName.MINOR, TokenType.OPERATOR);
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\+)"))
                    {
                        Cursor++;
                        return new Token(TokenName.PLUS, TokenType.OPERATOR);
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\-)"))
                    {
                        Cursor++;
                        return new Token(TokenName.MINUS, TokenType.OPERATOR);
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\=)"))
                    {
                        Cursor++;
                        return new Token(TokenName.ASSIGN, TokenType.OPERATOR);
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^(\s)"))
                    {
                        Cursor++;
                        return new Token(TokenName.SPACE, TokenType.SYMBOL);
                    }
                    else if (catchExpression(line.Substring(Cursor), "^(\".*\")"))
                    {
                        Token newToken = new Token();
                        newToken.Value = Helper.extractStringContent(line.Substring(Cursor));
                        Cursor+= newToken.Value.Length+2;
                        newToken.Name = TokenName.STRING;
                        newToken.Type = TokenType.TERMINAL;
                        return newToken;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^([0-9])+"))
                    {
                        Regex r = new Regex(@"^([0-9])+");
                        Token newToken = new Token();
                        newToken.Value = r.Match(line.Substring(Cursor)).Value;
                        Cursor += newToken.Value.Length;
                        newToken.Name = TokenName.NUMBER;
                        newToken.Type = TokenType.TERMINAL;
                        return newToken;
                    }
                    else if (catchExpression(line.Substring(Cursor), @"^([a-zA-Z]+([0-9a-zA-Z]*))"))
                    {
                        Regex r = new Regex(@"^([a-zA-Z]+([0-9a-zA-Z]*))");
                        Token newToken = new Token();
                        newToken.Value = r.Match(line.Substring(Cursor)).Value;
                        Cursor += newToken.Value.Length;
                        newToken.Name = TokenName.VAR;
                        newToken.Type = TokenType.VAR;
                        return newToken;

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
