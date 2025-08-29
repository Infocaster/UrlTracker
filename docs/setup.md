# How to set up your development environment

## System requirements
To work on the URL Tracker, you need to have the following:
 - You must have SQL Server installed. Developer edition is sufficient. Sql Server Management Studio is recommended to have as well.
 - An IDE for ASP.NET 9.0 applications. Visual studio 2022 and Visual Studio code work.

## Steps to get started
 1. Create a new database in your SQL Server. Check the connection string in web.config in UrlTracker.Resources.Website to see what username, password and database name you need to use. NOTE: Be careful with permissions. Make sure that the user and login that you create only have access to the URL Tracker database. The user needs to be able to create and alter tables.
 1. Open the solution in your IDE. Clean & Rebuild your solution.
 1. Configure your secrets on UrlTracker.Resources.Website. There is an example json inside that project. Make sure all those secrets are configured.
 1. Build the project and run
 1. Go to the backoffice and make sure you're automatically logged in
 1. Run all the automated tests once to make sure that your website is fully running.
 1. Now you should be ready to work on the URL Tracker.