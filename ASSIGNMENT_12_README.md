# Assignment 12: Character Creation with Server-Side Validation

## Implementation Summary

This implementation demonstrates **why frontend validation is not enough** and shows proper server-side data cleaning and validation.

---

## 📁 Files Created/Updated

### Created Files:
- ✅ `ClassLibrary\Enums\CharacterClass.cs` - Enum for valid character classes
- ✅ `ClassLibrary\DTOs\CreateCharacterRequest.cs` - Request DTO (prevents overposting)
- ✅ `ClassLibrary\Entities\EmployeeEntity.cs` - Employee table entity
- ✅ `ClassLibrary\DTOs\EmployeeDTO.cs` - Employee response DTO
- ✅ `Back-EndAPI\Services\EmployeeService.cs` - Employee CRUD service
- ✅ `Back-EndAPI\Controllers\EmployeeController.cs` - Employee API endpoints

### Updated Files:
- ✅ `ClassLibrary\Entities\CharacterEntity.cs` - Added Gold, IsAdmin, IsDeleted
- ✅ `ClassLibrary\DTOs\CharacterDTO.cs` - Added Gold field
- ✅ `Back-EndAPI\Services\CharacterService.cs` - Added validation logic
- ✅ `Back-EndAPI\Controllers\CharacterController.cs` - Updated POST endpoint

---

## 🛡️ Validation Rules Enforced

### Name Validation
- ✅ **Required** - Cannot be null or empty
- ✅ **Trimmed** - Whitespace removed
- ✅ **Max Length** - 20 characters max
- ✅ **Alphanumeric Only** - Only letters and numbers allowed

### Class Validation (Enum)
- ✅ Must be one of: `Warrior`, `Mage`, `Rogue`, `Archer`
- ✅ Case insensitive (accepts "warrior", "WARRIOR", "Warrior")
- ✅ Invalid values return 400 with clear error

### Level Validation
- ✅ Minimum: 1
- ✅ Maximum: 50

### Gold Validation
- ✅ Minimum: 0
- ✅ Maximum: 10,000

### 🔒 Security: Overposting Protection
- ✅ Client **CANNOT** set `Id` (server generates)
- ✅ Client **CANNOT** set `IsAdmin` (always false)
- ✅ Client **CANNOT** set `IsDeleted` (always false)
- ✅ Client **CANNOT** set `CreatedAt` (server generates)
- ✅ Extra fields are **ignored** (no 500 error)

---

## 🧪 Test Cases for Swagger

### Test 1: Valid Request ✅
```json
{
  "name": " Artemis ",
  "class": "archer",
  "level": 5,
  "gold": 300
}
```
**Expected Result:** `201 Created`
```json
{
  "id": 1,
  "name": "Artemis",
  "class": "Archer",
  "level": 5,
  "health": 100,
  "mana": 100,
  "gold": 300
}
```

---

### Test 2: Trimmed Whitespace ✅
```json
{
  "name": "   ShadowKnight   ",
  "class": "warrior",
  "level": 10,
  "gold": 500
}
```
**Expected Result:** `201 Created` with `"name": "ShadowKnight"`

---

### Test 3: Invalid Enum ❌
```json
{
  "name": "Merlin",
  "class": "wizard",
  "level": 5,
  "gold": 100
}
```
**Expected Result:** `400 Bad Request`
```json
{
  "error": "Invalid class. Must be one of: Warrior, Mage, Rogue, Archer."
}
```

---

### Test 4: Negative Level ❌
```json
{
  "name": "BadChar",
  "class": "warrior",
  "level": -3,
  "gold": 100
}
```
**Expected Result:** `400 Bad Request`
```json
{
  "error": "Level must be between 1 and 50."
}
```

---

### Test 5: Negative Gold ❌
```json
{
  "name": "Thief",
  "class": "rogue",
  "level": 5,
  "gold": -10
}
```
**Expected Result:** `400 Bad Request`
```json
{
  "error": "Gold must be between 0 and 10,000."
}
```

---

### Test 6: Name Too Long ❌
```json
{
  "name": "SuperLongNameThatExceeds20Characters",
  "class": "mage",
  "level": 5,
  "gold": 100
}
```
**Expected Result:** `400 Bad Request`
```json
{
  "error": "Name cannot exceed 20 characters."
}
```

---

### Test 7: Empty Name (after trim) ❌
```json
{
  "name": "   ",
  "class": "warrior",
  "level": 5,
  "gold": 100
}
```
**Expected Result:** `400 Bad Request`
```json
{
  "error": "Name is required and cannot be empty."
}
```

---

### Test 8: Invalid Characters in Name ❌
```json
{
  "name": "Shadow@Knight!",
  "class": "warrior",
  "level": 5,
  "gold": 100
}
```
**Expected Result:** `400 Bad Request`
```json
{
  "error": "Name can only contain letters and numbers."
}
```

---

### Test 9: Overposting Attack Attempt 🔒
```json
{
  "name": "Hacker",
  "class": "warrior",
  "level": 5,
  "gold": 100,
  "isAdmin": true,
  "id": 999,
  "isDeleted": true
}
```
**Expected Result:** `201 Created` (extra fields ignored)
```json
{
  "id": 1,
  "name": "Hacker",
  "class": "Warrior",
  "level": 5,
  "health": 100,
  "mana": 100,
  "gold": 100
}
```
**Security Check:** `isAdmin` should be `false` in database, `id` should be auto-generated

