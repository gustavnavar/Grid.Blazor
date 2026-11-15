using GridShared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GridBlazor.Tests.Utility
{
    /// <summary>
    ///     The sanitizer keeps the tags on its whitelist and strips every other one, leaving the text
    ///     between them. The expressions behind it are built differently per target framework, so
    ///     these pin what it does rather than how.
    /// </summary>
    [TestClass]
    public class SanitizerTests
    {
        private static string Sanitize(string html)
        {
            return new Sanitizer().Sanitize(html);
        }

        [TestMethod]
        public void NothingToSanitizeIsReturnedAsIs()
        {
            Assert.IsNull(Sanitize(null));
            Assert.AreEqual("", Sanitize(""));
            Assert.AreEqual("plain text", Sanitize("plain text"));
        }

        [TestMethod]
        public void WhitelistedTagsAreKept()
        {
            const string html = "<b>bold</b> <em>em</em> <p>para</p><ul><li>item</li></ul><h2>title</h2><br/><hr>";
            Assert.AreEqual(html, Sanitize(html));
        }

        [TestMethod]
        public void TagsAreMatchedWhateverTheirCase()
        {
            const string html = "<B>bold</B><BR/>";
            Assert.AreEqual(html, Sanitize(html));
        }

        [TestMethod]
        public void OtherTagsAreStrippedAndTheirTextKept()
        {
            Assert.AreEqual("alert(1)x", Sanitize("<script>alert(1)</script>x"));
            Assert.AreEqual("text", Sanitize("<div class=\"c\">text</div>"));
            Assert.AreEqual("t</p>", Sanitize("<p onclick=\"x\">t</p>"));
        }

        [TestMethod]
        public void LinksAreKeptOnlyWithASafeHref()
        {
            const string link = "<a href=\"https://example.com/page?a=1\" title=\"Example\">link</a>";
            Assert.AreEqual(link, Sanitize(link));
            Assert.AreEqual("link</a>", Sanitize("<a href=\"javascript:alert(1)\">link</a>"));
        }

        [TestMethod]
        public void ImagesAreKeptOnlyWithASafeSource()
        {
            const string image = "<img src=\"https://example.com/a.png\" width=\"10\" height=\"20\" alt=\"A\">";
            Assert.AreEqual(image, Sanitize(image));
            Assert.AreEqual("", Sanitize("<img src=\"x\" onerror=\"alert(1)\">"));
        }

        [TestMethod]
        public void AnUnclosedTagAtTheEndIsStripped()
        {
            Assert.AreEqual("unclosed ", Sanitize("unclosed <b"));
        }
    }
}
