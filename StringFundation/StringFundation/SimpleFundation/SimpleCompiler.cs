using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CodeConfusion;

namespace Unikon.serialize
{
    public class SimpleCompiler
    {
        public static string lineCash = "";
        public static int index = 0;
        public static string getFullClassName(string path)
        {
            lineCash = "";
            index = 0;
            StreamReader reader = new StreamReader(path, Encoding.UTF8);
            TonkenParser parser = new TonkenParser(reader);
            string token = "";
            string classname = "";
           // token = parser.NextToken();
            LinkedList<string> namespaces = new LinkedList<string>();
            int n_name_space_deep = 0;
            while (true)
            { 
                token = parser.NextToken();
                if (token == null)
                    break;
                
                if (token == ""||
                    token == "public"||
                    token == "sealed" ||
                    token == "static" ||
                    token == "void" ||
                    token == "private" ||
                    token == "protected" ||
                    token == "string"||
                    token == "." )
                    continue;
                if (token == "{" && n_name_space_deep > 0)
                {
                    n_name_space_deep = 0;
                    continue;
                }
                
                

                if (token == "class")
                {
                    classname = parser.NextToken();
                    break;
                }
                if( token == "namespace" )
                {
                    n_name_space_deep++;
                    continue;
                }
                if (n_name_space_deep > 0)
                    namespaces.AddLast(token);
            }
             
            reader.Close();
            StringBuilder sb = new StringBuilder();
            var it = namespaces.First;
            if(it != null)
            {
                sb.Append(it.Value);
                it = it.Next;
                while (it != null)
                {
                    sb.Append(".");
                    sb.Append(it.Value);
                    it = it.Next;
                }
                sb.Append(".");
            }
           
            sb.Append(classname);
            return sb.ToString() ;
        }


    }

}
