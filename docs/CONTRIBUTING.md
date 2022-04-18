Contributing
=====

First of all: thank you very much for taking the time to make our products even more awesome with your contribution.

## How do I contribute?
There are several ways for you to make our products awesome, but everything starts by creating an issue:

### Create an issue
Creating an issue is the best way to bring our attention to a bug or a new feature. What catches our eye the most are issues with many votes, so before you make a new issue, always check if somebody else has already made a similar issue and vote and comment on that instead.

If there are no similar issues to yours, you can create a new issue. Issues are used to track feature requests and bugs, both having an appropriate template, so use these. If your issue does not fit in either template, you may want to start a discussion instead.

#### Why is this important?
An issue is the start of a conversation. We would like to give our opinion on your contribution before you dive into it. Many of our products have a particular philosophy or direction in some way and your idea may not be fit for the project. We don't want you to waste your time on a development which we already know won't make it into the product, so please allow us to check in with your idea before you get started.

When you find or create an issue and you want to solve it, it's time for you to make a pull request:

### Make a pull request
#### Branching strategy
Our products follow a branching strategy to keep our versioning organised. Our products are based on Umbraco and for each major umbraco version there is a separate `main`, `develop` and `contrib` branch. `main` branches contain the code of the active release. `develop` branches contain the code of the active prerelease. `contrib` is synchronised with `develop`, but is specifically meant for contributions.

#### Writing code
Follow these steps when writing code:
- Fork the project
- Check out the correct branch. The correct branch is the `contrib` branch of your major version of choice.
- Make sure you know [how to set up the URL Tracker for local development](/docs/setup.md).
- Create a new branch. A good branch name includes the major version, the id of the issue and optionally a descriptive name, for example: `v9/i-1234-improve-UI-on-mobile`
- Make your changes, experiment, have fun!
- Push your changes to your fork on github
- Create a pull request!