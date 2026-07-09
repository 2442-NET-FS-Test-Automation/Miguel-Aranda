# VideogameStore

A lightweight, high-performance E-Commerce backend built with **ASP.NET Core** and **Entity Framework Core** to manage video game store inventory, employee records, customer tracking, and order fulfillment.

---

## Project Structure

The solution is divided into two main layers:
* **`VideogameStore.Data`**: Class library containing entity models (`Sale`, `Employee`, `Videogame_Store`), `DbContext` configuration, Fluent API constraints, and database migrations.
* **`VideogameStore.Api`**: Minimal APIs project handling HTTP endpoints, request routing, dependency injection, and business services.

---

## Key Features

* **Direct & Lightweight Queries**: Optimized endpoints utilizing database-level filtering (`AsQueryable`) to handle high-performance searches by customer name, store, or employee.
* **Structured Logging**: Powered by **Serilog** to track application startup, requests, execution timings, and system warnings.
* **Advanced Order Fulfillment**: 
  * **Optimistic Concurrency**: Automated stock management handling high-traffic collision retries (`DbUpdateConcurrencyException`).
  * **Priority Queue Processing**: Custom deterministic sorting (`BurstPlanner`) favoring expedited shipping and chronological order processing.

---

## Technology Stack

* **Framework:** .NET 8 / ASP.NET Core (Minimal APIs)
* **ORM:** Entity Framework Core (SQL Server)
* **Logging:** Serilog
* **API Documentation:** Swagger / OpenAPI

---

## Getting Started

### Prerequisites
* .NET SDK (v8.0+)
* SQL Server instance installed and running

### 🔧 Database Setup
1. Open your terminal and navigate to your Data project directory.
2. Run the following command to apply the migrations and generate your local database schema:

   ```bash
   dotnet ef database update --project ../VideogameStore.Data
