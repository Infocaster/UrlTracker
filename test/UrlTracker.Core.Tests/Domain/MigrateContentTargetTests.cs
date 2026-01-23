using System;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Tests.Domain;

public class MigrateContentTargetTests
{
    private Mock<IIdKeyMap> KeyValueMock;
    private readonly string GuidString;

    public MigrateContentTargetTests()
    {
        GuidString = "00000000-0000-0000-0000-000000001234";
        KeyValueMock = new Mock<IIdKeyMap>();
        KeyValueMock.Setup(kvs => kvs.GetKeyForId(1234, It.IsAny<UmbracoObjectTypes>()))
            .Returns(Attempt.Succeed(Guid.Parse(GuidString)));
        KeyValueMock.Setup(kvs => kvs.GetKeyForId(5678, It.IsAny<UmbracoObjectTypes>()))
            .Returns(Attempt.Fail<Guid>());
    }

    // Happy flow, content redirect met int id wordt vertaalt naar GUID.
    [TestCase(TestName = "content redirect returns target value with GUID")]
    public void MigrateContentTarget_ContentRedirectWithIntId_ReturnsGuidTarget()
    {
        // given
        var redirect = CreateRedirect(EntityStrategy.ContentTarget("1234"));

        // when 
        var result = redirect.MigrateContentTarget(KeyValueMock.Object);

        // then
        Assert.That(result.Target.Value, Is.EqualTo(GuidString));
    }

    // Happy flow, met culture
    [TestCase(TestName = "content redirect with culture returns target value with GUID and culture")]
    public void MigrateContentTarget_ContentRedirectWithIntIdAndCulture_ReturnsGuidTargetWithCulture()
    {
        // given
        var redirect = CreateRedirect(EntityStrategy.ContentTarget("1234;nl-NL"));
        // when 
        var result = redirect.MigrateContentTarget(KeyValueMock.Object);
        // then
        Assert.That(result.Target.Value, Is.EqualTo($"{GuidString};nl-NL"));
    }

    // Redirect is geen content redirect, geen wijziging. obv strategy.
    [TestCase(TestName = "redirect is not a content redirect")]
    public void MigrateContentTarget_NotAContentRedirect_NoChange()
    {
        // given
        var target = EntityStrategy.UrlTarget("/dolarsit");
        var redirect = CreateRedirect(target);

        // when 
        var result = redirect.MigrateContentTarget(KeyValueMock.Object);

        // then
        Assert.That(result.Target.Value, Is.SameAs(target.Value));
    }


    // Redirect is content redirect, maar is al een guid.
    [TestCase("00000000-0000-0000-0000-000000001234", TestName = "content redirect with guid target no change")]
    [TestCase("00000000-0000-0000-0000-000000001234;nl-NL", TestName = "content redirect with guid target and culture, no change")]
    public void MigrateContentTarget_ContentRedirectWithGuidTarget_NoChange(string targetValue)
    {
        // given
        var target = EntityStrategy.ContentTarget(targetValue);
        var redirect = CreateRedirect(target);
        // when 
        var result = redirect.MigrateContentTarget(KeyValueMock.Object);
        // then
        Assert.That(result.Target.Value, Is.SameAs(target.Value));
    }

    // Content bestaat niet meer. 
    [TestCase(TestName = "content redirect with non existing content, no change")]
    public void MigrateContentTarget_ContentRedirectWithNonExistingContent_NoChange()
    {
        // given
        var target = EntityStrategy.ContentTarget("5678");
        var redirect = CreateRedirect(target);
        // when 
        var result = redirect.MigrateContentTarget(KeyValueMock.Object);
        // then
        Assert.That(result.Target.Value, Is.SameAs(target.Value));
    }

    private static RedirectEntity CreateRedirect(EntityStrategy target)
    {
        return new RedirectEntity(default, default, default, default, EntityStrategy.UrlSource("/loremipsum"), target);
    }
}
