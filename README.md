# 📙 Library Management System

A simple and user-friendly desktop application built with **WinUI 3** and **Entity Framework Core** to manage books, customers, loans, and reservations.

---

## 🔹 Description

This application helps you:
- Add, remove, and view books
- Track borrowed books and calculate late fees
- Manage customers (add/remove)
- Reserve books for customers
- Generate reports of borrowed books and the customers who have them

---

## 🛠️ Technologies Used

- ✅ **WinUI 3** – for creating modern Windows desktop UI
- ✅ **CommunityToolkit.MVVM** – for implementing the MVVM design pattern
- ✅ **Entity Framework Core** – for database access
- ✅ **SQL Server** – as the backend database

---

## 🗵 UI Features

- Clean layout with modern design (Borders, CornerRadius, consistent spacing)
- Real-time search for books by title or author
- Confirmation dialogs before delete operations
- Professional and organized DataGrid layouts

---

## 🔑 Project Structure
```
📁 Models         => Entity models: Book, Customer, Reservation
📁 ViewModels     => Business logic for each view
📁 Views          => Pages and controls for UI
📁 Services       => Lending, Reservation, and utility logic
📁 Data       => EF DbContext and configurations
```

---

## 🧪 How to Run the Project

1. Open the solution in **Visual Studio 2022+**
2. Make sure SQL Server is set up and the connection string is correct
3. Press **F5** to run the application
4. Manage your library like a pro 👨‍🏫📚

---

## ⚠️ Notes
- All models are Entity Framework-compatible
- Each ViewModel uses `ObservableObject` and `RelayCommand`
- No business logic is placed in Views (clean MVVM)

---

## 📸 Screenshots

- Book Management
  ![Screenshot 2025-04-07 002029](https://github.com/user-attachments/assets/746b3f60-2610-4768-a0c0-c1557229d1f3)

- Lending System
  ![Screenshot 2025-04-07 002130](https://github.com/user-attachments/assets/e6c1c4b6-95e5-48f6-93ee-a37562e92f17)

- Customer With Borrowed Books
  ![Screenshot 2025-04-07 002154](https://github.com/user-attachments/assets/9ee46fa6-ea76-497c-b832-cbf087a5bc53)

  

---

## ✍️ Author
- 👤 Moatsem Hussain
- 📧 motsememamhussain@gmail.com

---
