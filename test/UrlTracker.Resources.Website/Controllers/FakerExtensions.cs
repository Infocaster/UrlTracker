using Bogus;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Resources.Website.Controllers
{
    public static class FakerExtensions
    {
        public static EntityStrategy GenerateSourceStrategy(this Faker faker)
        {
            return faker.PickRandom(GenerateUrlRedirect, GenerateFileRedirect, GenerateRegexRedirect).Invoke(faker);
        }

        public static EntityStrategy GenerateTargetStrategy(this Faker faker)
        {
            return faker.PickRandom(GenerateUrlTarget, GenerateContentTarget, GenerateMediaTarget).Invoke(faker);
        }

        private static EntityStrategy GenerateUrlRedirect(Faker faker)
        {
            return new EntityStrategy(
                Core.Defaults.DatabaseSchema.RedirectSourceStrategies.Url,
                faker.Internet.UrlRootedPath());
        }

        private static EntityStrategy GenerateFileRedirect(Faker faker)
        {
            return new EntityStrategy(
                Core.Defaults.DatabaseSchema.RedirectSourceStrategies.Url,
                faker.Internet.UrlRootedPath(faker.PickRandom("jpg", "png", "css", "js", "docx", "pdf")));
        }

        private static EntityStrategy GenerateRegexRedirect(Faker faker)
        {
            return new EntityStrategy(
                Core.Defaults.DatabaseSchema.RedirectSourceStrategies.RegularExpression,
                faker.PickRandom("[a-zA-Z0-9]+", "lorem", @"\d{2,5}"));
        }

        private static EntityStrategy GenerateUrlTarget(Faker faker)
        {
            return new EntityStrategy(
                Core.Defaults.DatabaseSchema.RedirectTargetStrategies.Url,
                faker.Internet.UrlWithPath());
        }

        private static EntityStrategy GenerateContentTarget(Faker faker)
        {
            return new EntityStrategy(
                Core.Defaults.DatabaseSchema.RedirectTargetStrategies.Content,
                $"{faker.Random.Int(1000, 9999)};{faker.Random.RandomLocale()}");
        }

        private static EntityStrategy GenerateMediaTarget(Faker faker)
        {
            return new EntityStrategy(
                Core.Defaults.DatabaseSchema.RedirectTargetStrategies.Content,
                faker.Random.Int(1000, 9999).ToString());
        }
    }
}
