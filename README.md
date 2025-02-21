# Transaction Authorizer

## 📌 Overview
Transaction Authorizer is a C# API that processes banking transactions and checks balance availability for different transaction categories.

### 🔹 Features:
- **Process Transactions:** Validates and categorizes transactions based on MCC and balance.
- **Balance Management:** Ensures transactions are processed only when sufficient funds are available.
- **Category Correction:** Adjusts incorrect MCC codes before processing.

## 💡 Why Use This Approach?
This project uses dependency injection and a clean service layer to separate concerns.

### ✅ **Advantages:**
- **Encapsulation:** The `BalanceService` handles balance operations independently.
- **Testability:** The API structure allows easy unit testing.
- **Extensibility:** The `TransactionCategoryService` makes it simple to add new transaction categories.

---

## 🛠️ Future Improvements
- **Database Integration:** Store accounts and transactions persistently.
- **Logging & Monitoring:** Add better error handling and logging mechanisms.
- **Security Features:** Implement authentication for transaction requests.

---

## 🧩 Unit Testing
Unit tests ensure reliable transaction processing. The key test cases include:
1. **Transaction Invalid → Returns `07`**
2. **Transaction Approved (Sufficient Balance) → Returns `00`**
3. **Transaction Declined (Insufficient Balance) → Returns `51`**
   
## 🛠️ Handling Simultaneous Transactions
Another approach to ensuring that an account does not have more than one transaction being processed simultaneously is to implement control directly at the database level. The execution flow can follow these steps:

Mark balance as "PENDING": Before processing the transaction, the system marks the account balance as "PENDING" in the database.
Block new transactions: While the balance is marked as "PENDING," new transactions for that account are either blocked or added to a queue.
Finalize the transaction: After the transaction is completed, the account balance is updated, and the "PENDING" status is removed.
This process eliminates inconsistencies related to balance management and prevents concurrency issues with simultaneous transactions. To optimize execution time, database locks can be used to ensure that only one transaction is processed at a time without compromising performance.

Run tests with:
```sh
dotnet test
```

## 🚀 Getting Started
### 🔹 Clone the Repository:
```sh
git clone [repository-url]
cd TransactionAuthorizer
```

### 🔹 Build & Run:
```sh
dotnet build
dotnet run
```

