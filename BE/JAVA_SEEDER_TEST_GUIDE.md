# Java Exercises Seeder - Testing Guide

## Overview

A comprehensive seeder has been added to `Learn2CodeDbContextSeeder.cs` to test the **Submit Code** and **Run Code** flows for Java exercises with the main function approach.

**Seeder Method:** `SeedJavaExercisesForTestingAsync()`

---

## Test Data Structure

### Course & Environment
- **Course:** "Java Fundamentals - Testing"
- **Section:** "Java Code Testing Section" 
- **Category:** Programming Languages
- **Language:** Java

### 5 Comprehensive Exercises

---

## Exercise 1: Sum Two Numbers ✅

**Purpose:** Test function-based exercise with `DefaultMainCode` and `MainCode`

**Type:** `GradedCode`

**Code Structure:**
```
┌─────────────────────┐
│   Student Code      │ (implement add method)
├─────────────────────┤
│ DefaultMainCode     │ (shown when hitting Run)
│ + MainCode          │ (appended during Submit per testcase)
└─────────────────────┘
```

### Starter Code
```java
public class Solution {
    public static int add(int a, int b) {
        // TODO: implement
        return 0;
    }
}
```

### Flow Testing
- **Run Flow:** Appends `DefaultMainCode` → Outputs sample result (e.g., `5`)
- **Submit Flow:** Each testcase appends its own `MainCode` → Compares stdout with `expected_output`

### Test Cases

| # | Main Code Call | Expected Output | Hidden | Notes |
|---|---|---|---|---|
| 1 | `Solution.add(5, 3)` | `8` | No | Basic case |
| 2 | `Solution.add(-10, 10)` | `0` | No | Negative numbers |
| 3 | `Solution.add(50, 50)` | `100` | Yes | Hidden test |

---

## Exercise 2: Hello World ✅

**Purpose:** Test output-only exercise with NO `DefaultMainCode` and NO `MainCode`

**Type:** `GradedCode` (but output-only)

**Code Structure:**
```
┌──────────────────────────────┐
│      Student Code            │ (complete program with main)
│  (runs directly, no append)  │
└──────────────────────────────┘
```

### Starter Code
```
// Write your code here
// You need to print: Hello World
```

### Expected Solution
```java
public class Main {
    public static void main(String[] args) {
        System.out.println("Hello World");
    }
}
```

### Flow Testing
- **Run Flow:** Runs student code directly (no DefaultMainCode appended)
- **Submit Flow:** Runs student code directly (no MainCode appended), compares stdout with expected

### Test Cases

| # | Expected Output | TextInput | MainCode | Notes |
|---|---|---|---|---|
| 1 | `Hello World` | null | null | Student provides complete program |

---

## Exercise 3: Multiply Numbers ✅

**Purpose:** Test another function-based exercise to verify generalization

**Type:** `GradedCode`

### Starter Code
```java
public class Solution {
    public static int multiply(int a, int b) {
        // TODO: implement
        return 0;
    }
}
```

### Test Cases

| # | Main Code Call | Expected Output | Hidden |
|---|---|---|---|
| 1 | `Solution.multiply(4, 5)` | `20` | No |
| 2 | `Solution.multiply(0, 100)` | `0` | No |
| 3 | `Solution.multiply(-3, 5)` | `-15` | Yes |

---

## Exercise 4: Factorial (Recursion) ✅

**Purpose:** Test recursive function implementation

**Type:** `GradedCode`

### Starter Code
```java
public class Solution {
    public static long factorial(int n) {
        // TODO: implement recursively
        // Base case: if n <= 1, return 1
        // Recursive case: return n * factorial(n-1)
        return 0;
    }
}
```

### Expected Solution
```java
public class Solution {
    public static long factorial(int n) {
        if (n <= 1) return 1;
        return n * factorial(n - 1);
    }
}
```

### Test Cases

| # | Main Code Call | Expected Output | Hidden |
|---|---|---|---|
| 1 | `Solution.factorial(1)` | `1` | No |
| 2 | `Solution.factorial(5)` | `120` | No |
| 3 | `Solution.factorial(10)` | `3628800` | Yes |

---

## Exercise 5: Reverse String (Using Scanner Input) ✅

**Purpose:** Test input-based exercise with stdin (no MainCode)

**Type:** `GradedCode`

**Code Structure:**
```
┌──────────────────────────────────┐
│      Student Code                │
│  (reads from stdin, prints out)  │
│  (runs directly, no append)      │
└──────────────────────────────────┘
```

### Starter Code
```java
import java.util.Scanner;

public class Main {
    public static void main(String[] args) {
        Scanner sc = new Scanner(System.in);
        // TODO: read input and reverse it
    }
}
```

### Expected Solution
```java
import java.util.Scanner;

public class Main {
    public static void main(String[] args) {
        Scanner sc = new Scanner(System.in);
        String input = sc.nextLine();
        System.out.println(new StringBuilder(input).reverse());
    }
}
```

### Flow Testing
- **Run Flow:** No DefaultMainCode (student provides complete program)
- **Submit Flow:** Tests with stdin (TextInput) instead of MainCode

### Test Cases

| # | Expected Output | TextInput | MainCode | Notes |
|---|---|---|---|---|
| 1 | `olleh` | `hello` | null | Reverse simple word |
| 2 | `dlrow` | `world` | null | Another example |

---

## Key Testing Scenarios

### ✅ Scenario 1: Run Flow (DefaultMainCode)
```
POST /exercises/{id}/run
{
    "code": "public class Solution { ... }",  // student code
    "language": "java"
}

Backend Flow:
1. Check if exercise.DefaultMainCode != null → YES
2. Combine: student_code + "\n\n" + default_main_code
3. Compile & run combined code
4. Return stdout as result
```

