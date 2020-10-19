using System.Text.RegularExpressions;

namespace MarekMotykaBot.Services.Core
{
    public static class RegexChecker
    {
        private static readonly Regex WhatRegex = new Regex(@"^\.*\s*c+o+[?!]*$", RegexOptions.IgnoreCase);

        public static bool IsWhatWord(string message) => WhatRegex.IsMatch(message);
    }
}
