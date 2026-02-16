# PizzariaFalia

PizzariaFalia is a simple ASP.NET Core MVC web application for ordering pizza online. The goal of the project is to simulate a real restaurant ordering system where users can browse a menu, add items to a cart, and place orders. The system also includes a debug admin panel for managing the menu.

## Features
- Login/Register system implementing ASP.NET Identity
- An Interactive menu where users can choose what items to order, sort by categories and see details for specific dishes
- A Cart which keeps a collection of ordered items
- An Admin Debug Menu for CRUD Operations (Adding, Updating and Removing Dishes and categories)
- An Order screen to track/manage orders

## Technologies used

The project is built with ASP.NET Core MVC using C#. It uses Entity Framework Core for database access and ASP.NET Identity for authentication and role management. The frontend is created with Razor Views and Bootstrap.

## Setup Instructions

1. Clone the repository.
2. Open the solution in Visual Studio.
3. Change the database connection string inside `appsettings.json` to match your environment.
4. Apply the migrations using `Update-Database` in the nuget package manager console
5. Run the project from Visual Studio.

## Purpose

This project was created as a final module project for SoftUni, implementing the ASP.NET Framework and EF Core.

## Author

Nikolay Veselinov Stoyanov

GitHub: https://github.com/nstoyanov1007
