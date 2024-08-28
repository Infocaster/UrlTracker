using System;
using System.Collections.Generic;
using NUnit.Framework;
using UrlTracker.Core.Models;

namespace UrlTracker.Core.Tests.Domain.Models
{
    public class UrlTests
    {
        public static IEnumerable<TestCaseData> ParseTestCases()
        {
            yield return new TestCaseData
            (
                "example.com",
                null,
                "example.com",
                null,
                "/",
                null
            ).SetName("Plain domain name is parsed correctly");
            yield return new TestCaseData
            (
                "www.example.com",
                null,
                "www.example.com",
                null,
                "/",
                null
            ).SetName("Plain subdomain name is parsed correctly");
            yield return new TestCaseData
            (
                "example.com:8080",
                null,
                "example.com",
                8080,
                "/",
                null
            ).SetName("Plain domain with port is parsed correctly");
            yield return new TestCaseData
            (
                "https://www.example.com:8080",
                Protocol.Https,
                "www.example.com",
                8080,
                "/",
                null
            ).SetName("Absolute root url with port is parsed correctly");
            yield return new TestCaseData
            (
                "https://www.example.com:8080/en",
                Protocol.Https,
                "www.example.com",
                8080,
                "/en",
                null
            ).SetName("Absolute url with port and path is parsed correctly");
            yield return new TestCaseData
            (
                "example.com:8080/en",
                null,
                "example.com",
                8080,
                "/en",
                null
            ).SetName("Plain domain with port and path is parsed correctly");
            yield return new TestCaseData
            (
                "https://example.com/",
                Protocol.Https,
                "example.com",
                null,
                "/",
                null
            ).SetName("Absolute root url is parsed correctly");
            yield return new TestCaseData
            (
                "example.com/en",
                null,
                "example.com",
                null,
                "/en",
                null
            ).SetName("Plain domain with path is parsed correctly");
            yield return new TestCaseData
            (
                "/en",
                null,
                null,
                null,
                "/en",
                null
            ).SetName("Relative url is parsed correctly");
            yield return new TestCaseData
            (
                "example.com?lorem=ipsum&dolor=sit",
                null,
                "example.com",
                null,
                "/",
                "lorem=ipsum&dolor=sit"
            ).SetName("Url with query string is parsed correctly");
        }

        [TestCaseSource(nameof(ParseTestCases))]
        public void Parse_NormalFlow_ReturnsProperlyInterpretedUrl(string input, Protocol? expectedProtocol, string expectedHost, int? expectedPort, string expectedPath, string expectedQuery)
        {
            // arrange

            // act
            Url result = Url.Parse(input);

            // assert
            Assert.That(result.Protocol, Is.EqualTo(expectedProtocol));
            Assert.That(result.Path, Is.EqualTo(expectedPath));
            Assert.That(result.Host, Is.EqualTo(expectedHost));
            Assert.That(result.Port, Is.EqualTo(expectedPort));
            Assert.That(result.Query, Is.EqualTo(expectedQuery));
        }

        [TestCase(TestName = "Parse throws argument exception if parameter is null or whitespace")]
        public void Parse_ParameterWhiteSpace_ThrowsException()
        {
            // arrange

            // act
            static void result() => Url.Parse(string.Empty);

            // assert
            Assert.Throws<ArgumentException>(result);
        }

        public static IEnumerable<TestCaseData> FromAbsoluteUriTestCases()
        {
            yield return new TestCaseData
            (
                new Uri("https://www.example.com:8080"),
                Protocol.Https,
                "www.example.com",
                8080,
                "/",
                null
            ).SetName("Uri with port is converted correctly");
            yield return new TestCaseData
            (
                new Uri("https://www.example.com:8080/en"),
                Protocol.Https,
                "www.example.com",
                8080,
                "/en",
                null
            ).SetName("Uri with port and path is converted correctly");
            yield return new TestCaseData
            (
                new Uri("https://example.com"),
                Protocol.Https,
                "example.com",
                null,
                "/",
                null
            ).SetName("Root Uri is converted correctly");
            yield return new TestCaseData
            (
                new Uri("https://example.com/en"),
                Protocol.Https,
                "example.com",
                null,
                "/en",
                null
            ).SetName("Uri with path is converted correctly");
            yield return new TestCaseData
            (
                new Uri("https://example.com/?lorem=ipsum"),
                Protocol.Https,
                "example.com",
                null,
                "/",
                "lorem=ipsum"
            ).SetName("Uri with query string is converted correctly");
            yield return new TestCaseData
                (
                new Uri("ftp://example.com"),
                null,
                "example.com",
                null,
                "/",
                null
                ).SetName("Protocol is null if it cannot be parsed");
        }

