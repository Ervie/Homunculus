using FluentAssertions;
using MarekMotykaBot.Services.Core;
using Xunit;

namespace MarekMotykaBot.Tests.Services
{
    public class EpisodeTitleParserTests
    {
        // "Name - NN" variant (the most common SubsPlease/Erai-raws layout).
        [Theory]
        [InlineData(
            "[SubsPlease] Kamiina Botan, Yoeru Sugata wa Yuri no Hana - 03 (1080p) [FDC0E8ED].mkv",
            "Kamiina Botan, Yoeru Sugata wa Yuri no Hana", 3)]
        [InlineData(
            "[ASW] Kamiina Botan, Yoeru Sugata wa Yuri no Hana - 03 [1080p HEVC x265 10Bit][AAC]",
            "Kamiina Botan, Yoeru Sugata wa Yuri no Hana", 3)]
        [InlineData(
            "[Erai-raws] Hokuto no Ken (2026) - 05 [1080p AMZN WEBRip HEVC EAC3][MultiSub][8E8F55CF]",
            "Hokuto no Ken", 5)]
        [InlineData(
            "[Erai-raws] Tensei Shitara Slime Datta Ken 4th Season - 04 [1080p CR WEBRip HEVC AAC][MultiSub][CB53801D]",
            "Tensei Shitara Slime Datta Ken 4th Season", 4)]
        public void Parse_PlainDashEpisode_ShouldExtractNameAndEpisodeWithoutSeason(
            string title, string expectedName, int expectedEpisode)
        {
            var result = EpisodeTitleParser.Parse(title);

            result.Should().NotBeNull();
            result.Name.Should().Be(expectedName);
            result.Season.Should().BeNull();
            result.Episode.Should().Be(expectedEpisode);
        }

        // "Name Sx - NN" variant.
        [Theory]
        [InlineData("Sousou no Frieren S2 - 09", "Sousou no Frieren", 2, 9)]
        [InlineData(
            "[ASW] Tensei Shitara Slime Datta Ken S4 - 04 [1080p HEVC x265 10Bit][AAC]",
            "Tensei Shitara Slime Datta Ken", 4, 4)]
        [InlineData(
            "[ASW] Otonari no Tenshi-sama ni Itsunomanika Dame Ningen ni Sareteita Ken S2 - 04 [1080p HEVC x265 10Bit][AAC]",
            "Otonari no Tenshi-sama ni Itsunomanika Dame Ningen ni Sareteita Ken", 2, 4)]
        public void Parse_SeasonThenDashEpisode_ShouldExtractNameSeasonAndEpisode(
            string title, string expectedName, int expectedSeason, int expectedEpisode)
        {
            var result = EpisodeTitleParser.Parse(title);

            result.Should().NotBeNull();
            result.Name.Should().Be(expectedName);
            result.Season.Should().Be(expectedSeason);
            result.Episode.Should().Be(expectedEpisode);
        }

        // "Name - SxxExx" / "Name SxxExx" variants (optionally with alt title in parens).
        [Theory]
        [InlineData(
            "[Judas] Witch Hat Atelier (Tongari Boushi no Atelier) - S01E01",
            "Witch Hat Atelier", 1, 1)]
        [InlineData(
            "[Judas] Tensei Shitara Slime Datta Ken (That Time I Got Reincarnated as a Slime) - S04E04 [1080p][HEVC x265 10bit][Multi-Subs] (Weekly)",
            "Tensei Shitara Slime Datta Ken", 4, 4)]
        [InlineData(
            "[DKB] Otonari no Tenshi-sama ni Itsunomanika Dame Ningen ni Sareteita Ken - S02E04 [1080p][HEVC x265 10bit][Multi-Subs][weekly]",
            "Otonari no Tenshi-sama ni Itsunomanika Dame Ningen ni Sareteita Ken", 2, 4)]
        [InlineData(
            "[Sokudo] NIPPON SANGOKU The Three Nations of the Crimson Sun S01E03 [1080p WEB-DL AV1][Dual Audio] (weekly)",
            "NIPPON SANGOKU The Three Nations of the Crimson Sun", 1, 3)]
        [InlineData(
            "[Yameii] The Drops of God - S01E01 [English Dub] [CR WEB-DL 1080p H264 AAC] [17177A24] (Kami no Shizuku)",
            "The Drops of God", 1, 1)]
        [InlineData(
            "Fist of the North Star HOKUTO NO KEN S01E05 Bloody Cross 1080p AMZN WEB-DL MULTi DDP2.0 H 264-VARYG (Hokuto no Ken: FIST OF THE NORTH STAR, Multi-Audio, Multi-Subs)",
            "Fist of the North Star HOKUTO NO KEN", 1, 5)]
        public void Parse_SeasonEpisodeCombined_ShouldExtractNameSeasonAndEpisode(
            string title, string expectedName, int expectedSeason, int expectedEpisode)
        {
            var result = EpisodeTitleParser.Parse(title);

            result.Should().NotBeNull();
            result.Name.Should().Be(expectedName);
            result.Season.Should().Be(expectedSeason);
            result.Episode.Should().Be(expectedEpisode);
        }

        [Theory]
        [InlineData("[Serenae] Meitantei Precure! - 1-12 Batch (1080p)")]
        [InlineData("[Serenae] Meitantei Precure! - 1-12 Batch (720p)")]
        [InlineData("[A&C] Fairy Tail 001-024 (BD HEVC 1080p) [Multi-Audio-Subs]")]
        public void Parse_BatchRelease_ShouldReturnNull(string title)
        {
            var result = EpisodeTitleParser.Parse(title);

            result.Should().BeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("[SubsPlease] just some garbage without episode info")]
        public void Parse_InvalidInput_ShouldReturnNull(string title)
        {
            var result = EpisodeTitleParser.Parse(title);

            result.Should().BeNull();
        }

        [Theory]
        [InlineData(
            "Sousou no Frieren S2 - 09",
            "Sousou no Frieren s2ep9")]
        [InlineData(
            "[Judas] Witch Hat Atelier (Tongari Boushi no Atelier) - S01E01",
            "Witch Hat Atelier s1ep1")]
        [InlineData(
            "[SubsPlease] Kamiina Botan, Yoeru Sugata wa Yuri no Hana - 03 (1080p) [FDC0E8ED].mkv",
            "Kamiina Botan, Yoeru Sugata wa Yuri no Hana ep3")]
        public void FormatEpisodeName_ShouldProduceCanonicalString(string title, string expected)
        {
            var result = EpisodeTitleParser.FormatEpisodeName(title);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("Sousou no Frieren S2 - 09", "s2ep9")]
        [InlineData("[Judas] Witch Hat Atelier (Tongari Boushi no Atelier) - S01E01", "s1ep1")]
        [InlineData("[ASW] Snowball Earth - 04 [1080p HEVC x265 10Bit][AAC]", "ep4")]
        public void FormatEpisodeSuffix_ShouldProduceSuffixOnly(string title, string expected)
        {
            var result = EpisodeTitleParser.FormatEpisodeSuffix(title);

            result.Should().Be(expected);
        }

        [Fact]
        public void Parse_VersionedEpisode_ShouldStripVersionSuffix()
        {
            var result = EpisodeTitleParser.Parse("[ASW] Some Show - 05v2 [1080p]");

            result.Should().NotBeNull();
            result.Name.Should().Be("Some Show");
            result.Episode.Should().Be(5);
        }
    }
}
