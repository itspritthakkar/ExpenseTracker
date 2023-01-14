
ExpenseTracker
============
***An assignment made from end-user's perspective.***

[![.NET Foundation](https://img.shields.io/badge/.NET%20Foundation-blueviolet.svg)](https://www.dotnetfoundation.org/) [![MIT License](https://img.shields.io/github/license/dotnet/aspnetcore?color=%230b0&style=flat-square)](https://github.com/dotnet/aspnetcore/blob/main/LICENSE.txt) [![Help Wanted](https://img.shields.io/github/issues/dotnet/aspnetcore/help%20wanted?color=%232EA043&label=help%20wanted&style=flat-square)](https://github.com/dotnet/aspnetcore/issues?q=is%3Aissue+is%3Aopen+label%3A%22help+wanted%22) [![Good First Issues](https://img.shields.io/github/issues/dotnet/aspnetcore/good%20first%20issue?color=%23512BD4&label=good%20first%20issue&style=flat-square)](https://github.com/dotnet/aspnetcore/issues?q=is%3Aissue+is%3Aopen+label%3A%22good+first+issue%22) [![Discord](https://img.shields.io/discord/732297728826277939?style=flat-square&label=Discord&logo=discord&logoColor=white&color=7289DA)](https://aka.ms/dotnet-discord)

*I Prit Thakkar, an associate of Gateway Group bear full guarantee that each and every line of code has been written solely by myself (except libraries and scaffolding).*

ASP.NET Core is an open-source and cross-platform framework for building modern cloud-based internet-connected applications, such as web apps, IoT apps and mobile backends. ASP.NET Core apps run on [.NET](https://dot.net), a free, cross-platform and open-source application runtime./aspnet/core/).

## Features
 - 100% Responsive
 - Clean Code
 - Advanced Features
 - Easy-to-use
 - Futuristic Design

## Prerequisites

**Backend**
 - Microsoft.EntityFrameworkCore (v6.0.13)
 - Microsoft.EntityFrameworkCore.SqlServer (v6.0.13)
 - Microsoft.EntityFrameworkCore.Tools (v6.0.13)
 - Microsoft.VisualStudio.Web.CodeGeneration.Design (v6.0.11)
 - Swashbuckle.AspNetCore (v6.5.0)
 
 **Frontend**
 
 - [Ajax Datatables](https://datatables.net/manual/ajax) (Included)
 - [Sweetalert2](https://sweetalert2.github.io/) (Included)
 - [Apex Charts](https://apexcharts.com/) (Included)

## How to install?

**Backend**
 1. There are two folders, Frontend and DashboardAPI. Change working directory to DashboardAPI to setup backend.
 2. Migration has been made with default seed data for testing.
 3. Create database with name "ExpenseTracker" and update connection string.
 4. Run following to command to setup the database.


        Update-Database

**Frontend**

 1. All APIs are dynamically configured. Just change the domain name in Frontend/assets/js/globals.js (if domain name change of backend API is required). Example, to set the current domain for all APIs to "http://localhost:5236" :
 
        const  appUrl = "http://localhost:5236";

## Usage

As mentioned earlier, the project is made from end-user's perspective. It implements a concept of income and expense. *Any expense transaction mentioned, can never exceed total available balance (Total Income - Total Expense) or corresponding category limit.*

 1. Create a category by going in the categories tab. Income type categories(Ex. Salary, Freelance fees) have infinite limit.
 2. Once category is created. You can add transaction by going to transactions tab. *You can also add transactions from future date and it won't appear in the category calculations till the date passes. It helps notify you of upcoming expenses.*
 3. To better understand your spend, use the feature-rich dashboard to get real-time reports.

## Assignment Glimpse

**Dashboard**

![img](https://www.immortal.org.in/images/uploads/Screenshot1.png)

**Categories**

![img](https://www.immortal.org.in/images/uploads/Screenshot2.png)

**Transactions**

![img](https://www.immortal.org.in/images/uploads/Screenshot3.png)


## License

    Copyright [2023] [Prit Thakkar]

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.