---

### Test 10: Gold Over Limit ❌
```json
{
  "name": "RichKing",
  "class": "warrior",
  "level": 5,
  "gold": 9999999
}
```
**Expected Result:** `400 Bad Request`
```json
{
  "error": "Gold must be between 0 and 10,000."
}
```

---

### Test 11: Level Over Limit ❌
```json
{
  "name": "GodMode",
  "class": "warrior",
  "level": 999,
  "gold": 100
}
```
**Expected Result:** `400 Bad Request`
```json
{
  "error": "Level must be between 1 and 50."
}
```

---

### Test 12: Case Insensitive Class ✅
```json
{
  "name": "CaselessWarrior",
  "class": "WARRIOR",
  "level": 5,
  "gold": 100
}
```
**Expected Result:** `201 Created` with `"class": "Warrior"`

---

## 🚀 How to Test

1. **Run the API:**
   ```bash
   cd Back-EndAPI
   dotnet run
   ```

2. **Open Swagger UI:**
   - Navigate to: `http://localhost:5285`

3. **Test POST /api/characters:**
   - Click on `POST /api/characters`
   - Click "Try it out"
   - Paste test JSON from above
   - Click "Execute"
   - Check response status code and body

---

## 📝 Reflection Questions

### 1. Why is frontend validation not enough?

**Answer:**  
Frontend validation runs in the **client's browser**, which the user controls. Attackers can:
- Disable JavaScript
- Modify HTTP requests using tools like Postman or browser DevTools
- Send requests directly to the API bypassing the UI entirely
- Inject malicious payloads

**Server-side validation** is the **only trustworthy validation** because it runs in a controlled environment where the attacker has no access.

---

### 2. What would happen if a mobile app skipped validation?

**Answer:**  
A mobile app can be:
- **Reverse engineered** to extract API endpoints
- **Modified** using tools to bypass frontend checks
- **Replaced** entirely with a custom HTTP client that sends malicious requests

If the backend relies on the mobile app to validate, attackers could:
- Create accounts with admin privileges (`isAdmin: true`)
- Give themselves infinite gold (`gold: 999999999`)
- Crash the system with negative values or overflow attacks
- Inject SQL or XSS payloads through unvalidated names

**Backend must always validate**, regardless of whether the client is a web app, mobile app, or Postman.

---

### 3. What is overposting and why is it dangerous?

**Answer:**  
**Overposting** (also called **mass assignment**) occurs when a client sends **extra fields** in the request body that the API wasn't expecting, and those fields get saved directly to the database.

**Example Attack:**
```json
{
  "name": "Hacker",
  "class": "warrior",
  "level": 5,
  "gold": 100,
  "isAdmin": true,
  "salary": 999999
}
```

If the API directly binds this to the database entity without filtering, the attacker just:
- Gave themselves admin access
- Set their salary to $999,999

**Protection:**
- Use **separate request DTOs** (like `CreateCharacterRequest`) that only contain allowed fields
- **Never** bind directly from request to entity
- Use **guard clauses** to set security-sensitive fields server-side

---

### 4. Where should business rules live and why?

**Answer:**  
Business rules should live in the **Service Layer** (not controllers), because:

**Why NOT in Controllers?**
- Controllers should be **thin** - only handle HTTP concerns (status codes, routing)
- Controllers are hard to test in isolation
- Logic in controllers cannot be reused across multiple endpoints

**Why in Services?**
- **Reusability** - Multiple controllers can use the same validation logic
- **Testability** - Services can be unit tested without HTTP context
- **Separation of Concerns** - Business logic stays separate from HTTP infrastructure
- **Maintainability** - One place to update when rules change

**Example:**
- ❌ **Bad:** Controller checks if level is 1-50
- ✅ **Good:** Service checks level, controller just calls service and translates result to HTTP response

---

## 🎓 Assignment Deliverables Checklist

- ✅ POST /api/characters endpoint created
- ✅ Name validation (required, trimmed, max 20, alphanumeric)
- ✅ Class validation (enum, case insensitive)
- ✅ Level validation (1-50)
- ✅ Gold validation (0-10,000)
- ✅ Overposting protection (Id, IsAdmin, IsDeleted cannot be set by client)
- ✅ Returns 400 with clear error messages for invalid data
- ✅ Returns 201 with normalized data for valid requests
- ✅ No 500 errors for validation failures
- ✅ Service layer contains validation logic (not controller)
- ✅ Guard clauses used for validation
- ✅ Separate request DTO from response DTO

---

## 📊 Database Schema Update Needed

You'll need to add these columns to your `character` table:

```sql
ALTER TABLE character ADD COLUMN IF NOT EXISTS gold INTEGER NOT NULL DEFAULT 0;
ALTER TABLE character ADD COLUMN IF NOT EXISTS is_admin BOOLEAN NOT NULL DEFAULT FALSE;
ALTER TABLE character ADD COLUMN IF NOT EXISTS is_deleted BOOLEAN NOT NULL DEFAULT FALSE;
```

---

## 📦 Bonus: Employee CRUD Fully Implemented

As a bonus, I also created a complete CRUD implementation for the **Employee** table:

**Endpoints:**
- GET /api/employees
- GET /api/employees/{id}
- POST /api/employees
- PUT /api/employees/{id}
- DELETE /api/employees/{id}

Test both in Swagger! 🎉
