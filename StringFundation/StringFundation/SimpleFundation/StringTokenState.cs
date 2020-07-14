using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 
class StringTokenState
{
    public string textTemp;


    public static int rCharCount(ref string value, int checkLen, char c)
    {
        int count = 0;
        for(int i=0;i<checkLen;i++)
        {
            if (value[i] == c)
                count++;

        }
        return count;
    }

    public bool isStringEnd()
    {
        if(textTemp.Length>2 && textTemp[textTemp.Length-1] == '\"')
        {
            int count = rCharCount(ref textTemp, textTemp.Length - 2,'\\');
            int re = count % 2;
            if (re == 0)
                return true;
        }
        return false; 
    }
   
    public void push(char c)
    {
        textTemp += c;
    }


}
 
