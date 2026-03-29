# Refactor: Submit Code Flow for Exercises

## Background

Clarified design:

- `starter_code` = what the student sees in the editor (function body, no main).
- `Exercise.DefaultMainCode` = a **default main** shown/used when student hits **Run** during the lesson (lets them see output with a sample call). **Null** for output-only exercises (e.g. print-hello).
- `TestCase.MainCode` = per-testcase main code appended during **Submit** to check each case independently.

**Run flow:**
- If `exercise.DefaultMainCode != null` → run `student_code + "\n\n" + default_main_code` as one file.
- If null → run `student_code` directly.

**Submit flow (GradedCode):**
- For each testcase:
  - If `testcase.MainCode != null` → run `student_code + "\n\n" + testcase.main_code`, compare stdout with `expected_output`.
  - If null → run `student_code` directly (stdin = `testcase.TextInput`), compare stdout.

> [!IMPORTANT]
> `Exercise.SolutionValidator` is **dropped**. `TestCase.ValidatorMain` is renamed to `TestCase.MainCode`. `Exercise.DefaultMainCode` is **new**. Requires a DB migration.

---

## Proposed Changes

### Domain Layer

#### [MODIFY] [Exercise.cs](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Domain/Entities/Exercise.cs)
- Remove `SolutionValidator` (column `solution_validator`).
- Add `DefaultMainCode` (column `default_main_code`, nullable): default main used when student hits Run.

#### [MODIFY] [TestCase.cs](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Domain/Entities/TestCase.cs)
- Rename `ValidatorMain` → `MainCode` (column `validator_main` → `main_code`).
- XML doc: "Per-testcase main code appended to student code during Submit. Null = run student code directly."

---

### Application Layer — DTOs

#### [MODIFY] [CreateTestCaseRequest.cs](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Application/DTOs/TestCaseDTOs/TestCaseRequests/CreateTestCaseRequest.cs)
- Rename `ValidatorMain` → `MainCode` (`json: "main_code"`).
- Remove old `ValidatorMain`.

#### [MODIFY] [UpdateTestCaseRequest.cs](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Application/DTOs/TestCaseDTOs/TestCaseRequests/UpdateTestCaseRequest.cs)
- Rename `ValidatorMain` → `MainCode` (`json: "main_code"`).

#### [MODIFY] [TestCaseDto.cs](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Application/DTOs/TestCaseDTOs/TestCaseResponses/TestCaseDto.cs)
- Rename `ValidatorMain` → `MainCode` (`json: "main_code"`).

#### [MODIFY] [CreateExerciseRequest.cs](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Application/DTOs/ExerciseDTOs/ExerciseRequests/CreateExerciseRequest.cs)
- Remove `SolutionValidator`, add `DefaultMainCode` (`json: "default_main_code"`, nullable).

#### [MODIFY] [UpdateExerciseRequest.cs](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Application/DTOs/ExerciseDTOs/ExerciseRequests/UpdateExerciseRequest.cs)
- Remove `SolutionValidator`, add `DefaultMainCode` (`json: "default_main_code"`, nullable).

#### [MODIFY] [ExerciseDetailDto.cs](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Application/DTOs/ExerciseDTOs/ExerciseResponses/ExerciseDetailDto.cs)
- Remove `SolutionValidator`, add `DefaultMainCode` (`json: "default_main_code"`) to response.

---

### Application Layer — Mapper

#### [MODIFY] [TestCaseMapper.cs](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Application/Mapper/TestCaseMapper.cs)
- `ValidatorMain` → `MainCode` in all mapper methods.

#### [MODIFY] [ExerciseMapper.cs](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Application/Mapper/ExerciseMapper.cs)
- Replace `SolutionValidator` with `DefaultMainCode` in [ToEntity](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Application/Mapper/TestCaseMapper.cs#26-41), [UpdateExercise](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Application/Mapper/ExerciseMapper.cs#84-127), [ToDetailDto](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Application/Mapper/ExerciseMapper.cs#26-51).
- Update `clearCodeFields` to also clear `DefaultMainCode`.

---

### Application Layer — Services

#### [MODIFY] [ExerciseService.cs](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Application/Services/ExerciseService.cs)

**RunCodeAsync path (updated):**
```
if exercise.DefaultMainCode != null:
    run (student_code + "\n\n" + default_main_code) as a single file
else:
    run student_code directly
```

**SubmitCodeAsync — new unified GradedCode path:**
```
foreach testcase in testCases:
    if testcase.MainCode != null:
        combinedCode = student_code + "\n\n" + testcase.MainCode
        run combinedCode (1 file), compare stdout with expected_output
    else:
        run student_code directly (stdin = testcase.TextInput)
        compare stdout with expected_output
```

- **Remove** [ExecuteWithValidatorAsync](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Application/Services/ExerciseService.cs#549-579), [ReplaceMainFunction](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Application/Services/ExerciseService.cs#654-683), all `ReplaceXxxMain`, [GetValidatorFileNames](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Application/Services/ExerciseService.cs#580-607).
- Simplify [SubmitCodeAsync](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Application/Services/ExerciseService.cs#208-363) to a single GradedCode branch (no more validator/fallback split).

#### [MODIFY] [TestCaseService.cs](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Application/Services/TestCaseService.cs)
- No logic change; just inherits rename from mapper.

---

### Infrastructure — EF Migration

A new EF migration is needed:
- Drop column `solution_validator` from `exercises`.
- Add column `default_main_code` (nullable text) to `exercises`.
- Rename column `validator_main` → `main_code` in `test_cases`.

---

### Documentation

#### [MODIFY] [Flow-api.md](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Flow-api.md)
- Update section **4.2 Exercise** submit flow to reflect the new model.
- Remove references to `solution_validator`.
- Describe `main_code` on testcase as the new mechanism.

#### [MODIFY] [dbdiagram.md](file:///c:/Users/Acer/Desktop/Learn2Code/BE/dbdiagram.md)
- [Exercises](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Application/Services/ExerciseService.cs#37-49): remove `solution_validator`, add `default_main_code` (nullable).
- [TestCases](file:///c:/Users/Acer/Desktop/Learn2Code/BE/Learn2Code.Application/Services/TestCaseService.cs#21-40): rename `validator_main` → `main_code`, update comment.

---

## Verification Plan

### Build Verification
```powershell
cd c:\Users\Acer\Desktop\Learn2Code\BE
dotnet build Learn2Code.BE.sln
```
Must build with no errors.

### EF Migration
```powershell
cd c:\Users\Acer\Desktop\Learn2Code\BE
dotnet ef migrations add RefactorSubmitFlow --project Learn2Code.Infrastructure --startup-project Learn2Code.API
dotnet ef database update --project Learn2Code.Infrastructure --startup-project Learn2Code.API
```

### Manual API Tests (via Swagger or Postman)

1. **Create a GradedCode exercise** (no `solution_validator` field should exist in request/response).
2. **Create testcase with `main_code`** (function-based) — verify `main_code` is stored and returned.
3. **Create testcase without `main_code`** (output-based) — verify works with empty/null `main_code`.
4. **Submit code** for function-based exercise — verify each testcase runs `student_code + main_code`, compare with `expected_output`.
5. **Submit code** for output-based exercise — verify runs `student_code` directly, compare with `expected_output`.
6. **Run code** — verify still runs student code directly, no testcase involvement.
