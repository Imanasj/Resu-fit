# Week 6 – Testing & Refinement

## Contributor
Jun Wang

## Project
Resume Match – Resume & Job Posting Compatibility Scanner

## Objective
The goal of Week 6 was to test the Resume Match application, identify bugs, improve match-score accuracy, and verify the main user workflow.

## Test Results

| Test Case | Description | Expected Result | Actual Result | Status |
|---|---|---|---|---|
| Test 1 | Resume and job posting contain the same technical skills | High match score with correct matched keywords | Initially returned 100%, but Java and .NET were falsely detected | Bug Found |
| Test 1 Retest | Same input after keyword-matching fix | Only exact skills should be matched | 100%, 6 matched, 0 missing | PASS |
| Test 2 | Resume contains 3 of 6 required skills | 50% score, 3 matched and 3 missing | 50%, 3 matched, 3 missing | PASS |
| Test 3 | Resume field left empty | Application should prevent submission and display validation | "Please enter both resume text and job description." | PASS |
| Test 4 | Completely unrelated resume and software developer job posting | 0% or very low match score | 0% | PASS |

## Bug Identified

The original keyword matching logic used substring matching.

This caused false-positive matches:

- "Java" was detected inside "JavaScript".
- ".NET" was detected inside "ASP.NET".

## Fix Implemented

The keyword matching logic was updated to use regular-expression boundaries instead of simple substring matching.

After the fix:

- JavaScript no longer incorrectly matches Java.
- ASP.NET no longer automatically creates a separate .NET match.
- C#, React, JavaScript, SQL, PostgreSQL, and ASP.NET are detected correctly.

## Accuracy Improvement

Before the fix, the test returned:

- 8 matched keywords
- 0 missing keywords
- 100%

After the fix, the same input correctly returned:

- 6 matched keywords
- 0 missing keywords
- 100%

The score remained 100% because all actual job requirements were present, but duplicate and false-positive keyword matches were removed.

## Additional Verification

The application was also verified for:

- Frontend startup
- Backend API startup
- PostgreSQL scan persistence
- Scan history
- Missing keyword detection
- Empty-input validation
- Low-match scenarios

## Conclusion

Week 6 testing confirmed that the main Resume Match workflow works correctly. A keyword-matching accuracy issue was identified and fixed. The application now produces more reliable keyword results and correctly handles full matches, partial matches, unrelated resumes, and empty input.