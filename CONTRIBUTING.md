# Contributing to TradeReconciliationTool

Thank you for your interest in contributing to TradeReconciliationTool. This document describes the recommended workflow, coding standards, and repository rules to ensure consistent, high-quality contributions.

## Guidelines

- Be respectful and constructive in all communication.
- Open an issue before implementing large features or breaking changes to discuss design.

## Branching and Workflow

- Use feature branches off `main` (or `master` if applicable):
  - Branch name format: `feature/<short-description>`
  - For bug fixes: `fix/<short-description>`
  - For chores/documentation: `chore/<short-description>`
- Create a Pull Request (PR) targeting `main` when ready. PR titles should be concise and reference related issue numbers when applicable (e.g., `Fix: handle null trade id (#42)`).

## Commit Messages

- Follow Conventional Commits: `type(scope?): subject`.
  - Examples: `feat(ui): add exception filtering`, `fix(db): handle null ImportBatchId`
- Keep subject line under 72 characters and use imperative mood.

## Code Style and Formatting

- We include an `.editorconfig` at the repository root. Please ensure your editor respects it.
- Basic rules enforced by `.editorconfig` (also present in the repo):
  - Indent: 4 spaces
  - End of line: LF
  - Charset: utf-8
  - Trim trailing whitespace: true
  - Insert final newline: true
- Keep names clear and follow existing naming conventions in the codebase.
- Prefer simple, testable, and well-documented changes.

## Pull Request Checklist

Before requesting review:
- [ ] Branch follows naming conventions
- [ ] PR targets the correct base branch (`main`)
- [ ] Code compiles in Visual Studio (__Build > Build Solution__)
- [ ] No sensitive information (API keys, passwords) included
- [ ] Relevant unit tests added or updated
- [ ] Descriptive PR description with motivation and changes

## Tests and CI

- Add unit tests where applicable and ensure existing tests pass.
- The repository should include a CI workflow (e.g., GitHub Actions) to build the solution and run tests.

## Files to Include

- `.editorconfig` — enforce formatting
- `CONTRIBUTING.md` — this document
- `LICENSE` — include a license (MIT or other) if not already present
- `README.md` — project overview and build instructions

## Reporting Security Issues

If you discover a security vulnerability, please report it privately to the repository owners instead of opening a public issue.

## Contact

If you have questions about the contribution process, open an issue or contact the maintainers.