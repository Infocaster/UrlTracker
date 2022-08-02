# How to set up your development environment

## System requirements
To work on the URL Tracker, you need to have the following:
 - You must have SQL Server installed. Developer edition is sufficient. Sql Server Management Studio is recommended to have as well.
 - IIS must be available and enabled.
 - An IDE for ASP.NET Framework applications. Visual studio 2019 or 2022 work.
 - Node 16.x

## Steps to get started
 1. Create a new website in IIS. Use `urltracker.ic` for the domain. Http is enough, you won't need https enabled. You may need to update your host file to point this domain to localhost (127.0.0.1).
 2. Create a new database in your SQL Server. Check the connection string in web.config in UrlTracker.Resources.Website to see what username, password and database name you need to use. NOTE: Be careful with permissions. Make sure that the user and login that you create only have access to the URL Tracker database. The user needs to be able to create and alter tables.
 3. Open a command window inside `src/UrlTracker.Frontend`
 4. Run the following commands:
    ```batch
    npm i
    npm run build:webpack
    ```
 4. Open the solution in your IDE. Clean & Rebuild your solution.
 5. Open your browser and check that the website can be reached on `http://urltracker.ic/`. Go through the installer if it pops up. You won't need the starter content if it asks for it.
 6. Log into the backoffice, go to settings, uSync and perform an import to import the required content into the website.
 7. Run all the automated tests once to make sure that your website is fully running.
 8. Now you should be ready to work on the URL Tracker.

## Working on C#
To work on the C#, you can make your changes in visual studio and simply build the project

## Working on JavaScript / TypeScript
All typescript is written inside the `src/UrlTracker.Frontend` folder. Whenever you make any changes to the frontend, run `npm run build:webpack` to apply your changes. After running the command, you can try out your changes in the browser.