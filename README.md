# 🏖️ Leave Management System – ASP.NET Core MVC

This is a fully functional Leave Management System built with **ASP.NET Core MVC**, **Entity Framework Core**, and **ASP.NET Core Identity**. It allows organizations to manage employee leave requests, approvals, and allocations efficiently.

---

## ✨ Features

✅ User Registration and Role-based Authentication (Admin, Employee, Supervisor)  
✅ Email Confirmation with Papercut SMTP (easy to test locally)  
✅ Leave Type and Leave Period Management  
✅ Submit, Cancel, and Approve/Reject Leave Requests  
✅ Dynamic Leave Allocation Logic  
✅ Admin Panel with Seeding (Admin user, LeaveTypes, Periods)  
✅ Responsive UI with Bootstrap Sidebar Navigation  
✅ Clean Architecture: Controllers, Services, AutoMapper, ViewModels  
✅ Entity Configuration using `IEntityTypeConfiguration<T>`

---

## 🚀 Getting Started

### 🛠 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- SQL Server Express
- [Papercut SMTP](https://github.com/ChangemakerStudios/Papercut-SMTP) (for local email testing)
- Visual Studio 2022 (recommended)

---

## 📦 Setup Instructions

### 1. Clone the repository

```bash
git clone https://github.com/tomislavboshkovski/leave-management-system.git
cd leave-management-system
```

---

### 2. Configure the database

Open `appsettings.json` and update the connection string to point to your local SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=LeaveManagementSystem;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

---

### 3. Run Migrations and Seed Data

Open the terminal in the project root and run:

```bash
dotnet ef database update
```

This will apply all migrations and seed:

- Admin user
- Leave Types
- Current Period

---

### 4. Run the App

Launch the app from Visual Studio **or** using:

```bash
dotnet run
```

Visit: [https://localhost:7188](https://localhost:7188)

---

## 👤 Default Admin Login

**Email:** `admin@localhost.com`  
**Password:** `P@ssword1`

---

## 🌐 Email Confirmation

This app uses **Papercut SMTP** to simulate email confirmation during development.  
Just run Papercut and emails will appear instantly in the desktop client.

---

## 💡 Potential Improvements

- Integrate [SendGrid](https://sendgrid.com/) for real email confirmation in production
- Implement Notification System
- Implement Supervisor Dashboard

---

## 📜 License

This project is for educational and portfolio purposes.

---

Created with ❤️ by **Tomislav**
