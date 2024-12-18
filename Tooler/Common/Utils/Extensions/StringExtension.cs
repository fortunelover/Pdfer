using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tooler.Common.Utils.Extensions
{
    public static class StringExtensions
    {
        // 去除指定字符的方法
        public static string RemoveString(this string input, params string[] charsToRemove)
        {
            if (string.IsNullOrWhiteSpace(input) || charsToRemove == null || charsToRemove.Length == 0)
            {
                return input; // 返回原始输入
            }

            foreach (var c in charsToRemove)
            {
                input = input.Replace(c.ToString(), string.Empty);
            }

            return input;
        }
    }
}