**Expected Result for Exercise 1:**
```
{
    "output": "8\n",
    "runtime_ms": 142
}
```

---

### ✅ Scenario 2: Submit Flow - Function-based (MainCode per testcase)
```
POST /exercises/{id}/submit
{
    "code": "public class Solution { ... }",  // student code
    "language": "java"
}

Backend Flow:
For each testcase:
  1. Check if testcase.MainCode != null → YES
  2. Combine: student_code + "\n\n" + testcase.main_code
  3. Compile & run combined code
  4. Compare stdout with expected_output
  5. Record result
```

**Expected Response:**
```json
{
    "is_passed": true,
    "results": [
        {
            "testcase_id": "...",
            "is_passed": true,
            "actual_output": "8\n"
        },
        {
            "testcase_id": "...",
            "is_passed": true,
            "actual_output": "0\n"
        },
        {
            "testcase_id": "...",
            "is_passed": true,
            "actual_output": "100\n"
        }
    ]
}
```

---

### ✅ Scenario 3: Submit Flow - Output-only (No MainCode)
```
POST /exercises/{id}/submit
{
    "code": "public class Main { ... }",  // complete program
    "language": "java"
}

Backend Flow:
For each testcase:
  1. Check if testcase.MainCode != null → NO
  2. Run student_code directly
  3. Use stdin = testcase.TextInput if present
  4. Compare stdout with expected_output
```

**For Exercise 2 (Hello World):**
```
1. Run as-is (no append)
2. Expected: "Hello World\n"
3. Match ✅
```

**For Exercise 5 (Reverse String):**
```
1. Run with stdin = "hello"
2. Expected stdout: "olleh\n"
3. Match ✅
```

---

## Testing Instructions

### 1. Apply Migrations (if needed)
If seeder includes new schema changes:
```powershell
cd "d:\FPT\Semester 8\PRN232\Learn2Code\BE"
dotnet ef migrations add JavaExerciserSeeder --project Learn2Code.Infrastructure --startup-project Learn2Code.API
dotnet ef database update --project Learn2Code.Infrastructure --startup-project Learn2Code.API
```

### 2. Run Application
```powershell
cd "d:\FPT\Semester 8\PRN232\Learn2Code\BE\Learn2Code.API"
dotnet run
```

The seeder will automatically populate data on first run.

### 3. Test via Swagger
Navigate to: `https://localhost:7000/swagger`

#### Test Run Flow:
```
POST /api/exercises/{exerciseId}/run

Request Body (Exercise 1 - Sum):
{
    "code": "public class Solution {\n    public static int add(int a, int b) {\n        return a + b;\n    }\n}",
    "language": "java"
}

Expected Response:
{
    "success": true,
    "data": {
        "output": "8",
        "runtimeMs": 142
    }
}
```

#### Test Submit Flow:
```
POST /api/exercises/{exerciseId}/submit

Request Body:
{
    "code": "public class Solution {\n    public static int add(int a, int b) {\n        return a + b;\n    }\n}",
    "language": "java"
}

Expected Response:
{
    "success": true,
    "data": {
        "is_passed": true,
        "results": [
            {
                "testcase_id": "...",
                "is_passed": true,
                "actual_output": "8"
            },
            {
                "testcase_id": "...",
                "is_passed": true,
                "actual_output": "0"
            },
            {
                "testcase_id": "...",
                "is_passed": true,
                "actual_output": "100"
            }
        ]
    }
}
```

### 4. Test with Postman (Optional)

**Get Java Course:**
```
GET /api/courses?search=Java
```

**Get Exercises:**
```
GET /api/courses/{courseId}/sections/{sectionId}/lessons/{lessonId}/exercises
```

---

## Data Model Reference

### Exercise Entity
```csharp
public class Exercise
{
    public Guid ExerciseId { get; set; }
    public ExerciseType ExerciseType { get; set; }  // GradedCode
    public string StarterCode { get; set; }         // What student sees
    public string? DefaultMainCode { get; set; }    // Appended for Run flow (can be null)
    public string Language { get; set; }            // "java"
    public ICollection<TestCase> TestCases { get; set; }
}
```

### TestCase Entity
```csharp
public class TestCase
{
    public Guid TestCaseId { get; set; }
    public string ExpectedOutput { get; set; }
    public string? TextInput { get; set; }          // stdin for input-based tests
    public string? MainCode { get; set; }           // Appended for Submit flow (can be null)
    public bool IsHidden { get; set; }
}
```

---

## Expected Behavior Summary

| Exercise | DefaultMainCode | MainCode | Flow |
|---|---|---|---|
| 1 (Sum) | ✅ Present | ✅ Per testcase | Function-based |
| 2 (Hello) | ❌ Null | ❌ Null | Output-only |
| 3 (Multiply) | ✅ Present | ✅ Per testcase | Function-based |
| 4 (Factorial) | ✅ Present | ✅ Per testcase | Recursive |
| 5 (Reverse) | ❌ Null | ❌ Null | stdin-based |

---

## Troubleshooting

### Issue: Exercises not appearing after seed
- Verify: Check database with `SELECT * FROM exercises WHERE language = 'java';`
- Check logs for seeder errors during startup

### Issue: Run returns compilation error
- Ensure Java is installed: `java -version`
- Check starter code for syntax errors

### Issue: Submit test fails unexpectedly
- Verify `MainCode` is correctly formatted (no extra newlines/spaces after output)
- Check console output for exact stdout vs expected  

---

## Future Enhancements

Consider adding:
1. **Python exercises** - Similar pattern for Python
2. **C# exercises** - For .NET ecosystem
3. **JavaScript exercises** - For web development
4. **Multiple language support** - Test polyglot code submission
5. **Edge case tests** - Large input/output, timeout scenarios
