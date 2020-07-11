using System;
using System.Collections.Generic;
using System.Text;

namespace StringFundation
{
    class UTF8
    {
        public static bool IsStringValidUTF8(string source)
        {
            int result = 0;
            int end = source.Length;
            while (result != end)
            {
                int t = 0;
                utf_error err_code = validate_next(in source,ref result, end,t);
                if (err_code != utf_error.UTF8_OK)
                    return result>= end;
            }
            return result>= end;
        }

        enum utf_error { UTF8_OK, NOT_ENOUGH_ROOM, INVALID_LEAD, INCOMPLETE_SEQUENCE, OVERLONG_SEQUENCE, INVALID_CODE_POINT };

        static byte mask8(char oc)
        {
            return (byte)(0xff & oc);
        }
        static int sequence_length(in string src,int lead_it)
    {
        byte lead = mask8(src[lead_it]);
        if (lead< 0x80)
            return 1;
        else if ((lead >> 5) == 0x6)
            return 2;
        else if ((lead >> 4) == 0xe)
            return 3;
        else if ((lead >> 3) == 0x1e)
            return 4;
        else
            return 0;
    }

        static utf_error get_sequence_1(in string src,int it, int end, ref int code_point)
        {
            if (it == end)
                return utf_error.NOT_ENOUGH_ROOM;

            code_point = mask8(src[it]);

             return utf_error.UTF8_OK;
        }

        static bool is_trail(char oc)
    {
            return ((mask8(oc) >> 6) == 0x2);
    }
        static utf_error increase_safely(in string src, int it, int end)
        {
            if (++it == end)
                return utf_error.NOT_ENOUGH_ROOM;

            if (!is_trail(src[it]))
            return utf_error.INCOMPLETE_SEQUENCE;

        return utf_error.UTF8_OK;
    }

        static utf_error get_sequence_2(in string src, int it, int end, ref int code_point)
        {
            if (it == end)
                return utf_error.NOT_ENOUGH_ROOM;

            code_point = mask8(src[it]);


            utf_error ret = increase_safely(src,it, end);
            if (ret != utf_error.UTF8_OK) return ret;
          

            code_point = ((code_point << 6) & 0x7ff) + ((src[it]) & 0x3f);

            return utf_error.UTF8_OK;
        }


        static utf_error get_sequence_3(in string src, int it, int end, ref int code_point)
{
    if (it == end)
        return utf_error.NOT_ENOUGH_ROOM;

     code_point = mask8(src[it]);

            {
                utf_error ret1 = increase_safely(src, it, end);
                if (ret1 != utf_error.UTF8_OK) return ret1;
            }


            code_point = ((code_point << 12) & 0xffff) + ((mask8(src[it]) << 6) & 0xfff);

            utf_error ret2 = increase_safely(src, it, end);
            if (ret2 != utf_error.UTF8_OK) return ret2;

            code_point += (src[it]) & 0x3f;

        return utf_error.UTF8_OK;
    }


        static utf_error get_sequence_4(in string src, int it, int end, ref int code_point)
{
    if (it == end)
        return utf_error.NOT_ENOUGH_ROOM;

            code_point = mask8(src[it]);

            utf_error ret2 = increase_safely(src, it, end);
            if (ret2 != utf_error.UTF8_OK) return ret2;

            code_point = ((code_point << 18) & 0x1fffff) + ((mask8(src[it]) << 12) & 0x3ffff);

            ret2 = increase_safely(src, it, end);
            if (ret2 != utf_error.UTF8_OK) return ret2;

            code_point += (mask8(src[it]) << 6) & 0xfff;

            ret2 = increase_safely(src, it, end);
            if (ret2 != utf_error.UTF8_OK) return ret2;

            code_point += (src[it]) & 0x3f;

        return utf_error.UTF8_OK;
    }
        static readonly UInt32 CODE_POINT_MAX = 0x0010ffffu;
        static bool is_code_point_valid(UInt32 cp)
    {
            return (cp <= CODE_POINT_MAX && !is_surrogate(cp));
    }
        static readonly    UInt16 LEAD_SURROGATE_MIN = (UInt16)0xd800u;
        static readonly UInt16 TRAIL_SURROGATE_MAX = (UInt16)0xdfffu;
        static bool is_surrogate(UInt32 cp)
    {
        return (cp >= LEAD_SURROGATE_MIN && cp <= TRAIL_SURROGATE_MAX);
    }

        static bool is_overlong_sequence(UInt32 cp, int length)
        {
            if (cp < 0x80)
            {
                if (length != 1)
                    return true;
            }
            else if (cp < 0x800)
            {
                if (length != 2)
                    return true;
            }
            else if (cp < 0x10000)
            {
                if (length != 3)
                    return true;
            }

            return false;
        }
        static utf_error validate_next(in string src, ref int it, int end, ref int code_point)
    {
            if (it == end)
                return utf_error.NOT_ENOUGH_ROOM;

            // Save the original value of it so we can go back in case of failure
            // Of course, it does not make much sense with i.e. stream iterators
            int original_it = it;

            Int32 cp = 0;
            // Determine the sequence length based on the lead octet
   
            int length = sequence_length(src,it);

        // Get trail octets and calculate the code point
                utf_error err = utf_error.UTF8_OK;
                switch (length) {
                    case 0:
                        return utf_error.INVALID_LEAD;
                    case 1:
                        err = get_sequence_1(src,it, end, ref cp);
                        break;
                    case 2:
                        err = get_sequence_2(src,it, end, ref cp);
                    break;
                    case 3:
                        err = get_sequence_3(src,it, end, ref cp);
                    break;
                    case 4:
                        err = get_sequence_4(src,it, end,ref cp);
                    break;
                }

                if (err == utf_error.UTF8_OK) {
                    // Decoding succeeded. Now, security checks...
                    if (is_code_point_valid((UInt32)cp)) {
                        if (!is_overlong_sequence((UInt32)cp, length)){
                            // Passed! Return here.
                            code_point = cp;
                            ++it;
                            return utf_error.UTF8_OK;
                        }
                        else
                            err = utf_error.OVERLONG_SEQUENCE;
                    }
                    else
                        err = utf_error.INVALID_CODE_POINT;
                }

                // Failure branch - restore the original value of the iterator
                it = original_it;
                return err;
            }

    }
}