        [TestCaseSource(nameof(FromAbsoluteUriTestCases))]
        public void FromAbsoluteUri_NormalFlowReturnsProperlyInterpretedUrl(Uri input, Protocol? expectedProtocol, string expectedHost, int? expectedPort, string expectedPath, string expectedQuery)
        {
            // arrange

            // act
            Url result = Url.FromAbsoluteUri(input);

            // assert
            Assert.That(result.Protocol, Is.EqualTo(expectedProtocol));
            Assert.That(result.Path, Is.EqualTo(expectedPath));
            Assert.That(result.Host, Is.EqualTo(expectedHost));
            Assert.That(result.Port, Is.EqualTo(expectedPort));
            Assert.That(result.Query, Is.EqualTo(expectedQuery));
        }

        [TestCase(TestName = "FromAbsoluteUri throws argument null exception if parameter is null")]
        public void FromAbsoluteUri_UriIsNull_ThrowsException()
        {
            // arrange

            // act
            static void result() => Url.FromAbsoluteUri(null!);

            // assert
            Assert.Throws<ArgumentNullException>(result);
        }

        [TestCase(TestName = "FromAbsoluteUri throws argument exception if uri is not absolute")]
        public void FromAbsoluteUri_UriIsRelative_ThrowsException()
        {
            // arrange
            Uri uri = new("/lorem", UriKind.Relative);

            // act
            void result() => Url.FromAbsoluteUri(uri);

            // assert
            Assert.Throws<ArgumentException>(result);
        }

        public static IEnumerable<TestCaseData> EqualsTestCases()
        {
            yield return new TestCaseData(
                Url.Parse("http://example.com:81/lorem?ipsum=dolor"),
                Url.Parse("http://example.com:81/lorem?ipsum=dolor"),
                true).SetName("Equals returns true if two urls are the exact same");
            yield return new TestCaseData(
                Url.Parse("http://example.com:81/lorem?ipsum=dolor"),
                Url.Parse("https://example.com:81/lorem?ipsum=dolor"),
                false).SetName("Equals returns false if the protocol is different");
            yield return new TestCaseData(
                Url.Parse("http://example.com:81/lorem?ipsum=dolor"),
                Url.Parse("http://urltracker.ic:81/lorem?ipsum=dolor"),
                false).SetName("Equals returns false if the host is different");
            yield return new TestCaseData(
                Url.Parse("http://example.com:81/lorem?ipsum=dolor"),
                Url.Parse("http://example.com:82/lorem?ipsum=dolor"),
                false).SetName("Equals returns false if the port is different");
            yield return new TestCaseData(
                Url.Parse("http://example.com:81/lorem?ipsum=dolor"),
                Url.Parse("http://example.com:81/sit?ipsum=dolor"),
                false).SetName("Equals returns false if the path is different");
            yield return new TestCaseData(
                Url.Parse("http://example.com:81/lorem?ipsum=dolor"),
                Url.Parse("http://example.com:81/lorem?ipsum=sit"),
                false).SetName("Equals returns false if the query string is different");
        }

        [TestCaseSource(nameof(EqualsTestCases))]
        public void Equals_NormalFlow_ReturnsEquality(Url left, Url right, bool expected)
        {
            // arrange

            // act
            var result = left.Equals(right);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }

        public static IEnumerable<TestCaseData> ToStringTestCases()
        {
            yield return new TestCaseData
            (
                "http://example.com"
            ).SetName("ToString can create absolute url strings");
            yield return new TestCaseData
            (
                "/lorem"
            ).SetName("ToString can create relative url strings");
            yield return new TestCaseData
            (
                "http://example.com:80"
            ).SetName("ToString can create absolute url strings with port number");
            yield return new TestCaseData
            (
                "http://example.com?lorem=ipsum"
            ).SetName("ToString can create url strings with query string");
        }

        [TestCaseSource(nameof(ToStringTestCases))]
        public void ToString_DefaultSettings_ReturnsUrlsWithDefaultSettings(string input)
        {
            // arrange
            Url url = Url.Parse(input);

            // act
            var result = url.ToString();

            // assert
            Assert.That(result, Is.EqualTo(input));
        }

        [TestCase(TestName = "ToString can exclude the query string")]
        public void ToString_ExcludeQuery_UrlHasNoQuery()
        {
            // arrange
            Url url = Url.Parse("http://example.com?lorem=ipsum");

            // act
            var result = url.ToString(UrlType.Absolute, excludeQuery: true);

            // assert
            Assert.That(result, Is.EqualTo("http://example.com"));
        }

        [TestCase(TestName = "ToString can add a trailing slash to the path")]
        public void ToString_EnsureTrailingSlash_HasTrailingSlash()
        {
            // arrange
            Url url = Url.Parse("http://example.com/lorem");

            // act
            var result = url.ToString(UrlType.Absolute, ensureTrailingSlash: true);

            // assert
            Assert.That(result, Is.EqualTo("http://example.com/lorem/"));
        }

        [TestCase(TestName = "ToString throws invalid operation exception if a url cannot be converted to the given url type")]
        public void ToString_RelativeUrlToAbsoluteUrlString_ThrowsException()
        {
            // arrange
            Url url = Url.Parse("/lorem");

            // act
            void result() => url.ToString(UrlType.Absolute);

            // assert
            Assert.Throws<ArgumentOutOfRangeException>(result);
        }
    }
}
