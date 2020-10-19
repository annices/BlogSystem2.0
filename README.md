# Table of contents
<details>
   <summary>Click here to expand content list.</summary>
   
1. [General information](#1-general-information)
2. [License](#2-license)
3. [System description](#3-system-description)
4. [System requirements](#4-system-requirements)
5. [Changes and added features to version 2.0](#5-changes-and-added-features-to-version-20)
6. [ER diagram](#6-er-diagram)
    * [6.1 ER rules](#61-er-rules)
7. [Sequence diagram](#7-sequence-diagram)
8. [Sitemaps](#8-sitemaps)
    * [8.1 Sitemap for anonymous users](#81-sitemap-for-anonymous-users)
    * [8.2 Sitemap for the admin user](#82-sitemap-for-the-admin-user)
9. [User interface](#9-user-interface)
10. [Setup guide](#10-setup-guide)
    * [10.1 Prerequisites](#101-prerequisites)
    * [10.2 Create the database and its tables](#102-create-the-database-and-its-tables)
    * [10.3 Configure the application](#103-configure-the-application)
    * [10.4 NuGet packages](#104-nuget-packages)
    * [10.5 Run and test the application](#105-run-and-test-the-application)
11. [Contact details](#11-contact-details)
</details>

---

# 1 General information
"Blog System 2.0" was created in Visual Studio Community by Annice Strömberg, 2020, with [Annice.se](https://annice.se) as the primary download location. In this script, an admin can create, edit and delete blog entries, entry categories and entry comments. The admin can also reply to comments posted by anonymous users. Furthermore, the admin can edit the admin details. Moreover, the script supports a password recovery function to reset the admin password via a token link sent to the admin email.

---

# 2 License
Released under the MIT license.

MIT: [http://rem.mit-license.org](http://rem.mit-license.org/), see [LICENSE](LICENSE).

---

# 3 System description
"Blog System 2.0" is built in the front-end languages: CSS (customized, and with Bootstrap 4.3.1), HTML5, JavaScript (classic, and with Ajax, jQuery3.3.1). Moreover, the back-end code is build in C# 8.0 based on ASP.NET Core 3.1 using the MVC (Model-View-Controller) design pattern. In addition, the database interaction is based on Entity Framework Core 3.1 – database first – using a relational database built in Transact SQL with SQL Server as a DBMS (DataBase Management System).

---

# 4 System requirements
The script can be run on servers that support C# 8.0 and ASP.NET Core 3.1 along with an SQL Server supported database.

---

# 5 Changes and added features to version 2.0
The following functions and features have been implemented to Blog System 2.0:
  * Modified database model to support several categories per entry, along with added ability to save blog posts as drafts or published entries.
  * Upgraded frameworks to support cross-platform web development to:
    * ASP.NET Core 3.1
    * Entity Framework Core 3.1.8.
  * Upgraded application server side code to C# 8.0.
  * Modified user password algorithm to HMAC-256 encryption based on ASP.NET Core Identity.
  * Modified password recovery function to reset the user password via a JSON Web Token link.
  * Modified authentication process to be handled via a middleware authentication server.
  * Upgraded CSS library to Bootstrap 4.3.1 with modified layout.
  * Added Bootstrap icon library 1.0.0.
  * Upgraded JavaScript library to jQuery 3.3.1.
  * Upgraded WYSIWYG HTML entry editor to TinyMCE 5.4.2.
  * Added syntax highlighter feature using Prism 1.21.0 when embedding code snippets in entries.
  * Modified sort, filter and pagination function of entries, comments and categories using Grid MVC 6.2.0.
  * Added pagination to entry comments.
  * Added multi-select option to support multiple delete of entries, comments and categories.
  
---
  
# 6 ER diagram
The following diagram illustrates the database table relationships reflecting the entity relationships, and the table attributes (columns) reflecting the entity properties used by this script.

<img src="https://diagrams.annice.se/c-sharp-blog-system-2.0/ERDiagram.png" alt="Sitemap for admin pages." width="700">

## 6.1 ER rules
  * A blog entry can have many categories, and one category can be linked to many entries.
  * A blog entry can have many comments, but one comment is linked to one entry.
  * A blog entry can be created and edited by one admin user, but the admin user can create and edit many entries.
  
---

# 7 Sequence diagram
The diagram below illustrates an example of a high level context flow when an admin user creates a blog post.

<img src="https://diagrams.annice.se/c-sharp-blog-system-2.0/CreateEntrySequenceDiagram.png" alt="Sequence flow to create an entry." width="750">

---

# 8 Sitemaps
In this section you can see an overview of the admin vs. anonymous page hierarchy in the application.

## 8.1 Sitemap for anonymous users
<img src="https://diagrams.annice.se/c-sharp-blog-system-2.0/PublicSitemap.png" alt="Sitemap for public pages." width="400">

## 8.2 Sitemap for the admin user
<img src="https://diagrams.annice.se/c-sharp-blog-system-2.0/AdminSitemap.png" alt="Sitemap for admin pages." width="600">

---

# 9 User interface
Screenshot of the application start page with the latest posted entries:

<img src="https://diagrams.annice.se/c-sharp-blog-system-2.0/GUI_StartPage.png" alt="" width="650">

Screenshot of an entry details page displayed for anonymous users:

<img src="https://diagrams.annice.se/c-sharp-blog-system-2.0/GUI_EntryDetails.png" alt="" width="650">

Screenshot of the comment section on an entry details page:

<img src="https://diagrams.annice.se/c-sharp-blog-system-2.0/GUI_Comments.png" alt="" width="650">

Screenshot of the admin view to create a new blog entry:

<img src="https://diagrams.annice.se/c-sharp-blog-system-2.0/GUI_CreateEntry.png" alt="" width="650">

Screenshot of the admin view to edit a blog entry:

<img src="https://diagrams.annice.se/c-sharp-blog-system-2.0/GUI_EditEntry.png" alt="" width="650">

Screenshot of the entry edit list with filter options displayed for the admin:

<img src="https://diagrams.annice.se/c-sharp-blog-system-2.0/GUI_EditEntryList.png" alt="" width="650">

---

# 10 Setup guide
As this script was created in Visual Studio Community with SQL Server, I will go through the necessary setup steps accordingly (all softwares used for this application setup are free).

## 10.1 Prerequisites
  * [Install SQL Server Express](https://www.microsoft.com/sv-se/sql-server/sql-server-downloads)
  * [Install SQL Server Management Studio (SSMS)](https://docs.microsoft.com/sv-se/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15)
  * [Install .NET Core 3.1 (SDK)](https://dotnet.microsoft.com/download)
  * [Install Visual Studio Community](https://visualstudio.microsoft.com/vs/community/)
  
## 10.2 Create the database and its tables
  1. Navigate to the unzipped script folder “BlogSystem2.0”. Open the file “sql_blogsystem.sql”, look up the code section below and change the highlighted values to suit your own settings. (Note! The default password is set to “admin”, but can be changed after your first login):

```sql
INSERT INTO BlogSystem.dbo.BS_Users 
VALUES 
  (
    'YourUserName',
    'YourFirstName', -- Optional.
    'YourLastName', -- Optional.
    'your@email.com', 
    -- Keep the hashed password below until your first login. Default password is set to
    -- "admin", but can be changed under the admin panel once you're logged in:
   'AQAAAAEAACcQAAAAEBehHmgEHZmjXlTBGlKSW9KVuxMIHp1f4r8sC502SFQkGGxiYeef6HFntNMCMdZ76w=='
)

```

2. Once you have updated the SQL code in “sql_blogsystem.sql”, then open and execute the SQL file/code in SQL Server Management Studio to create the BlogSystem database with its tables.

## 10.3 Configure the application
  3. When the database and tables are created, you can open the application in Visual Studio by double clicking the solution file “BlogSystem.sln” under the unzipped project folder path: “BlogSystem2.0 > BlogSystem > BlogSystem.sln”.

  4. In Visual Studio, you can then change the commented values below to suit your own settings in the appsettings.json file found in the Solution Explorer window:
  
```json
{
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DBConnect": "Server=.\\SQLEXPRESS;Database=BlogSystem;Trusted_Connection=True;MultipleActiveResultSets=true" // You can keep this AS-IS unless you use a DB user and DB password.
  },
  "Jwt": {
    "Key": "abc123^&%&^&%321", // Name the key with a minium length of 16 characters.
    "Issuer": "BlogSystem2.0", // The token issuer.
    "Audience": "Admin" // The token audience.
  },
  "EmailSettings": {
    "From": "your@gmail.com", // Specify an email through which emails will be sent.
    "Password": "YourMailPassword", // Specify the email password.
    "Host": "smtp.gmail.com", // You can keep this if you use gmail.
    "Gmail": "587" // You can keep this if you use gmail.
  }
}
```

## 10.4 NuGet packages
  6. Also, ensure you have the following NuGet packages installed for the solution, otherwise [install](https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-powershell) them:
  
    * Microsoft.AspNetCore.Authentication.JwtBearer (3.1.8)
    * Microsoft.EntityFrameworkCore.SqlServer (3.1.8)
    * Microsoft.EntityFrameworkCore.Tools (3.1.8)
    * Microsoft.VisualStudio.Web.CodeGeneration.Design (3.1.4)
    * NonFactors.Grid.Mvc6 (6.2.0)
    * ReflectionIT.Mvc.Pagin (4.0.0)

## 10.5 Run and test the application
  7. Select to run the application via the Visual Studio play button in the top menu bar.

  8. On your first login, use the password "**admin**" along with the user email you specified when you executed the SQL code (see section 10.2).

# 11 Contact details
For general feedback related to this script, such as any discovered bugs etc., you can contact me via the following email address: [info@annice.se](mailto:info@annice.se)
