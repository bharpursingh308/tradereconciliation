# TradeReconciliationTool
A lightweight Windows Forms utility (VB.NET) for importing, viewing and reconciling trade exceptions stored in an SQLite database. Designed for small teams or solo use — simple UI, minimal dependencies, and testable data access.

## Key features
- Browse exceptions stored in a local SQLite database
- Filter by status and search text
- Resolve exceptions and record an audit actor/notes
- Small, well-separated codebase for easy maintenance and testing

## Prerequisites
- Visual Studio 2026 with the .NET desktop development workload
- NuGet package restore enabled (__Tools > NuGet Package Manager > Restore NuGet Packages__)
- `System.Data.SQLite` (restored from NuGet by the solution)

## Quick start (Visual Studio)
1. Clone the repository:
1. 2. Open the solution file (`.sln`) in Visual Studio 2026.
3. Restore NuGet packages: right-click the solution → `Restore NuGet Packages` or use __Tools > NuGet Package Manager__.
4. Build: __Build > Build Solution__.
5. Run: __Debug > Start Debugging__ (F5) or __Start Without Debugging__ (Ctrl+F5).

## Project layout
- `TradeReconciliationTool/` —WinForms project and source files
- `TradeReconciliationTool/ExceptionsForm.vb` — UI for listing and resolving exceptions
-`TradeReconciliationTool/ExceptionRepository.vb` — data access logic
- `CONTRIBUTING.md` — contribution guidelinesand coding standards

## Database notes
- The app uses SQLite; the database path is provided by the `Db.DbPath()` helper.
- Do not check local database files into source control. Ensure `.gitignore` includes `*.sqlite`, `*.db`, `*.db-wal`.
- For testing, provide a sample DB under `docs/sample-data` (keep sample data out of main branches).

##Contributing
Please follow the project `CONTRIBUTING.md`:
- Branches: `feature/<desc>`, `fix/<desc>`, `chore/<desc>`
- Commit messages: Conventional Commits (e.g., `fix(db): handle null ImportBatchId`)
- Create PRs against `main` and include build/test notes

## CI and quality
- Add a GitHub Actionsworkflow to build the solution and run tests on push and PRs.
- Recommended checks: Restore NuGet, Build Solution, Run unittests.

## Troubleshooting
- NuGet restore fails: verify Visual Studio workloads and network access to NuGet.
- SQLite runtime errors: confirm native runtime (x86/x64) matches build configuration.
- Designer issues: close the designer and rebuild the solution.

## License
Add a `LICENSE` at the repository root (MIT is recommended for permissive usage).

## Support
Open an issue for bugs or feature requests. For contribution questions, see `CONTRIBUTING.md`.	# TradeReconciliationTool