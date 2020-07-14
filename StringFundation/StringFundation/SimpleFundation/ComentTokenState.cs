using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 
class ComentTokenState
{
    public string textTemp;
    

    public bool isComentEnd()
    {
        if (textTemp.Length <= 2)
            return false;
        string head = textTemp.Substring(0,2);
        if( head == "//" )
        {
            string end = textTemp.Substring(textTemp.Length-2, 1);
            if( end == "\n" )
            {
                return true;
            }
        }else if(head == "/*")
        {
            string end = textTemp.Substring(textTemp.Length - 3, 2);
            if (end == "*/")
            {
                return true;
            }
        }
        return false;
    }

    public void push(char c)
    {
        textTemp += c;
    }
}
 
