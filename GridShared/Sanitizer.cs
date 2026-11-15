using System;
using System.Text.RegularExpressions;

namespace GridShared
{
    /// <summary>
    ///     Default ISanitizer implementation
    /// </summary>
    public partial class Sanitizer : ISanitizer
    {
        private const string TagsPattern = "<[^>]*(>|$)";

        private const string WhitelistPattern = @"
                ^</?(b(lockquote)?|code|d(d|t|l|el)|em|h(1|2|3)|i|kbd|li|ol|p(re)?|s(ub|up|trong|trike)?|ul)>$|
                ^<(b|h)r\s?/?>$";

        private const string WhitelistAPattern = @"
                ^<a\s
                href=""(\#\d+|(https?|ftp)://[-a-z0-9+&@#/%?=~_|!:,.;\(\)]+)""
                (\stitle=""[^""<>]+"")?\s?>$|
                ^</a>$";

        private const string WhitelistImgPattern = @"
                ^<img\s
                src=""https?://[-a-z0-9+&@#/%?=~_|!:,.;\(\)]+""
                (\swidth=""\d{1,3}"")?
                (\sheight=""\d{1,3}"")?
                (\salt=""[^""<>]*"")?
                (\stitle=""[^""<>]*"")?
                \s?/?>$";

        private const RegexOptions TagsOptions = RegexOptions.Singleline | RegexOptions.ExplicitCapture;

        private const RegexOptions WhitelistOptions = RegexOptions.Singleline | RegexOptions.ExplicitCapture |
            RegexOptions.IgnorePatternWhitespace;

#if NET7_0_OR_GREATER
        // Generated at build time. RegexOptions.Compiled does nothing on Blazor WebAssembly, where code
        // cannot be emitted at run time, so the expressions used to be interpreted there and built on
        // first use; the generated ones are plain compiled code on every runtime.
        [GeneratedRegex(TagsPattern, TagsOptions)]
        private static partial Regex Tags();

        [GeneratedRegex(WhitelistPattern, WhitelistOptions)]
        private static partial Regex Whitelist();

        [GeneratedRegex(WhitelistAPattern, WhitelistOptions)]
        private static partial Regex WhitelistA();

        [GeneratedRegex(WhitelistImgPattern, WhitelistOptions)]
        private static partial Regex WhitelistImg();
#else
        private static readonly Regex _tags = new Regex(TagsPattern, TagsOptions | RegexOptions.Compiled);
        private static readonly Regex _whitelist = new Regex(WhitelistPattern, WhitelistOptions | RegexOptions.Compiled);
        private static readonly Regex _whitelistA = new Regex(WhitelistAPattern, WhitelistOptions | RegexOptions.Compiled);
        private static readonly Regex _whitelistImg = new Regex(WhitelistImgPattern, WhitelistOptions | RegexOptions.Compiled);

        private static Regex Tags() => _tags;
        private static Regex Whitelist() => _whitelist;
        private static Regex WhitelistA() => _whitelistA;
        private static Regex WhitelistImg() => _whitelistImg;
#endif

        #region ISanitizer Members

        /// <summary>
        ///     sanitize any potentially dangerous tags from the provided raw HTML input using
        ///     a whitelist based approach, leaving the "safe" HTML tags
        ///     CODESNIPPET:4100A61A-1711-4366-B0B0-144D1179A937
        /// </summary>
        public string Sanitize(string html)
        {
            if (String.IsNullOrEmpty(html)) return html;

            // match every HTML tag in the input
            MatchCollection tags = Tags().Matches(html);
            for (int i = tags.Count - 1; i > -1; i--)
            {
                Match tag = tags[i];
                string tagname = tag.Value.ToLowerInvariant();

                if (!(Whitelist().IsMatch(tagname) || WhitelistA().IsMatch(tagname) || WhitelistImg().IsMatch(tagname)))
                {
                    html = html.Remove(tag.Index, tag.Length);
                    //System.Diagnostics.Debug.WriteLine("tag sanitized: " + tagname);
                }
            }

            return html;
        }

        #endregion
    }
}