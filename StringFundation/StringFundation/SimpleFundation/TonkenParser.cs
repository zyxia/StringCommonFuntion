using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeConfusion
{
    public static class Token
    { 
        public static bool isComment(string strToken)
        {
            if (strToken.Length > 2)
            {
                string sub = strToken.Substring(0, 2);
                if (sub == "//" || sub == "/*")
                    return true;
            }
            return false;
        }

    }
    class TonkenParser
    {
        static StringBuilder tempText = new StringBuilder();

        static readonly string invalideStr = " \n\r";
         
        static void push(char c)
        {
            if (!invalideStr.Contains(c)&&c !=0xef && c != 0xbb && c != 0xbf)
            {
                tempText.Append(c);
            }
        }

        static bool TempTextSelfIsSeparator()
        {
            if(tempText.Length == 1)
            {
                string srcSeparator = "-+*/%;,()<>=.[]{}!~";
                return srcSeparator.Contains(tempText[tempText.Length-1]);
            } 
            return false;
        }

        
        static bool canNextToken(char c)
        {
            string srcSeparator = " \n\r-+*/%;,()<>=[]{}!~";
            return  TempTextSelfIsSeparator() || (tempText.Length>0 && srcSeparator.Contains(c)) /*|| (!Tool.isInterger(tempText)&&c=='.')*/;
        }
         
        StringTokenState gStringState = null;
        ComentTokenState gCommentState = null;
        StreamReader reader =  null;
        int i = 0;
        public TonkenParser(StreamReader r)
        {
            reader = r;
            reader.Read();
            reader.DiscardBufferedData();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            var check = reader.Read();
            if (check > '\u4E00')
            {
                i = 3;
            }
        }
        public string NextToken()
        {
            string result = "";
          
         
            while (!reader.EndOfStream)
            {
                reader.DiscardBufferedData();
                reader.BaseStream.Seek(i, SeekOrigin.Begin);
                if (gStringState == null && gCommentState == null)
                {
                    char c = (char)reader.Read();
                    reader.BaseStream.Seek(i+1, SeekOrigin.Begin);
                    bool isPreEnd = reader.EndOfStream;
                    char test = ' ';
                    if (!isPreEnd)
                        test = (char)reader.Read();
                    if (c == '\"')
                    {
                        gStringState = new StringTokenState();
                        gStringState.push(c);
                    }
                    else if (c == '/' && !isPreEnd && (test == '/' || test == '*'))
                    {
                        gCommentState = new ComentTokenState();
                        gCommentState.push(c);
                    }
                    else
                    {
                        if (canNextToken(c))
                        {
                            result = tempText.ToString(); 
                            tempText.Clear();
                            push(c);
                            i++;
                            break;
                        }
                        else
                        {
                            push(c);
                        }
                    }
                }
                else if (gStringState != null)
                {
                    char c = (char)reader.Read();
                    gStringState.push(c);
                    if (gStringState.isStringEnd())
                    {
                        result = gStringState.textTemp;
                        gStringState = null;
                        i++;
                        break;
                    }
                }
                else if (gCommentState != null)
                {
                    char c = (char)reader.Read();
                    gCommentState.push(c);
                    if (gCommentState.isComentEnd())
                    {
                        result = gCommentState.textTemp;
                        gCommentState = null;
                        i++;
                        break;
                    }
                }
                i++;
            }
           
            return result;
        }
       
    }
}
